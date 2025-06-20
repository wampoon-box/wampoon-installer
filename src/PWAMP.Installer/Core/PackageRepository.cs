using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using PWAMP.Installer.Models;
using Newtonsoft.Json;

namespace PWAMP.Installer.Core
{
    public class PackageRepository : IDisposable
    {
        private readonly HttpClient _httpClient;
        private List<InstallablePackage> _packages;
        private bool _disposed = false;

        public PackageRepository()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(5);
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
                _packages = GetDefaultPackages();
            }

            return _packages;
        }

        private async Task<List<InstallablePackage>> LoadPackagesFromManifestAsync()
        {
            const string manifestUrl = "https://raw.githubusercontent.com/your-org/pwamp-packages/main/packages.json";
            
            try
            {
                var json = await _httpClient.GetStringAsync(manifestUrl);
                return JsonConvert.DeserializeObject<List<InstallablePackage>>(json) ?? GetDefaultPackages();
            }
            catch
            {
                return GetDefaultPackages();
            }
        }

        private List<InstallablePackage> GetDefaultPackages()
        {
            return new List<InstallablePackage>
            {
                new InstallablePackage
                {
                    Name = "Apache HTTP Server",
                    Version = new Version(2, 4, 58),
                    DownloadUrl = new Uri("https://www.apachelounge.com/download/VS17/binaries/httpd-2.4.58-win64-VS17.zip"),
                    Type = PackageType.Apache,
                    ServerName = "Apache",
                    EstimatedSize = 14 * 1024 * 1024,
                    Description = "Apache HTTP Server - The world's most widely used web server software.",
                    InstallPath = "apps/apache"
                },
                new InstallablePackage
                {
                    Name = "MariaDB Server",
                    Version = new Version(11, 1, 3),
                    DownloadUrl = new Uri("https://downloads.mariadb.org/rest-api/mariadb/11.1.3/mariadb-11.1.3-winx64.zip"),
                    Type = PackageType.MariaDB,
                    ServerName = "MariaDB",
                    EstimatedSize = 180 * 1024 * 1024,
                    Description = "MariaDB Server - Popular MySQL-compatible database server.",
                    InstallPath = "apps/mariadb"
                },
                new InstallablePackage
                {
                    Name = "PHP Runtime",
                    Version = new Version(8, 2, 13),
                    DownloadUrl = new Uri("https://windows.php.net/downloads/releases/php-8.2.13-Win32-vs16-x64.zip"),
                    Type = PackageType.PHP,
                    ServerName = "PHP",
                    EstimatedSize = 32 * 1024 * 1024,
                    Description = "PHP - Server-side scripting language designed for web development.",
                    InstallPath = "apps/php",
                    Dependencies = new List<string> { "Apache HTTP Server" }
                },
                new InstallablePackage
                {
                    Name = "phpMyAdmin",
                    Version = new Version(5, 2, 1),
                    DownloadUrl = new Uri("https://files.phpmyadmin.net/phpMyAdmin/5.2.1/phpMyAdmin-5.2.1-all-languages.zip"),
                    Type = PackageType.PhpMyAdmin,
                    ServerName = "phpMyAdmin",
                    EstimatedSize = 18 * 1024 * 1024,
                    Description = "phpMyAdmin - Web-based database administration tool for MySQL and MariaDB.",
                    InstallPath = "apps/phpmyadmin",
                    Dependencies = new List<string> { "PHP Runtime", "MariaDB Server" }
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

        public List<InstallablePackage> ResolveDependencies(List<InstallablePackage> selectedPackages)
        {
            var result = new List<InstallablePackage>(selectedPackages);
            var toProcess = new Queue<InstallablePackage>(selectedPackages);

            while (toProcess.Count > 0)
            {
                var package = toProcess.Dequeue();
                
                foreach (var dependencyName in package.Dependencies)
                {
                    var dependency = GetPackageByName(dependencyName);
                    if (dependency != null && !result.Any(p => p.Name == dependency.Name))
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