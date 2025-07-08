using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Reflection;
using PWAMP.Installer.Neo.Models;
using Newtonsoft.Json;

namespace PWAMP.Installer.Neo.Core
{
    public class PackageRepository : IDisposable
    {
        private readonly HttpClient _httpClient;
        private List<InstallablePackage> _packages;
        private bool _disposed = false;

        public PackageRepository()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = InstallerConstants.HttpShortTimeout;
        }

        public async Task<List<InstallablePackage>> GetAvailablePackagesAsync()
        {
            if (_packages != null)
                return _packages;

            try
            {
                _packages = await LoadPackagesFromManifestAsync();
            }
            catch
            {
                _packages = LoadPackagesFromLocalFile();
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
                return packages != null ? MergeWithMetadata(packages) : LoadPackagesFromLocalFile();
            }
            catch
            {
                return LoadPackagesFromLocalFile();
            }
        }

        private string GetConfiguredManifestUrl()
        {
            // Try to get from config file first, then fallback to default
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
                // Ignore config errors and use default
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
                // Try multiple locations for the packages.json file.
                var possiblePaths = GetPossiblePackageFilePaths();
                
                foreach (var path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        var json = File.ReadAllText(path);
                        var packages = JsonConvert.DeserializeObject<List<InstallablePackage>>(json);
                        if (packages != null && packages.Any())
                        {
                            return MergeWithMetadata(packages);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error if logging is available.
                System.Diagnostics.Debug.WriteLine($"Error loading packages from local file: {ex.Message}");
            }

            // Fallback to minimal package list if JSON loading fails.
            return GetFallbackPackages();
        }

        private string[] GetPossiblePackageFilePaths()
        {
            var appDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory;
            var currentDir = Environment.CurrentDirectory;
            
            return new[]
            {
                Path.Combine(appDir, "Data", InstallerConstants.PackagesFileName),
                Path.Combine(appDir, InstallerConstants.PackagesFileName),
                Path.Combine(currentDir, "Data", InstallerConstants.PackagesFileName),
                Path.Combine(currentDir, InstallerConstants.PackagesFileName)
            };
        }

        private List<InstallablePackage> MergeWithMetadata(List<InstallablePackage> packages)
        {
            foreach (var package in packages)
            {
                var metadata = PackageMetadata.GetMetadata(package.PackageID);
                if (metadata != null)
                {
                    // Merge hardcoded metadata with JSON data
                    package.Type = metadata.Type;
                    package.ServerName = metadata.ServerName;
                    package.EstimatedSize = metadata.EstimatedSize;
                    package.Description = metadata.Description;
                    package.InstallPath = metadata.InstallPath;
                    package.ArchiveFormat = metadata.ArchiveFormat;
                    package.Dependencies = metadata.Dependencies;
                    
                    // If no checksum URL but we have a direct checksum, clear the URL
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
            // Minimal fallback list with only essential packages.
            return new List<InstallablePackage>
            {
                new InstallablePackage
                {
                    PackageID = PackageType.Apache,
                    Name = "Apache HTTP Server",
                    Version = new Version(2, 4, 58),
                    DownloadUrl = new Uri("https://www.apachelounge.com/download/VS17/binaries/httpd-2.4.58-win64-VS17.zip"),
                    Type = PackageType.Apache,
                    ServerName = "Apache",
                    EstimatedSize = 14 * 1024 * 1024,
                    Description = "Apache HTTP Server - The world's most widely used web server software.",
                    InstallPath = "apps/apache",
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