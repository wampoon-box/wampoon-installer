using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PWAMP.Installer.Neo.Models;

namespace PWAMP.Installer.Neo.Core
{
    public class PackageManager : IDisposable
    {
        private readonly PackageRepository _packageRepository;
        private readonly PackageDownloader _packageDownloader;
        private readonly ArchiveExtractor _archiveExtractor;
        private bool _disposed = false;

        public PackageManager()
        {
            _packageRepository = new PackageRepository();
            _packageDownloader = new PackageDownloader();
            _archiveExtractor = new ArchiveExtractor();
        }

        public async Task<InstallablePackage> GetPackageByNameAsync(string packageName)
        {
            var packages = await _packageRepository.GetAvailablePackagesAsync();
            return packages.Find(p => p.Name.Equals(packageName, StringComparison.OrdinalIgnoreCase)) 
                ?? GetFallbackPackage(packageName);
        }

        public async Task<string> DownloadPackageAsync(string packageName, string downloadDirectory, 
            IProgress<string> logger, CancellationToken cancellationToken = default)
        {
            var package = await GetPackageByNameAsync(packageName);
            if (package == null)
            {
                throw new Exception($"Package '{packageName}' not found in repository");
            }

            logger?.Report($"Downloading {package.Name} v{package.Version}...");

            // Subscribe to download progress
            _packageDownloader.ProgressReported += (sender, e) =>
            {
                var message = $"Downloading {e.PackageName}: {e.Status}";
                if (e.PercentComplete > 0)
                {
                    message += $" ({e.PercentComplete:F1}%)";
                }
                logger?.Report(message);
            };

            try
            {
                var downloadedPath = await _packageDownloader.DownloadPackageAsync(package, downloadDirectory, cancellationToken);
                logger?.Report($"✓ {package.Name} downloaded to: {downloadedPath}");
                return downloadedPath;
            }
            catch (Exception ex)
            {
                logger?.Report($"✗ Failed to download {package.Name}: {ex.Message}");
                throw;
            }
        }

        public async Task<string> ExtractPackageAsync(string packageName, string archivePath, string extractPath,
            IProgress<string> logger, CancellationToken cancellationToken = default)
        {
            var package = await GetPackageByNameAsync(packageName);
            if (package == null)
            {
                throw new Exception($"Package '{packageName}' not found in repository");
            }

            logger?.Report($"Extracting {package.Name}...");

            // Subscribe to extraction progress
            _archiveExtractor.ProgressReported += (sender, e) =>
            {
                var message = $"Extracting {e.PackageName}: {e.CurrentOperation}";
                if (e.PercentComplete > 0)
                {
                    message += $" ({e.PercentComplete}%)";
                }
                logger?.Report(message);
            };

            try
            {
                var extractedPath = await _archiveExtractor.ExtractPackageAsync(package, archivePath, extractPath, cancellationToken);
                logger?.Report($"✓ {package.Name} extracted to: {extractedPath}");
                return extractedPath;
            }
            catch (Exception ex)
            {
                logger?.Report($"✗ Failed to extract {package.Name}: {ex.Message}");
                throw;
            }
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
            var extractPath = Path.Combine(installPath, "apps", GetPackageDirectoryName(packageName));
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

        private InstallablePackage GetFallbackPackage(string packageName)
        {
            // Provide fallback package definitions for common packages
            switch (packageName.ToLower())
            {
                case "apache":
                    return new InstallablePackage
                    {
                        PackageID = PackageType.Apache,
                        Name = "Apache HTTP Server",
                        Version = new Version(2, 4, 62),
                        DownloadUrl = new Uri("https://www.apachelounge.com/download/VS17/binaries/httpd-2.4.63-250207-win64-VS17.zip"),
                        Type = PackageType.Apache,
                        ServerName = "Apache",
                        EstimatedSize = 15 * 1024 * 1024,
                        Description = "Apache HTTP Server",
                        InstallPath = "apps/apache",
                        ArchiveFormat = "zip"
                    };
                case "mariadb":
                    return new InstallablePackage
                    {
                        PackageID = PackageType.MariaDB,
                        Name = "MariaDB Database",
                        Version = new Version(11, 8, 2),
                        DownloadUrl = new Uri("https://mirror.us.mirhosting.net/mariadb/mariadb-11.8.2/winx64-packages/mariadb-11.8.2-winx64.zip"),
                        Type = PackageType.MariaDB,
                        ServerName = "MariaDB",
                        EstimatedSize = 180 * 1024 * 1024,
                        Description = "MariaDB Database Server",
                        InstallPath = "apps/mariadb",
                        ArchiveFormat = "zip"
                    };
                case "php":
                    return new InstallablePackage
                    {
                        PackageID = PackageType.PHP,
                        Name = "PHP Scripting Language",
                        Version = new Version(8, 4, 9),
                        DownloadUrl = new Uri("https://windows.php.net/downloads/releases/archives/php-8.4.9-Win32-vs17-x64.zip"),                        
                        Type = PackageType.PHP,
                        ServerName = "PHP",
                        EstimatedSize = 35 * 1024 * 1024,
                        Description = "PHP Scripting Language",
                        InstallPath = "apps/php",
                        ArchiveFormat = "zip"
                    };
                case "phpmyadmin":
                    return new InstallablePackage
                    {
                        PackageID = PackageType.PhpMyAdmin,
                        Name = "phpMyAdmin Database Manager",
                        Version = new Version(5, 2, 2),
                        DownloadUrl = new Uri("https://files.phpmyadmin.net/phpMyAdmin/5.2.2/phpMyAdmin-5.2.2-all-languages.zip"),
                        Type = PackageType.PhpMyAdmin,
                        ServerName = "phpMyAdmin",
                        EstimatedSize = 15 * 1024 * 1024,
                        Description = "phpMyAdmin Database Manager",
                        InstallPath = "apps/phpmyadmin",
                        ArchiveFormat = "zip"
                    };
                default:
                    return null;
            }
        }

        private string GetPackageDirectoryName(string packageName)
        {
            switch (packageName.ToLower())
            {
                case "apache":
                    return "apache";
                case "mariadb":
                    return "mariadb";
                case "mysql":
                    return "mariadb"; // Map mysql to mariadb
                case "php":
                    return "php";
                case "phpmyadmin":
                    return "phpmyadmin";
                default:
                    return packageName.ToLower();
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _packageRepository?.Dispose();
                _packageDownloader?.Dispose();
                _archiveExtractor?.Dispose();
                _disposed = true;
            }
        }
    }
}