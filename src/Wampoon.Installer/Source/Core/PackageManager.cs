using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wampoon.Installer.Core.PackageDiscovery;
using Wampoon.Installer.Core.PackageOperations;
using Wampoon.Installer.Models;
using Wampoon.Installer.Events;
using Wampoon.Installer.Helpers;

namespace Wampoon.Installer.Core
{
    public class PackageManager : IDisposable
    {
        public event EventHandler<DownloadProgressEventArgs> DownloadProgressReported;
        public event EventHandler<InstallationProgressEventArgs> ExtractionProgressReported;
        public event EventHandler<InstallationCompletedEventArgs> PackageInstallationCompleted;

        private readonly IPackageDiscoveryService _packageDiscoveryService;
        private readonly IPackageDownloadService _packageDownloadService;
        private readonly IPackageExtractionService _packageExtractionService;
        private bool _disposed = false;

        public PackageManager()
        {
            var packageRepository = new PackageRepository();
            var packageDownloader = new PackageDownloader();
            var archiveExtractor = new ArchiveExtractor();
            
            _packageDiscoveryService = new PackageDiscoveryService(packageRepository);
            _packageDownloadService = new PackageDownloadService(packageDownloader);
            _packageExtractionService = new PackageExtractionService(archiveExtractor);

            // Subscribe to detailed progress events from services.
            _packageDownloadService.DownloadProgressReported += (sender, e) => DownloadProgressReported?.Invoke(this, e);
            _packageExtractionService.ExtractionProgressReported += (sender, e) => ExtractionProgressReported?.Invoke(this, e);
        }

        public async Task<InstallablePackage> GetPackageByNameAsync(string packageName)
        {
            return await _packageDiscoveryService.GetPackageByNameAsync(packageName);
        }

        public async Task<string> DownloadPackageAsync(string packageName, string downloadDirectory, 
            IProgress<string> logger, CancellationToken cancellationToken = default)
        {
            var package = await GetPackageByNameAsync(packageName);
            if (package == null)
            {
                throw new Exception($"Package '{packageName}' not found in repository");
            }

            // Progress is now handled centrally through event forwarding.

            return await _packageDownloadService.DownloadPackageAsync(package, downloadDirectory, logger, cancellationToken);
        }

        public async Task<string> ExtractPackageAsync(string packageName, string archivePath, string extractPath,
            IProgress<string> logger, CancellationToken cancellationToken = default)
        {
            var package = await GetPackageByNameAsync(packageName);
            if (package == null)
            {
                throw new Exception($"Package '{packageName}' not found in repository");
            }

            // Progress is now handled centrally through event forwarding.

            return await _packageExtractionService.ExtractPackageAsync(package, archivePath, extractPath, logger, cancellationToken);
        }

        public async Task<string> DownloadAndExtractPackageAsync(string packageName, string installPath,
            IProgress<string> logger, CancellationToken cancellationToken = default)
        {
            var package = await GetPackageByNameAsync(packageName);
            if (package == null)
            {
                throw new Exception($"Package '{packageName}' not found in repository");
            }

            // Create download directory.
            var downloadDir = Path.Combine(installPath, "downloads");
            Directory.CreateDirectory(downloadDir);

            // Download package.
            var downloadedPath = await DownloadPackageAsync(packageName, downloadDir, logger, cancellationToken);

            // Extract to appropriate folder based on package type.
            var extractPath = GetPackageExtractionPath(packageName, installPath);
            var extractedPath = await ExtractPackageAsync(packageName, downloadedPath, extractPath, logger, cancellationToken);
            
            // Special handling for control panel - move files from temp to install directory
            if (packageName.Equals(AppSettings.PackageNames.ControlPanel, StringComparison.OrdinalIgnoreCase))
            {
                await MoveControlPanelFilesToInstallDirectory(extractedPath, installPath, logger);
                extractedPath = installPath;
            }

            // Clean up downloaded archive to save space (but keep DLL files for Xdebug)
            try
            {
                if (!packageName.Equals(AppSettings.PackageNames.Xdebug, StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(downloadedPath);
                    logger?.Report($"Cleaned up download file: {Path.GetFileName(downloadedPath)}");
                }
                else
                {
                    logger?.Report($"Keeping Xdebug DLL file: {Path.GetFileName(downloadedPath)}");
                }
            }
            catch
            {
                // Ignore cleanup errors.
            }

            // Fire package installation completed event.
            PackageInstallationCompleted?.Invoke(this, new InstallationCompletedEventArgs
            {
                Success = true,
                PackageName = packageName,
                InstallPath = extractedPath,
                Message = $"Package {packageName} installed successfully"
            });

            return extractedPath;
        }

        private string GetPackageExtractionPath(string packageName, string installPath)
        {
            // Control panel extracts to temp folder first, then files will be moved to install directory
            if (packageName.Equals(AppSettings.PackageNames.ControlPanel, StringComparison.OrdinalIgnoreCase))
            {
                return Path.Combine(installPath, "temp", _packageDiscoveryService.GetPackageDirectoryName(packageName));
            }
            
            // Xdebug extracts to temp folder for processing by config helper
            if (packageName.Equals(AppSettings.PackageNames.Xdebug, StringComparison.OrdinalIgnoreCase))
            {
                return Path.Combine(installPath, "temp", _packageDiscoveryService.GetPackageDirectoryName(packageName));
            }
            
            // All other packages extract to apps folder
            return Path.Combine(installPath, "apps", _packageDiscoveryService.GetPackageDirectoryName(packageName));
        }

        private void MoveControlPanelFilesToInstallDirectory(string tempPath, string installPath, IProgress<string> logger)
        {
            try
            {
                logger?.Report("Moving control panel files to install directory...");
                
                // Move all files and directories from temp to install directory
                var files = Directory.GetFiles(tempPath, "*", SearchOption.AllDirectories);
                var directories = Directory.GetDirectories(tempPath, "*", SearchOption.AllDirectories);
                
                // Create directory structure first
                foreach (var dir in directories)
                {
                    var relativePath = dir.Substring(tempPath.Length + 1);
                    var targetDir = Path.Combine(installPath, relativePath);
                    Directory.CreateDirectory(targetDir);
                }
                
                // Move all files
                foreach (var file in files)
                {
                    var relativePath = file.Substring(tempPath.Length + 1);
                    var targetFile = Path.Combine(installPath, relativePath);
                    
                    // Ensure target directory exists
                    var targetDir = Path.GetDirectoryName(targetFile);
                    if (!Directory.Exists(targetDir))
                        Directory.CreateDirectory(targetDir);
                    
                    if (File.Exists(targetFile))
                        File.Delete(targetFile);
                    File.Move(file, targetFile);
                }
                
                // Clean up temp directory
                Directory.Delete(tempPath, true);
                
                // Also clean up the temp parent directory if it's empty
                var tempParent = Path.GetDirectoryName(tempPath);
                if (Directory.Exists(tempParent) && !Directory.EnumerateFileSystemEntries(tempParent).Any())
                {
                    Directory.Delete(tempParent);
                }
                
                logger?.Report("Control panel files moved successfully");
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogExceptionInfo(ex);
                logger?.Report($"Warning: Could not move all control panel files: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _packageDownloadService?.Dispose();
                _packageExtractionService?.Dispose();
                _disposed = true;
            }
        }
    }
}