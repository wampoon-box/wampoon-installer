using System;
using System.Linq;
using System.Threading.Tasks;
using PWAMP.Installer.Neo.Models;

namespace PWAMP.Installer.Neo.Core.PackageDiscovery
{
    public class PackageDiscoveryService : IPackageDiscoveryService
    {
        private readonly PackageRepository _packageRepository;

        public PackageDiscoveryService(PackageRepository packageRepository)
        {
            _packageRepository = packageRepository ?? throw new ArgumentNullException(nameof(packageRepository));
        }

        public async Task<InstallablePackage> GetPackageByNameAsync(string packageName)
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                return null;
            }

            var packages = await _packageRepository.GetAvailablePackagesAsync();
            return packages.Find(p => p.Name.Equals(packageName, StringComparison.OrdinalIgnoreCase)) 
                ?? GetFallbackPackage(packageName);
        }

        public async Task<InstallablePackage[]> GetAvailablePackagesAsync()
        {
            var packages = await _packageRepository.GetAvailablePackagesAsync();
            return packages.ToArray();
        }

        public bool IsPackageSupported(string packageName)
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                return false;
            }

            switch (packageName.ToLower())
            {
                case PackageNames.Apache:
                case PackageNames.MariaDB:
                case PackageNames.PHP:
                case PackageNames.PhpMyAdmin:
                    return true;
                default:
                    return false;
            }
        }

        public string GetPackageDirectoryName(string packageName)
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                return string.Empty;
            }

            switch (packageName.ToLower())
            {
                case PackageNames.Apache:
                    return "apache";
                case PackageNames.MariaDB:
                    return "mariadb";
                case "mysql":
                    return "mariadb"; // Map mysql to mariadb for legacy compatibility
                case PackageNames.PHP:
                    return "php";
                case PackageNames.PhpMyAdmin:
                    return "phpmyadmin";
                default:
                    return packageName.ToLower();
            }
        }

        private InstallablePackage GetFallbackPackage(string packageName)
        {
            switch (packageName.ToLower())
            {
                case PackageNames.Apache:
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
                case PackageNames.MariaDB:
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
                case PackageNames.PHP:
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
                case PackageNames.PhpMyAdmin:
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
    }
}