using System;
using System.Collections.Generic;
using System.IO;
using Wampoon.Installer.Helpers.Common;

namespace Wampoon.Installer.Core.Paths
{
    /// <summary>
    /// Concrete implementation of IPathResolver that handles path resolution for Wampoon packages.    
    /// </summary>
    public class PackagePathResolver : IPathResolver
    {
        private readonly string _installPath;
        private readonly Dictionary<string, IPackagePathConfiguration> _packageConfigurations;

        /// <summary>
        /// Initializes a new instance of the PackagePathResolver.
        /// </summary>
        /// <param name="installPath">The base installation path for Wampoon</param>
        /// <param name="packageConfigurations">Dictionary of package configurations by package name</param>
        public PackagePathResolver(string installPath, Dictionary<string, IPackagePathConfiguration> packageConfigurations)
        {
            _installPath = installPath ?? throw new ArgumentNullException(nameof(installPath));
            _packageConfigurations = packageConfigurations ?? throw new ArgumentNullException(nameof(packageConfigurations));
        }

        public string GetPackageDirectory(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
                throw new ArgumentException("Package name cannot be null or empty", nameof(packageName));

            return Path.Combine(_installPath, "apps", packageName);
        }

        public string GetBinaryPath(string packageName, string binaryName)
        {
            if (string.IsNullOrEmpty(packageName))
                throw new ArgumentException("Package name cannot be null or empty", nameof(packageName));
            if (string.IsNullOrEmpty(binaryName))
                throw new ArgumentException("Binary name cannot be null or empty", nameof(binaryName));

            var packageConfig = GetPackageConfiguration(packageName);
            var packageDir = GetPackageDirectory(packageName);

            // Try to find the binary in the configured locations
            if (packageConfig.BinaryFiles.TryGetValue(binaryName, out var binaryPath))
            {
                var fullPath = Path.Combine(packageDir, binaryPath);
                if (File.Exists(fullPath))
                    return fullPath;
            }

            // Try alternative paths if configured
            if (packageConfig.AlternativeBinaryPaths.TryGetValue(binaryName, out var alternativePaths))
            {
                foreach (var alternativePath in alternativePaths)
                {
                    var fullPath = Path.Combine(packageDir, alternativePath);
                    if (File.Exists(fullPath))
                        return fullPath;
                }
            }

            // Fallback to default binary directory
            var defaultPath = Path.Combine(packageDir, packageConfig.BinaryDirectory, binaryName);
            return defaultPath;
        }

        public string GetConfigPath(string packageName, string configName)
        {
            if (string.IsNullOrEmpty(packageName))
                throw new ArgumentException("Package name cannot be null or empty", nameof(packageName));
            if (string.IsNullOrEmpty(configName))
                throw new ArgumentException("Config name cannot be null or empty", nameof(configName));

            var packageConfig = GetPackageConfiguration(packageName);
            var packageDir = GetPackageDirectory(packageName);

            // Try to find the config in the configured locations
            if (packageConfig.ConfigFiles.TryGetValue(configName, out var configPath))
            {
                return Path.Combine(packageDir, configPath);
            }

            // Fallback to default config directory
            var defaultPath = Path.Combine(packageDir, packageConfig.ConfigDirectory, configName);
            return defaultPath;
        }

        public string GetSubdirectoryPath(string packageName, string subdirectory)
        {
            if (string.IsNullOrEmpty(packageName))
                throw new ArgumentException("Package name cannot be null or empty", nameof(packageName));
            if (string.IsNullOrEmpty(subdirectory))
                throw new ArgumentException("Subdirectory cannot be null or empty", nameof(subdirectory));

            var packageConfig = GetPackageConfiguration(packageName);
            var packageDir = GetPackageDirectory(packageName);

            // Try to find the subdirectory in the configured locations
            if (packageConfig.Subdirectories.TryGetValue(subdirectory, out var subdirectoryPath))
            {
                return Path.Combine(packageDir, subdirectoryPath);
            }

            // Fallback to direct subdirectory
            return Path.Combine(packageDir, subdirectory);
        }

        public string GetSubdirectoryFilePath(string packageName, string subdirectory, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

            var subdirectoryPath = GetSubdirectoryPath(packageName, subdirectory);
            return Path.Combine(subdirectoryPath, fileName);
        }

        public string GetAppsDirectory()
        {
            return Path.Combine(_installPath, "apps");
        }

        public string GetHtdocsDirectory()
        {
            return Path.Combine(_installPath, "htdocs");
        }

        /// <summary>
        /// Gets the package configuration for the specified package name.
        /// </summary>
        /// <param name="packageName">The name of the package</param>
        /// <returns>The package configuration</returns>
        /// <exception cref="InvalidOperationException">Thrown when package configuration is not found</exception>
        private IPackagePathConfiguration GetPackageConfiguration(string packageName)
        {
            if (!_packageConfigurations.TryGetValue(packageName, out var config))
            {
                throw new InvalidOperationException($"Package configuration not found for package: {packageName}");
            }

            return config;
        }
    }
}