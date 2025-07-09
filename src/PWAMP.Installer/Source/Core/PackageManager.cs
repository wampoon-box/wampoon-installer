using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PWAMP.Installer.Neo.Core.PackageDiscovery;
using PWAMP.Installer.Neo.Core.PackageOperations;
using PWAMP.Installer.Neo.Models;
using PWAMP.Installer.Neo.Events;

namespace PWAMP.Installer.Neo.Core
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

            // Subscribe to detailed progress events from services
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

            // Progress is now handled centrally through event forwarding

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

            // Progress is now handled centrally through event forwarding

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

            // Create download directory
            var downloadDir = Path.Combine(installPath, "downloads");
            Directory.CreateDirectory(downloadDir);

            // Download package
            var downloadedPath = await DownloadPackageAsync(packageName, downloadDir, logger, cancellationToken);

            // Extract to apps folder
            var extractPath = Path.Combine(installPath, "apps", _packageDiscoveryService.GetPackageDirectoryName(packageName));
            var extractedPath = await ExtractPackageAsync(packageName, downloadedPath, extractPath, logger, cancellationToken);

            // Clean up downloaded archive to save space
            try
            {
                File.Delete(downloadedPath);
                logger?.Report($"Cleaned up download file: {Path.GetFileName(downloadedPath)}");
            }
            catch
            {
                // Ignore cleanup errors
            }

            // Fire package installation completed event
            PackageInstallationCompleted?.Invoke(this, new InstallationCompletedEventArgs
            {
                Success = true,
                PackageName = packageName,
                InstallPath = extractedPath,
                Message = $"Package {packageName} installed successfully"
            });

            return extractedPath;
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