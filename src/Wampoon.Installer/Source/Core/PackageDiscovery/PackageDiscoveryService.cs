using System;
using System.Linq;
using System.Threading.Tasks;
using Wampoon.Installer.Models;

namespace Wampoon.Installer.Core.PackageDiscovery
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

            // Map PackageNames constants to PackageType enum.
            var packageType = GetPackageTypeFromName(packageName);
            if (packageType == null)
            {
                return null;
            }

            var packages = await _packageRepository.GetAvailablePackagesAsync();
            return packages.Find(p => p.PackageID == packageType.Value);
        }

        private PackageType? GetPackageTypeFromName(string packageName)
        {
            switch (packageName.ToLower())
            {
                case AppSettings.PackageNames.Apache:
                    return PackageType.Apache;
                case AppSettings.PackageNames.MariaDB:
                    return PackageType.MariaDB;
                case AppSettings.PackageNames.PHP:
                    return PackageType.PHP;
                case AppSettings.PackageNames.PhpMyAdmin:
                    return PackageType.PhpMyAdmin;
                default:
                    return null;
            }
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
                case AppSettings.PackageNames.Apache:
                case AppSettings.PackageNames.MariaDB:
                case AppSettings.PackageNames.PHP:
                case AppSettings.PackageNames.PhpMyAdmin:
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
                case AppSettings.PackageNames.Apache:
                    return "apache";
                case AppSettings.PackageNames.MariaDB:
                    return "mariadb";
                case "mysql":
                    return "mariadb"; // Map mysql to mariadb for legacy compatibility.
                case AppSettings.PackageNames.PHP:
                    return "php";
                case AppSettings.PackageNames.PhpMyAdmin:
                    return "phpmyadmin";
                default:
                    return packageName.ToLower();
            }
        }

    }
}