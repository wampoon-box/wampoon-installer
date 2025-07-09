using System;
using System.Threading;
using System.Threading.Tasks;
using PWAMP.Installer.Neo.Events;
using PWAMP.Installer.Neo.Models;

namespace PWAMP.Installer.Neo.Core.PackageOperations
{
    public class PackageDownloadService : IPackageDownloadService, IDisposable
    {
        private readonly PackageDownloader _packageDownloader;
        private bool _disposed = false;

        public event EventHandler<DownloadProgressEventArgs> DownloadProgressReported;

        public PackageDownloadService(PackageDownloader packageDownloader)
        {
            _packageDownloader = packageDownloader ?? throw new ArgumentNullException(nameof(packageDownloader));
            _packageDownloader.ProgressReported += OnDownloadProgressReported;
        }

        public async Task<string> DownloadPackageAsync(InstallablePackage package, string downloadDirectory, 
            IProgress<string> progress, CancellationToken cancellationToken = default)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            if (string.IsNullOrWhiteSpace(downloadDirectory))
            {
                throw new ArgumentException("Download directory cannot be null or empty", nameof(downloadDirectory));
            }

            progress?.Report($"Downloading {package.Name} v{package.Version}...");

            try
            {
                var downloadedPath = await _packageDownloader.DownloadPackageAsync(package, downloadDirectory, cancellationToken);
                progress?.Report($"✓ {package.Name} downloaded to: {downloadedPath}");
                return downloadedPath;
            }
            catch (Exception ex)
            {
                progress?.Report($"✗ Failed to download {package.Name}: {ex.Message}");
                throw;
            }
        }

        private void OnDownloadProgressReported(object sender, DownloadProgressEventArgs e)
        {
            DownloadProgressReported?.Invoke(this, e);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_packageDownloader != null)
                {
                    _packageDownloader.ProgressReported -= OnDownloadProgressReported;
                    _packageDownloader.Dispose();
                }
                _disposed = true;
            }
        }
    }
}