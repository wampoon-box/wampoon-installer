using System.Collections.Generic;
using Wampoon.Installer.Core.Paths.Configurations;

namespace Wampoon.Installer.Core.Paths
{
    /// <summary>
    /// Factory class for creating configured IPathResolver instances.    
    /// Centralizes the setup of all package configurations.
    /// </summary>
    public static class PathResolverFactory
    {
        /// <summary>
        /// Creates a fully configured IPathResolver instance with all supported packages.
        /// </summary>
        /// <param name="installPath">The base installation path for Wampoon</param>
        /// <returns>A configured IPathResolver instance</returns>
        public static IPathResolver CreatePathResolver(string installPath)
        {
            var packageConfigurations = new Dictionary<string, IPackagePathConfiguration>
            {
                { AppSettings.PackageNames.Apache, new ApachePathConfiguration() },
                { AppSettings.PackageNames.MariaDB, new MariaDBPathConfiguration() },
                { AppSettings.PackageNames.PHP, new PHPPathConfiguration() },
                { AppSettings.PackageNames.PhpMyAdmin, new PhpMyAdminPathConfiguration() }
            };

            return new PackagePathResolver(installPath, packageConfigurations);
        }

        /// <summary>
        /// Creates a path resolver with only the specified packages.
        /// Useful for testing or when only specific packages are needed.
        /// </summary>
        /// <param name="installPath">The base installation path for Wampoon</param>
        /// <param name="packageNames">The names of packages to include</param>
        /// <returns>A configured IPathResolver instance</returns>
        public static IPathResolver CreatePathResolver(string installPath, params string[] packageNames)
        {
            var allConfigurations = new Dictionary<string, IPackagePathConfiguration>
            {
                { AppSettings.PackageNames.Apache, new ApachePathConfiguration() },
                { AppSettings.PackageNames.MariaDB, new MariaDBPathConfiguration() },
                { AppSettings.PackageNames.PHP, new PHPPathConfiguration() },
                { AppSettings.PackageNames.PhpMyAdmin, new PhpMyAdminPathConfiguration() }
            };

            var selectedConfigurations = new Dictionary<string, IPackagePathConfiguration>();
            
            foreach (var packageName in packageNames)
            {
                if (allConfigurations.TryGetValue(packageName, out var config))
                {
                    selectedConfigurations[packageName] = config;
                }
            }

            return new PackagePathResolver(installPath, selectedConfigurations);
        }
    }
}