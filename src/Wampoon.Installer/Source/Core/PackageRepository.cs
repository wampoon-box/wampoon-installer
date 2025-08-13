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

        public async Task<List<InstallablePackage>> GetAvailablePackagesAsync()
        {
            return await GetAvailablePackagesAsync(PackageSource.Auto);
        }

        public async Task<List<InstallablePackage>> GetAvailablePackagesAsync(PackageSource source)
        {
            // Reset packages if we're changing sources
            if (_packages != null && source != PackageSource.Auto)
            {
                _packages = null;
            }

            if (_packages != null)
                return _packages;

            switch (source)
            {
                case PackageSource.LocalOnly:
                    _packages = LoadPackagesFromLocalFile();
                    if (_packages == null || !_packages.Any())
                    {
                        throw new InvalidOperationException("Unable to load packages from local file. Please ensure the packagesInfo.json file exists and contains valid package data, or switch to web-based package source.");
                    }
                    break;

                case PackageSource.WebOnly:
                    try
                    {
                        _packages = await LoadPackagesFromManifestAsync();
                        if (_packages == null || !_packages.Any())
                        {
                            throw new InvalidOperationException("Unable to load packages from web manifest. Please check your internet connection and try again.");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("Failed to load web manifest. Please check your internet connection and try again.", ex);
                    }
                    break;


                case PackageSource.Auto:
                default:
                    // Try remote manifest first for latest packages.
                    try
                    {
                        _packages = await LoadPackagesFromManifestAsync();
                        
                        // If remote failed or returned empty, try local file as backup.
                        if (_packages == null || !_packages.Any())
                        {
                            _packages = LoadPackagesFromLocalFile();
                        }
                    }
                    catch
                    {
                        // If remote fails, try local file as backup.
                        _packages = LoadPackagesFromLocalFile();
                    }
                    
                    // If both failed, notify the user instead of using outdated embedded packages.
                    if (_packages == null || !_packages.Any())
                    {
                        throw new InvalidOperationException("Unable to load packages from both web manifest and local file. Please check your internet connection or ensure the local packagesInfo.json file exists and contains valid package data.");
                    }
                    break;
            }

            return _packages;
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
                return packages != null ? MergeWithMetadata(packages) : null;
            }
            catch
            {
                return null;
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
            
            return "https://raw.githubusercontent.com/wampoon-box/wampoon-packages/refs/heads/main/packagesInfo.json";
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
            var appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory;
            var packagesPath = Path.Combine(appDir, "Data", InstallerConstants.PackagesFileName);
            
            try
            {
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
                        _logger.LogWarning("Local packages file is empty or contains invalid data.");
                    }
                }
                else
                {
                    _logger.LogWarning("Local packages file not found. Ensure that the packagesInfo.json file exists in Data/ folder.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading packages from local file. Path: {packagesPath}. Error: {ex.Message}", ex);
            }

            return null;
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
                },
                new InstallablePackage
                {
                    PackageID = PackageType.Xdebug,
                    Name = "Xdebug",
                    Version = new Version(3, 4, 5),
                    DownloadUrl = new Uri("https://xdebug.org/files/php_xdebug-3.4.5-8.4-ts-vs17-x86_64.dll"),
                    Type = PackageType.Xdebug,
                    ServerName = "Xdebug",
                    EstimatedSize = 1 * 1024 * 1024,
                    Description = "Xdebug - PHP debugging and profiling extension.",
                    InstallPath = "temp/xdebug",
                    ArchiveFormat = "dll",
                    Dependencies = new List<PackageType> { PackageType.PHP }
                },
                new InstallablePackage
                {
                    PackageID = PackageType.Composer,
                    Name = "Composer",
                    Version = new Version(2, 8, 10),
                    DownloadUrl = new Uri("https://getcomposer.org/download/2.8.10/composer.phar"),
                    Type = PackageType.Composer,
                    ServerName = "Composer",
                    EstimatedSize = 2 * 1024 * 1024,
                    Description = "Composer - PHP dependency manager.",
                    InstallPath = "apps/composer",
                    ArchiveFormat = "phar",
                    Dependencies = new List<PackageType> { PackageType.PHP }
                },
                new InstallablePackage
                {
                    PackageID = PackageType.VCRuntime,
                    Name = "Microsoft Visual C++ Runtime",
                    Version = new Version(14, 0, 0),
                    DownloadUrl = new Uri("https://github.com/wampoon-box/wampoon-packages/raw/refs/heads/main/vc_runtime.zip"),
                    Type = PackageType.VCRuntime,
                    ServerName = "VC++ Runtime",
                    EstimatedSize = 10 * 1024 * 1024,
                    Description = "Microsoft Visual C++ Runtime redistributables for PHP, Apache, and MariaDB.",
                    InstallPath = "temp/vcruntime",
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