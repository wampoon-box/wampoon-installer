using System;
using System.Threading;
using System.Threading.Tasks;
using PWAMP.Installer.Neo.Events;
using PWAMP.Installer.Neo.Models;

namespace PWAMP.Installer.Neo.Core.PackageOperations
{
    public class PackageExtractionService : IPackageExtractionService, IDisposable
    {
        private readonly ArchiveExtractor _archiveExtractor;
        private bool _disposed = false;

        public event EventHandler<InstallationProgressEventArgs> ExtractionProgressReported;

        public PackageExtractionService(ArchiveExtractor archiveExtractor)
        {
            _archiveExtractor = archiveExtractor ?? throw new ArgumentNullException(nameof(archiveExtractor));
            _archiveExtractor.ProgressReported += OnExtractionProgressReported;
        }

        public async Task<string> ExtractPackageAsync(InstallablePackage package, string archivePath, string extractPath,
            IProgress<string> progress, CancellationToken cancellationToken = default)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            if (string.IsNullOrWhiteSpace(archivePath))
            {
                throw new ArgumentException("Archive path cannot be null or empty", nameof(archivePath));
            }

            if (string.IsNullOrWhiteSpace(extractPath))
            {
                throw new ArgumentException("Extract path cannot be null or empty", nameof(extractPath));
            }

            progress?.Report($"Extracting {package.Name}...");

            try
            {
                var extractedPath = await _archiveExtractor.ExtractPackageAsync(package, archivePath, extractPath, cancellationToken);
                progress?.Report($"✓ {package.Name} extracted to: {extractedPath}");
                return extractedPath;
            }
            catch (Exception ex)
            {
                progress?.Report($"✗ Failed to extract {package.Name}: {ex.Message}");
                throw;
            }
        }

        private void OnExtractionProgressReported(object sender, InstallationProgressEventArgs e)
        {
            ExtractionProgressReported?.Invoke(this, e);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_archiveExtractor != null)
                {
                    _archiveExtractor.ProgressReported -= OnExtractionProgressReported;
                    _archiveExtractor.Dispose();
                }
                _disposed = true;
            }
        }
    }
}