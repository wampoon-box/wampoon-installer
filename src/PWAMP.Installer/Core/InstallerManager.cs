using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PWAMP.Installer.Core;
using PWAMP.Installer.Events;
using PWAMP.Installer.Models;

namespace PWAMP.Installer.Core
{
    public class InstallerManager : IDisposable
    {
        private readonly PackageRepository _packageRepository;
        private readonly PackageDownloader _downloader;
        private readonly ArchiveExtractor _extractor;
        private readonly string _tempDirectory;
        private readonly string _installRootDirectory;
        private bool _disposed = false;

        public event EventHandler<DownloadProgressEventArgs> DownloadProgressReported;
        public event EventHandler<InstallationProgressEventArgs> InstallationProgressReported;
        public event EventHandler<InstallerErrorEventArgs> ErrorOccurred;
        public event EventHandler<InstallationCompletedEventArgs> InstallationCompleted;

        public InstallerManager(string installRootDirectory = null)
        {
            _packageRepository = new PackageRepository();
            _downloader = new PackageDownloader();
            _extractor = new ArchiveExtractor();
            
            _installRootDirectory = installRootDirectory ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PWAMP");
            _tempDirectory = Path.Combine(Path.GetTempPath(), "PWAMP-Installer", Guid.NewGuid().ToString("N").Substring(0, 8));

            _downloader.ProgressReported += (s, e) => { if (DownloadProgressReported != null) DownloadProgressReported.Invoke(this, e); };
            _extractor.ProgressReported += (s, e) => { if (InstallationProgressReported != null) InstallationProgressReported.Invoke(this, e); };

            Directory.CreateDirectory(_tempDirectory);
        }

        public async Task<List<InstallablePackage>> GetAvailablePackagesAsync()
        {
            try
            {
                return await _packageRepository.GetAvailablePackagesAsync();
            }
            catch (Exception ex)
            {
                OnErrorOccurred(new InstallerErrorEventArgs
                {
                    Exception = ex,
                    Message = "Failed to retrieve available packages",
                    ErrorType = ErrorType.Network,
                    IsFatal = false
                });
                return new List<InstallablePackage>();
            }
        }

        public List<InstallablePackage> ResolveDependencies(List<InstallablePackage> selectedPackages)
        {
            return _packageRepository.ResolveDependencies(selectedPackages);
        }

        public async Task<bool> InstallPackagesAsync(List<InstallablePackage> packages, CancellationToken cancellationToken = default)
        {
            if (packages == null || packages.Count == 0)
                return false;

            var resolvedPackages = ResolveDependencies(packages);
            var totalPackages = resolvedPackages.Count;
            var currentPackage = 0;

            try
            {
                OnInstallationProgressReported(new InstallationProgressEventArgs
                {
                    CurrentOperation = "Starting installation process...",
                    Stage = InstallationStage.Initializing,
                    TotalSteps = totalPackages,
                    CompletedSteps = 0,
                    PercentComplete = 0
                });

                if (!ValidatePrerequisites())
                {
                    OnErrorOccurred(new InstallerErrorEventArgs
                    {
                        Message = "System prerequisites validation failed",
                        ErrorType = ErrorType.Validation,
                        IsFatal = true
                    });
                    return false;
                }

                foreach (var package in resolvedPackages)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var packageStartTime = DateTime.UtcNow;
                    currentPackage++;

                    try
                    {
                        OnInstallationProgressReported(new InstallationProgressEventArgs
                        {
                            PackageName = package.Name,
                            CurrentOperation = string.Format("Installing {0} ({1} of {2})", package.Name, currentPackage, totalPackages),
                            Stage = InstallationStage.Initializing,
                            TotalSteps = totalPackages,
                            CompletedSteps = currentPackage - 1,
                            PercentComplete = (double)(currentPackage - 1) / totalPackages * 100
                        });

                        await InstallSinglePackageAsync(package, cancellationToken);

                        var duration = DateTime.UtcNow - packageStartTime;
                        OnInstallationCompleted(new InstallationCompletedEventArgs
                        {
                            Success = true,
                            PackageName = package.Name,
                            Duration = duration,
                            InstallPath = Path.Combine(_installRootDirectory, package.InstallPath),
                            Message = string.Format("{0} installed successfully", package.Name)
                        });
                    }
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch (Exception ex)
                    {
                        OnErrorOccurred(new InstallerErrorEventArgs
                        {
                            Exception = ex,
                            Message = string.Format("Failed to install {0}", package.Name),
                            PackageName = package.Name,
                            ErrorType = GetErrorType(ex),
                            IsFatal = false
                        });

                        OnInstallationCompleted(new InstallationCompletedEventArgs
                        {
                            Success = false,
                            PackageName = package.Name,
                            Duration = DateTime.UtcNow - packageStartTime,
                            Message = string.Format("Failed to install {0}: {1}", package.Name, ex.Message)
                        });

                        continue;
                    }
                }

                OnInstallationProgressReported(new InstallationProgressEventArgs
                {
                    CurrentOperation = "Installation process completed",
                    Stage = InstallationStage.Completed,
                    TotalSteps = totalPackages,
                    CompletedSteps = totalPackages,
                    PercentComplete = 100
                });

                return true;
            }
            catch (OperationCanceledException)
            {
                OnInstallationProgressReported(new InstallationProgressEventArgs
                {
                    CurrentOperation = "Installation cancelled by user",
                    Stage = InstallationStage.Failed,
                    PercentComplete = 0
                });
                return false;
            }
            catch (Exception ex)
            {
                OnErrorOccurred(new InstallerErrorEventArgs
                {
                    Exception = ex,
                    Message = "Installation process failed",
                    ErrorType = ErrorType.Unknown,
                    IsFatal = true
                });
                return false;
            }
            finally
            {
                CleanupTempFiles();
            }
        }

        private async Task InstallSinglePackageAsync(InstallablePackage package, CancellationToken cancellationToken)
        {
            var finalInstallPath = Path.Combine(_installRootDirectory, package.InstallPath);

            OnInstallationProgressReported(new InstallationProgressEventArgs
            {
                PackageName = package.Name,
                CurrentOperation = "Starting download...",
                Stage = InstallationStage.Downloading,
                PercentComplete = 0
            });

            var downloadedFile = await _downloader.DownloadPackageAsync(package, _tempDirectory, cancellationToken);

            OnInstallationProgressReported(new InstallationProgressEventArgs
            {
                PackageName = package.Name,
                CurrentOperation = "Validating downloaded file...",
                Stage = InstallationStage.Downloading,
                PercentComplete = 100
            });

            if (!string.IsNullOrEmpty(package.ChecksumUrl))
            {
                var isValid = await _downloader.ValidateChecksumAsync(downloadedFile, package.ChecksumUrl, cancellationToken);
                if (!isValid)
                {
                    throw new InvalidOperationException(string.Format("Checksum validation failed for {0}", package.Name));
                }
            }

            OnInstallationProgressReported(new InstallationProgressEventArgs
            {
                PackageName = package.Name,
                CurrentOperation = "Extracting package...",
                Stage = InstallationStage.Extracting,
                PercentComplete = 0
            });

            await _extractor.ExtractPackageAsync(package, downloadedFile, finalInstallPath, cancellationToken);

            OnInstallationProgressReported(new InstallationProgressEventArgs
            {
                PackageName = package.Name,
                CurrentOperation = "Configuring package...",
                Stage = InstallationStage.Configuring,
                PercentComplete = 90
            });

            await ConfigurePackageAsync(package, finalInstallPath, cancellationToken);

            OnInstallationProgressReported(new InstallationProgressEventArgs
            {
                PackageName = package.Name,
                CurrentOperation = "Installation completed",
                Stage = InstallationStage.Completed,
                PercentComplete = 100
            });
        }

        private async Task ConfigurePackageAsync(InstallablePackage package, string installPath, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                switch (package.Type)
                {
                    case PackageType.Apache:
                        ConfigureApache(installPath);
                        break;
                    case PackageType.MariaDB:
                    case PackageType.MySQL:
                        ConfigureDatabase(installPath, package);
                        break;
                    case PackageType.PHP:
                        ConfigurePHP(installPath);
                        break;
                    case PackageType.PhpMyAdmin:
                        ConfigurePhpMyAdmin(installPath);
                        break;
                }
            }, cancellationToken);
        }

        private void ConfigureApache(string installPath)
        {
            var confDir = Path.Combine(installPath, "conf");
            if (!Directory.Exists(confDir)) return;

            var httpConfPath = Path.Combine(confDir, "httpd.conf");
            if (File.Exists(httpConfPath))
            {
                var documentRoot = Path.Combine(_installRootDirectory, "htdocs");
                Directory.CreateDirectory(documentRoot);

                var config = File.ReadAllText(httpConfPath);
                config = config.Replace("c:/Apache24/htdocs", documentRoot.Replace("\\", "/"));
                config = config.Replace("c:/Apache24", installPath.Replace("\\", "/"));
                
                File.WriteAllText(httpConfPath, config);
            }
        }

        private void ConfigureDatabase(string installPath, InstallablePackage package)
        {
            var dataDir = Path.Combine(_installRootDirectory, "mysql-data");
            Directory.CreateDirectory(dataDir);

            var confFile = Path.Combine(installPath, "my.ini");
            if (!File.Exists(confFile))
            {
                var configContent = string.Format(@"[mysqld]
basedir={0}
datadir={1}
port=3306
server_id=1
", installPath.Replace("\\", "/"), dataDir.Replace("\\", "/"));
                File.WriteAllText(confFile, configContent);
            }
        }

        private void ConfigurePHP(string installPath)
        {
            var phpIniDev = Path.Combine(installPath, "php.ini-development");
            var phpIni = Path.Combine(installPath, "php.ini");
            
            if (File.Exists(phpIniDev) && !File.Exists(phpIni))
            {
                File.Copy(phpIniDev, phpIni);
            }
        }

        private void ConfigurePhpMyAdmin(string installPath)
        {
            var configSample = Path.Combine(installPath, "config.sample.inc.php");
            var config = Path.Combine(installPath, "config.inc.php");
            
            if (File.Exists(configSample) && !File.Exists(config))
            {
                var configContent = File.ReadAllText(configSample);
                configContent = configContent.Replace("$cfg['blowfish_secret'] = '';", 
                    string.Format("$cfg['blowfish_secret'] = '{0}';", GenerateBlowfishSecret()));
                File.WriteAllText(config, configContent);
            }
        }

        private string GenerateBlowfishSecret()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 32).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public bool ValidatePrerequisites()
        {
            try
            {
                if (!HasAdministratorPrivileges())
                    return false;

                if (!HasSufficientDiskSpace())
                    return false;

                if (!HasInternetConnectivity())
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool HasAdministratorPrivileges()
        {
            try
            {
                var testDir = Path.Combine(_installRootDirectory, "test");
                Directory.CreateDirectory(testDir);
                Directory.Delete(testDir);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool HasSufficientDiskSpace()
        {
            try
            {
                var drive = new DriveInfo(Path.GetPathRoot(_installRootDirectory));
                return drive.AvailableFreeSpace > 500L * 1024L * 1024L; // 500MB minimum
            }
            catch
            {
                return false;
            }
        }

        private bool HasInternetConnectivity()
        {
            try
            {
                using (var client = new System.Net.NetworkInformation.Ping())
                {
                    var reply = client.Send("8.8.8.8", 5000);
                    return reply.Status == System.Net.NetworkInformation.IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        private ErrorType GetErrorType(Exception ex)
        {
            if (ex is UnauthorizedAccessException || ex is DirectoryNotFoundException)
                return ErrorType.Permission;
            if (ex is System.Net.Http.HttpRequestException || ex is System.Net.WebException)
                return ErrorType.Network;
            if (ex is IOException || ex is FileNotFoundException)
                return ErrorType.FileSystem;
            if (ex is InvalidDataException || ex is InvalidOperationException)
                return ErrorType.Validation;
            return ErrorType.Unknown;
        }

        private void CleanupTempFiles()
        {
            try
            {
                if (Directory.Exists(_tempDirectory))
                    Directory.Delete(_tempDirectory, true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }

        protected virtual void OnDownloadProgressReported(DownloadProgressEventArgs e)
        {
            if (DownloadProgressReported != null)
                DownloadProgressReported.Invoke(this, e);
        }

        protected virtual void OnInstallationProgressReported(InstallationProgressEventArgs e)
        {
            if (InstallationProgressReported != null)
                InstallationProgressReported.Invoke(this, e);
        }

        protected virtual void OnErrorOccurred(InstallerErrorEventArgs e)
        {
            if (ErrorOccurred != null)
                ErrorOccurred.Invoke(this, e);
        }

        protected virtual void OnInstallationCompleted(InstallationCompletedEventArgs e)
        {
            if (InstallationCompleted != null)
                InstallationCompleted.Invoke(this, e);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_packageRepository != null)
                    _packageRepository.Dispose();
                if (_downloader != null)
                    _downloader.Dispose();
                if (_extractor != null)
                    _extractor.Dispose();
                CleanupTempFiles();
                _disposed = true;
            }
        }
    }
}