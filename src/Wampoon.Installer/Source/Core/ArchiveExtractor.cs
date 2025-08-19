using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Wampoon.Installer.Events;
using Wampoon.Installer.Models;
using Wampoon.Installer.Helpers;

namespace Wampoon.Installer.Core
{
    public class ArchiveExtractor : IDisposable
    {
        private bool _disposed = false;

        public event EventHandler<InstallationProgressEventArgs> ProgressReported;

        public async Task<string> ExtractPackageAsync(InstallablePackage package, string archivePath, string extractPath, CancellationToken cancellationToken = default)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));
            if (string.IsNullOrEmpty(archivePath)) throw new ArgumentException("Archive path cannot be null or empty", nameof(archivePath));
            if (string.IsNullOrEmpty(extractPath)) throw new ArgumentException("Extract path cannot be null or empty", nameof(extractPath));

            if (!File.Exists(archivePath))
                throw new FileNotFoundException($"Archive file not found: {archivePath}");

            // Handle DLL files (like Xdebug) specially - just copy them
            if (package.ArchiveFormat?.ToLower() == "dll" || archivePath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                return await HandleDllPackageAsync(package, archivePath, extractPath, cancellationToken);
            }
            
            // Handle .phar files (like Composer) specially - just copy them
            if (package.ArchiveFormat?.ToLower() == "phar" || archivePath.EndsWith(".phar", StringComparison.OrdinalIgnoreCase))
            {
                return await HandlePharPackageAsync(package, archivePath, extractPath, cancellationToken);
            }

            // Handle VC++ runtime specially - extract and distribute to multiple directories
            if (package.Type == PackageType.VCRuntime)
            {
                return await HandleVCRuntimePackageAsync(package, archivePath, extractPath, cancellationToken);
            }

            // Validate ZIP file integrity before attempting extraction.
            try
            {
                using (var testArchive = ZipFile.OpenRead(archivePath))
                {
                    // Try to read the central directory.
                    var entryCount = testArchive.Entries.Count;
                    OnProgressReported(new InstallationProgressEventArgs
                    {
                        PackageName = package.Name,
                        CurrentOperation = $"Validated archive with {entryCount} entries",
                        Stage = InstallationStage.Extracting,
                        PercentComplete = 0
                    });
                }
            }
            catch (InvalidDataException ex)
            {
                throw new InvalidOperationException($"Archive file appears to be corrupted or incomplete: {archivePath}. Error: {ex.Message}. Please try downloading the package again.", ex);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogExceptionInfo(ex);
                throw new InvalidOperationException($"Failed to validate archive: {archivePath}. Error: {ex.Message}", ex);
            }

            try
            {
                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = "Preparing extraction...",
                    Stage = InstallationStage.Extracting,
                    PercentComplete = 0
                });

                if (Directory.Exists(extractPath))
                {
                    OnProgressReported(new InstallationProgressEventArgs
                    {
                        PackageName = package.Name,
                        CurrentOperation = "Cleaning existing directory...",
                        Stage = InstallationStage.Extracting,
                        PercentComplete = 5
                    });

                    Directory.Delete(extractPath, true);
                }

                Directory.CreateDirectory(extractPath);

                using (var archive = ZipFile.OpenRead(archivePath))
                {
                    var totalEntries = archive.Entries.Count;
                    var extractedEntries = 0;

                    OnProgressReported(new InstallationProgressEventArgs
                    {
                        PackageName = package.Name,
                        CurrentOperation = $"Extracting {totalEntries} files...",
                        Stage = InstallationStage.Extracting,
                        PercentComplete = 10,
                        TotalSteps = totalEntries,
                        CompletedSteps = 0
                    });

                    var rootDirName = GetRootDirectoryName(archive);
                    var shouldFlattenRoot = !string.IsNullOrEmpty(rootDirName) && 
                                           ShouldFlattenRootDirectory(archive, rootDirName);

                    foreach (var entry in archive.Entries)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        if (string.IsNullOrEmpty(entry.Name))
                            continue;

                        var destinationPath = GetDestinationPath(entry.FullName, extractPath, shouldFlattenRoot ? rootDirName : null);
                        
                        // Validate destination path to prevent path traversal attacks
                        if (!IsPathSafe(destinationPath, extractPath))
                        {
                            OnProgressReported(new InstallationProgressEventArgs
                            {
                                PackageName = package.Name,
                                CurrentOperation = $"Skipping unsafe path: {entry.FullName}",
                                Stage = InstallationStage.Extracting
                            });
                            continue;
                        }
                        
                        var destinationDir = Path.GetDirectoryName(destinationPath);
                        if (!Directory.Exists(destinationDir))
                            Directory.CreateDirectory(destinationDir);

                        if (entry.FullName.EndsWith("/") || entry.FullName.EndsWith("\\"))
                        {
                            if (!Directory.Exists(destinationPath))
                                Directory.CreateDirectory(destinationPath);
                        }
                        else
                        {
                            try
                            {
                                entry.ExtractToFile(destinationPath, overwrite: true);
                            }
                            catch (UnauthorizedAccessException)
                            {
                                OnProgressReported(new InstallationProgressEventArgs
                                {
                                    PackageName = package.Name,
                                    CurrentOperation = $"Permission issue with: {Path.GetFileName(destinationPath)}",
                                    Stage = InstallationStage.Extracting
                                });
                            }
                        }

                        extractedEntries++;
                        
                        // Only report at 25%, 50%, 75% intervals.
                        var progress = 10 + (extractedEntries * 80 / totalEntries);
                        var currentQuarter = progress / 25;
                        var lastQuarter = (10 + ((extractedEntries - 1) * 80 / totalEntries)) / 25;
                        
                        if (currentQuarter > lastQuarter && progress >= 25)
                        {
                            OnProgressReported(new InstallationProgressEventArgs
                            {
                                PackageName = package.Name,
                                CurrentOperation = $"Extracting... ({progress}%)",
                                Stage = InstallationStage.Extracting,
                                PercentComplete = progress,
                                TotalSteps = totalEntries,
                                CompletedSteps = extractedEntries
                            });
                        }
                    }
                }

                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = "Validating extracted files...",
                    Stage = InstallationStage.Validating,
                    PercentComplete = 95
                });

                await ValidateExtraction(package, extractPath, cancellationToken);

                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = "Extraction completed successfully",
                    Stage = InstallationStage.Completed,
                    PercentComplete = 100
                });

                return extractPath;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogExceptionInfo(ex);
                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = $"Extraction failed: {ex.Message}",
                    Stage = InstallationStage.Failed,
                    PercentComplete = 0
                });

                if (Directory.Exists(extractPath))
                {
                    try { Directory.Delete(extractPath, true); } catch { }
                }

                throw new InvalidOperationException($"Failed to extract {package.Name}: {ex.Message}", ex);
            }
        }

        private async Task<string> HandleDllPackageAsync(InstallablePackage package, string dllPath, string extractPath, CancellationToken cancellationToken)
        {
            try
            {
                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = "Preparing DLL installation...",
                    Stage = InstallationStage.Extracting,
                    PercentComplete = 10
                });

                // Create the extract directory if it doesn't exist
                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);
                }
                Directory.CreateDirectory(extractPath);

                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = "Copying DLL file...",
                    Stage = InstallationStage.Extracting,
                    PercentComplete = 50
                });

                // Copy the DLL file to the extract path
                var fileName = Path.GetFileName(dllPath);
                var destinationPath = Path.Combine(extractPath, fileName);
                
                await Task.Run(() => File.Copy(dllPath, destinationPath, overwrite: true), cancellationToken);

                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = "Validating DLL file...",
                    Stage = InstallationStage.Validating,
                    PercentComplete = 90
                });

                // Validate the copied file
                if (!File.Exists(destinationPath))
                {
                    throw new InvalidOperationException("Failed to copy DLL file to destination");
                }

                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = "DLL installation completed",
                    Stage = InstallationStage.Completed,
                    PercentComplete = 100
                });

                return extractPath;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogExceptionInfo(ex);
                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = $"DLL installation failed: {ex.Message}",
                    Stage = InstallationStage.Failed,
                    PercentComplete = 0
                });

                if (Directory.Exists(extractPath))
                {
                    try { Directory.Delete(extractPath, true); } catch { }
                }

                throw new InvalidOperationException($"Failed to install DLL {package.Name}: {ex.Message}", ex);
            }
        }

        private async Task<string> HandlePharPackageAsync(InstallablePackage package, string pharPath, string extractPath, CancellationToken cancellationToken)
        {
            try
            {
                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = "Preparing .phar file installation...",
                    Stage = InstallationStage.Extracting,
                    PercentComplete = 10
                });

                // Create the extract directory if it doesn't exist
                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, true);
                }
                Directory.CreateDirectory(extractPath);

                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = "Copying .phar file...",
                    Stage = InstallationStage.Extracting,
                    PercentComplete = 50
                });

                // Copy the .phar file to the extract path
                var fileName = Path.GetFileName(pharPath);
                var destinationPath = Path.Combine(extractPath, fileName);
                
                await Task.Run(() => File.Copy(pharPath, destinationPath, overwrite: true), cancellationToken);

                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = "Validating .phar file...",
                    Stage = InstallationStage.Validating,
                    PercentComplete = 90
                });

                // Validate the copied file
                if (!File.Exists(destinationPath))
                {
                    throw new InvalidOperationException("Failed to copy .phar file to destination");
                }

                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = ".phar file installation completed",
                    Stage = InstallationStage.Completed,
                    PercentComplete = 100
                });

                return extractPath;
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogExceptionInfo(ex);
                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = $".phar file installation failed: {ex.Message}",
                    Stage = InstallationStage.Failed,
                    PercentComplete = 0
                });

                if (Directory.Exists(extractPath))
                {
                    try { Directory.Delete(extractPath, true); } catch { }
                }

                throw new InvalidOperationException($"Failed to install .phar file {package.Name}: {ex.Message}", ex);
            }
        }

        private async Task<string> HandleVCRuntimePackageAsync(InstallablePackage package, string archivePath, string extractPath, CancellationToken cancellationToken)
        {
            try
            {
                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = "Preparing VC++ Runtime installation...",
                    Stage = InstallationStage.Extracting,
                    PercentComplete = 10
                });

                var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempPath);

                try
                {
                    OnProgressReported(new InstallationProgressEventArgs
                    {
                        PackageName = package.Name,
                        CurrentOperation = "Extracting VC++ Runtime files...",
                        Stage = InstallationStage.Extracting,
                        PercentComplete = 30
                    });

                    using (var archive = ZipFile.OpenRead(archivePath))
                    {
                        archive.ExtractToDirectory(tempPath);
                    }

                    OnProgressReported(new InstallationProgressEventArgs
                    {
                        PackageName = package.Name,
                        CurrentOperation = "Distributing VC++ Runtime files...",
                        Stage = InstallationStage.Extracting,
                        PercentComplete = 50
                    });

                    var installDir = Directory.GetParent(extractPath).Parent.FullName;
                    var targetDirectories = new[]
                    {
                        Path.Combine(installDir, "apps", "php"),
                        Path.Combine(installDir, "apps", "mariadb", "bin"),
                        Path.Combine(installDir, "apps", "apache", "bin")
                    };

                    var files = Directory.GetFiles(tempPath, "*", SearchOption.AllDirectories);
                    var totalOperations = files.Length * targetDirectories.Length;
                    var currentOperation = 0;

                    foreach (var targetDir in targetDirectories)
                    {
                        Directory.CreateDirectory(targetDir);
                        
                        foreach (var file in files)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            
                            var fileName = Path.GetFileName(file);
                            var targetFile = Path.Combine(targetDir, fileName);
                            
                            await Task.Run(() => File.Copy(file, targetFile, overwrite: true), cancellationToken);
                            
                            currentOperation++;
                            var progress = 50 + (currentOperation * 40 / totalOperations);
                            
                            if (currentOperation % 5 == 0 || currentOperation == totalOperations)
                            {
                                OnProgressReported(new InstallationProgressEventArgs
                                {
                                    PackageName = package.Name,
                                    CurrentOperation = $"Copying to {Path.GetFileName(targetDir)}... ({progress}%)",
                                    Stage = InstallationStage.Extracting,
                                    PercentComplete = progress
                                });
                            }
                        }
                    }

                    OnProgressReported(new InstallationProgressEventArgs
                    {
                        PackageName = package.Name,
                        CurrentOperation = "VC++ Runtime installation completed",
                        Stage = InstallationStage.Completed,
                        PercentComplete = 100
                    });

                    // Don't create the vcruntime directory - files are distributed directly to target directories
                    return extractPath;
                }
                finally
                {
                    try { Directory.Delete(tempPath, true); } catch { }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogExceptionInfo(ex);
                OnProgressReported(new InstallationProgressEventArgs
                {
                    PackageName = package.Name,
                    CurrentOperation = $"VC++ Runtime installation failed: {ex.Message}",
                    Stage = InstallationStage.Failed,
                    PercentComplete = 0
                });

                if (Directory.Exists(extractPath))
                {
                    try { Directory.Delete(extractPath, true); } catch { }
                }

                throw new InvalidOperationException($"Failed to install VC++ Runtime {package.Name}: {ex.Message}", ex);
            }
        }

        private string GetRootDirectoryName(ZipArchive archive)
        {
            string rootDir = null;
            
            foreach (var entry in archive.Entries)
            {
                if (string.IsNullOrEmpty(entry.FullName)) continue;
                
                var parts = entry.FullName.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;
                
                if (rootDir == null)
                {
                    rootDir = parts[0];
                }
                else if (rootDir != parts[0])
                {
                    return null;
                }
            }
            
            return rootDir;
        }

        private bool ShouldFlattenRootDirectory(ZipArchive archive, string rootDirName)
        {
            var rootDirEntryCount = 0;
            var hasFilesInRoot = false;
            
            foreach (var entry in archive.Entries)
            {
                if (string.IsNullOrEmpty(entry.FullName)) continue;
                
                var parts = entry.FullName.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;
                
                if (parts[0] == rootDirName)
                {
                    rootDirEntryCount++;
                    if (parts.Length == 1 && !string.IsNullOrEmpty(entry.Name))
                        hasFilesInRoot = true;
                }
            }
            
            return rootDirEntryCount > 1 && !hasFilesInRoot;
        }

        private string GetDestinationPath(string entryPath, string extractPath, string rootDirToRemove)
        {
            var relativePath = entryPath.Replace('/', Path.DirectorySeparatorChar);
            
            if (!string.IsNullOrEmpty(rootDirToRemove))
            {
                var rootPrefix = rootDirToRemove + Path.DirectorySeparatorChar;
                if (relativePath.StartsWith(rootPrefix))
                    relativePath = relativePath.Substring(rootPrefix.Length);
            }
            
            return Path.Combine(extractPath, relativePath);
        }

        private Task ValidateExtraction(InstallablePackage package, string extractPath, CancellationToken cancellationToken)
        {
            if (!Directory.Exists(extractPath))
                throw new DirectoryNotFoundException($"Extract directory not found: {extractPath}");

            var extractedFiles = Directory.GetFiles(extractPath, "*", SearchOption.AllDirectories);
            if (extractedFiles.Length == 0)
                throw new InvalidOperationException("No files were extracted from the archive");

            switch (package.Type)
            {
                case PackageType.Apache:
                    ValidateApacheExtraction(extractPath);
                    break;
                case PackageType.MariaDB:
                case PackageType.MySQL:
                    ValidateDatabaseExtraction(extractPath);
                    break;
                case PackageType.PHP:
                    ValidatePhpExtraction(extractPath);
                    break;
                case PackageType.PhpMyAdmin:
                    ValidatePhpMyAdminExtraction(extractPath);
                    break;
                case PackageType.Xdebug:
                    ValidateXdebugExtraction(extractPath);
                    break;
                case PackageType.VCRuntime:
                    ValidateVCRuntimeExtraction(extractPath);
                    break;
            }
            
            return Task.CompletedTask;
        }

        private void ValidateApacheExtraction(string extractPath)
        {
            var expectedFiles = new[] { "httpd.exe", "ApacheMonitor.exe" };
            foreach (var file in expectedFiles)
            {
                if (!FileExists(extractPath, file))
                    throw new InvalidOperationException($"Apache validation failed: {file} not found");
            }
        }

        private void ValidateDatabaseExtraction(string extractPath)
        {
            var expectedFiles = new[] { "mysqld.exe", "mysql.exe" };
            foreach (var file in expectedFiles)
            {
                if (!FileExists(extractPath, file))
                    throw new InvalidOperationException($"Database validation failed: {file} not found");
            }
        }

        private void ValidatePhpExtraction(string extractPath)
        {
            var expectedFiles = new[] { "php.exe", "php-cgi.exe" };
            foreach (var file in expectedFiles)
            {
                if (!FileExists(extractPath, file))
                    throw new InvalidOperationException($"PHP validation failed: {file} not found");
            }
        }

        private void ValidatePhpMyAdminExtraction(string extractPath)
        {
            var expectedFiles = new[] { "index.php", "config.sample.inc.php" };
            foreach (var file in expectedFiles)
            {
                if (!FileExists(extractPath, file))
                    throw new InvalidOperationException($"phpMyAdmin validation failed: {file} not found");
            }
        }

        private void ValidateXdebugExtraction(string extractPath)
        {
            var dllFiles = Directory.GetFiles(extractPath, "*.dll", SearchOption.AllDirectories);
            if (dllFiles.Length == 0)
                throw new InvalidOperationException("Xdebug validation failed: No DLL file found");
        }

        private void ValidateVCRuntimeExtraction(string extractPath)
        {
            // VC++ runtime validation is handled differently since files are distributed directly to target directories
            // The vcruntime directory itself is not created - files go directly to PHP, Apache, and MariaDB directories
            // Validation is performed by checking the target directories have the runtime files
        }

        private bool FileExists(string basePath, string fileName)
        {
            return Directory.GetFiles(basePath, fileName, SearchOption.AllDirectories).Length > 0;
        }

        private bool IsPathSafe(string destinationPath, string extractPath)
        {
            try
            {
                var fullDestination = Path.GetFullPath(destinationPath);
                var fullExtractPath = Path.GetFullPath(extractPath);
                
                // Ensure the destination is within the extract directory.
                return fullDestination.StartsWith(fullExtractPath, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        protected virtual void OnProgressReported(InstallationProgressEventArgs e)
        {
            ProgressReported?.Invoke(this, e);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}