using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PWAMP.Installer.Neo.Core.PackageDiscovery;
using PWAMP.Installer.Neo.Core.PackageOperations;
using PWAMP.Installer.Neo.Models;

namespace PWAMP.Installer.Neo.Core
{
    public class PackageManager : IDisposable
    {
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

            // Subscribe to download progress for logging
            _packageDownloadService.DownloadProgressReported += (sender, e) =>
            {
                var message = $"Downloading {e.PackageName}: {e.Status}";
                if (e.PercentComplete > 0)
                {
                    message += $" ({e.PercentComplete:F1}%)";
                }
                logger?.Report(message);
            };

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

            // Subscribe to extraction progress for logging
            _packageExtractionService.ExtractionProgressReported += (sender, e) =>
            {
                var message = $"Extracting {e.PackageName}: {e.CurrentOperation}";
                if (e.PercentComplete > 0)
                {
                    message += $" ({e.PercentComplete}%)";
                }
                logger?.Report(message);
            };

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