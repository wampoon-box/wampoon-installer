using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using PWAMP.Installer.Events;
using PWAMP.Installer.Models;

namespace PWAMP.Installer.Core
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

                    await Task.Run(() => Directory.Delete(extractPath, true), cancellationToken);
                }

                Directory.CreateDirectory(extractPath);

                await Task.Run(async () =>
                {
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
                            var progress = 10 + (extractedEntries * 80 / totalEntries);

                            if (extractedEntries % Math.Max(1, totalEntries / InstallerConstants.ProgressReportFrequency) == 0)
                            {
                                OnProgressReported(new InstallationProgressEventArgs
                                {
                                    PackageName = package.Name,
                                    CurrentOperation = $"Extracted {extractedEntries} of {totalEntries} files...",
                                    Stage = InstallationStage.Extracting,
                                    PercentComplete = progress,
                                    TotalSteps = totalEntries,
                                    CompletedSteps = extractedEntries
                                });
                            }
                        }
                    }
                }, cancellationToken);

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

        private async Task ValidateExtraction(InstallablePackage package, string extractPath, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
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
                }
            }, cancellationToken);
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
                
                // Ensure the destination is within the extract directory
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