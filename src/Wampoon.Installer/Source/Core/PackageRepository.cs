using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reflection;
using Wampoon.Installer.Models;
using Wampoon.Installer.Helpers.Logging;
using Wampoon.Installer.Helpers;
using Newtonsoft.Json;

namespace Wampoon.Installer.Core
{
    public class PackageRepository : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private List<InstallablePackage> _packages;
        private bool _disposed = false;

        public PackageRepository()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = InstallerConstants.HttpShortTimeout;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", AppConstants.USER_AGENT);
            _logger = LoggerFactory.Default;
        }

        public Task<List<InstallablePackage>> GetAvailablePackagesAsync()
        {
            if (_packages != null)
                return Task.FromResult(_packages);

            // Try local file first for faster startup.
            _packages = LoadPackagesFromLocalFile();
            
            // If local file didn't have valid packages, try remote manifest.
            if (_packages == null || !_packages.Any())
            {
                // TODO: Re-enable remote manifest loading when web server is ready
                // try
                // {
                //     _packages = await LoadPackagesFromManifestAsync();
                // }
                // catch
                // {
                //     // If both fail, use fallback.
                //     _packages = GetFallbackPackages();
                // }
                
                // Temporarily use fallback packages directly
                _packages = GetFallbackPackages();
            }

            return Task.FromResult(_packages);
        }

        private async Task<List<InstallablePackage>> LoadPackagesFromManifestAsync()
        {
            var manifestUrl = GetConfiguredManifestUrl();
            
            if (string.IsNullOrEmpty(manifestUrl) || !IsUrlAllowed(manifestUrl))
            {
                return LoadPackagesFromLocalFile();
            }
            
            try
            {
                var json = await _httpClient.GetStringAsync(manifestUrl);
                var packages = JsonConvert.DeserializeObject<List<InstallablePackage>>(json);
                return packages != null ? MergeWithMetadata(packages) : LoadPackagesFromLocalFile();
            }
            catch
            {
                return LoadPackagesFromLocalFile();
            }
        }

        private string GetConfiguredManifestUrl()
        {
            // Try to get from config file first, then fallback to default.
            try
            {
                var configPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory, InstallerConstants.ConfigFileName);
                if (File.Exists(configPath))
                {
                    var config = JsonConvert.DeserializeObject<dynamic>(File.ReadAllText(configPath));
                    return config?.manifestUrl?.ToString();
                }
            }
            catch
            {
                // Ignore config errors and use default.
            }
            
            return "https://raw.githubusercontent.com/your-org/pwamp-packages/main/packages.json";
        }

        private bool IsUrlAllowed(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return false;
                
            if (uri.Scheme != "https")
                return false;
                
            return InstallerConstants.AllowedManifestDomains.Any(domain => uri.Host.Equals(domain, StringComparison.OrdinalIgnoreCase));
        }

        private List<InstallablePackage> LoadPackagesFromLocalFile()
        {
            try
            {
                var appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory;
                var packagesPath = Path.Combine(appDir, "Data", InstallerConstants.PackagesFileName);
                
                if (File.Exists(packagesPath))
                {
                    var json = File.ReadAllText(packagesPath);
                    var packages = JsonConvert.DeserializeObject<List<InstallablePackage>>(json);
                    if (packages != null && packages.Any())
                    {
                        return MergeWithMetadata(packages);
                    }
                    else
                    {
                        _logger.LogWarning("Local packages file is empty or contains invalid data. Falling back to embedded package information.");
                    }
                }
                else
                {
                    _logger.LogWarning("Local packages file not found. Ensure that the packagesInfo.json file exists in Data/ folder. Falling back to embedded package information.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading packages from local file. Falling back to embedded package information.", ex);
            }

            // Fallback to minimal package list if JSON loading fails.
            return GetFallbackPackages();
        }


        private List<InstallablePackage> MergeWithMetadata(List<InstallablePackage> packages)
        {
            foreach (var package in packages)
            {
                var metadata = PackageMetadata.GetMetadata(package.PackageID);
                if (metadata != null)
                {
                    // Merge hardcoded metadata with JSON data.
                    package.Type = metadata.Type;
                    package.ServerName = metadata.ServerName;
                    package.EstimatedSize = metadata.EstimatedSize;
                    package.Description = metadata.Description;
                    package.InstallPath = metadata.InstallPath;
                    package.ArchiveFormat = metadata.ArchiveFormat;
                    package.Dependencies = metadata.Dependencies;
                    
                    // If no checksum URL but we have a direct checksum, clear the URL.
                    if (!string.IsNullOrEmpty(package.Checksum))
                    {
                        package.ChecksumUrl = null;
                    }
                }
            }
            
            return packages;
        }

        private List<InstallablePackage> GetFallbackPackages()
        {
            // Fallback packages matching the latest versions from packages.json.
            return new List<InstallablePackage>
            {
                new InstallablePackage
                {
                    PackageID = PackageType.Apache,
                    Name = "Apache HTTP Server",
                    Version = new Version(2, 4, 63),
                    DownloadUrl = new Uri("https://www.apachelounge.com/download/VS17/binaries/httpd-2.4.63-250207-win64-VS17.zip"),
                    Type = PackageType.Apache,
                    ServerName = "Apache",
                    EstimatedSize = 15 * 1024 * 1024,
                    Description = "Apache HTTP Server - The world's most widely used web server software.",
                    InstallPath = "apps/apache",
                    ArchiveFormat = "zip",
                    Dependencies = new List<PackageType>()
                },
                new InstallablePackage
                {
                    PackageID = PackageType.MariaDB,
                    Name = "MariaDB Server",
                    Version = new Version(11, 8, 2),
                    DownloadUrl = new Uri("https://mirror.us.mirhosting.net/mariadb/mariadb-11.8.2/winx64-packages/mariadb-11.8.2-winx64.zip"),
                    Type = PackageType.MariaDB,
                    ServerName = "MariaDB",
                    EstimatedSize = 180 * 1024 * 1024,
                    Description = "MariaDB Server - Popular MySQL-compatible database server.",
                    InstallPath = "apps/mariadb",
                    ArchiveFormat = "zip",
                    Dependencies = new List<PackageType>()
                },
                new InstallablePackage
                {
                    PackageID = PackageType.PHP,
                    Name = "PHP Runtime",
                    Version = new Version(8, 4, 10),
                    DownloadUrl = new Uri("https://windows.php.net/downloads/releases/php-8.4.10-Win32-vs17-x64.zip"),
                    Type = PackageType.PHP,
                    ServerName = "PHP",
                    EstimatedSize = 35 * 1024 * 1024,
                    Description = "PHP - Server-side scripting language designed for web development.",
                    InstallPath = "apps/php",
                    ArchiveFormat = "zip",
                    Dependencies = new List<PackageType> { PackageType.Apache }
                },
                new InstallablePackage
                {
                    PackageID = PackageType.PhpMyAdmin,
                    Name = "phpMyAdmin",
                    Version = new Version(5, 2, 2),
                    DownloadUrl = new Uri("https://files.phpmyadmin.net/phpMyAdmin/5.2.2/phpMyAdmin-5.2.2-all-languages.zip"),
                    Type = PackageType.PhpMyAdmin,
                    ServerName = "phpMyAdmin",
                    EstimatedSize = 15 * 1024 * 1024,
                    Description = "phpMyAdmin - Web-based database administration tool for MySQL and MariaDB.",
                    InstallPath = "apps/phpmyadmin",
                    ArchiveFormat = "zip",
                    Dependencies = new List<PackageType> { PackageType.PHP, PackageType.MariaDB }
                },
                new InstallablePackage
                {
                    PackageID = PackageType.WampoonDashboard,
                    Name = "wampoon-dashboard",
                    Version = new Version(0, 1, 1),
                    DownloadUrl = new Uri("https://github.com/wampoon-box/wampoon-dashboard/releases/latest/download/wampoon-dashboard.zip"),
                    Type = PackageType.WampoonDashboard,
                    ServerName = "Wampoon Dashboard",
                    EstimatedSize = 5 * 1024 * 1024,
                    Description = "Wampoon Dashboard - Web-based management interface for Wampoon.",
                    InstallPath = "apps/wampoon-dashboard",
                    ArchiveFormat = "zip",
                    Dependencies = new List<PackageType>()
                },
                new InstallablePackage
                {
                    PackageID = PackageType.WampoonControlPanel,
                    Name = "Wampoon Control Panel",
                    Version = new Version(0, 1, 0),
                    DownloadUrl = new Uri("https://github.com/wampoon-box/wampoon-control/releases/latest/download/wampoon-control.zip"),
                    Type = PackageType.WampoonControlPanel,
                    ServerName = "Wampoon Control Panel",
                    EstimatedSize = 3 * 1024 * 1024,
                    Description = "Wampoon Control Panel - Desktop control panel for managing Wampoon services.",
                    InstallPath = "wampoon-control-panel",
                    ArchiveFormat = "zip",
                    Dependencies = new List<PackageType>()
                }
            };
        }

        public List<InstallablePackage> GetPackagesByType(PackageType type)
        {
            return _packages?.Where(p => p.Type == type).ToList() ?? new List<InstallablePackage>();
        }

        public InstallablePackage GetPackageByName(string name)
        {
            return _packages?.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public InstallablePackage GetPackageByType(PackageType packageType)
        {
            return _packages?.FirstOrDefault(p => p.Type == packageType);
        }

        public List<InstallablePackage> ResolveDependencies(List<InstallablePackage> selectedPackages)
        {
            var result = new List<InstallablePackage>(selectedPackages);
            var toProcess = new Queue<InstallablePackage>(selectedPackages);

            while (toProcess.Count > 0)
            {
                var package = toProcess.Dequeue();
                
                foreach (var dependencyType in package.Dependencies)
                {
                    var dependency = GetPackageByType(dependencyType);
                    if (dependency != null && !result.Any(p => p.Type == dependency.Type))
                    {
                        result.Add(dependency);
                        toProcess.Enqueue(dependency);
                    }
                }
            }

            return result.OrderBy(p => p.Dependencies.Count).ToList();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }
    }
}