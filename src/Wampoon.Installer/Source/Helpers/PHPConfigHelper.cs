using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Wampoon.Installer.Helpers.Common;
using Wampoon.Installer.Core;
using Wampoon.Installer.Core.Paths;

namespace Wampoon.Installer.Helpers
{
    public static class PHPConfigHelper
    {
        private static readonly PHPConfigHelperImpl _impl = new PHPConfigHelperImpl();

        public static async Task ConfigurePHPAsync(string installPath, IProgress<string> logger)
        {
            await _impl.ConfigureAsync(installPath, logger);
        }

        public static async Task ConfigurePHPAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            await _impl.ConfigureAsync(pathResolver, logger);
        }

        private class PHPConfigHelperImpl : BaseConfigHelper
        {
            protected override string PackageName => AppSettings.PackageNames.PHP;
            protected override string DisplayName => "PHP Scripting Language";
            protected override string BinaryFileName => AppSettings.PHPFiles.PhpExe;
            protected override string TemplateFileName => AppSettings.PHPFiles.Templates.PhpIni;
            protected override string ConfigFileName => AppSettings.PHPFiles.PhpIni;

            protected override async Task CreatePackageSpecificDirectoriesAsync(IPathResolver pathResolver, string packageDir, IProgress<string> logger)
            {
                // Create required PHP folders.
                await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(packageDir, "logs"));
                await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(packageDir, "sessions"));
                await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(packageDir, "temp"));
                await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(packageDir, "extra"));
            }

            protected override async Task ConfigureAdditionalTemplatesAsync(IPathResolver pathResolver, IProgress<string> logger)
            {
                // Download browscap.ini file.
                await DownloadBrowscapFileAsync(pathResolver, logger);
                
                // Update php.ini with correct extension_dir path.
                //await UpdatePhpIniExtensionDirAsync(pathResolver, logger);
                
                // Download curl certificate bundle.
                await DownloadCurlCertificateAsync(pathResolver, logger);
            }

            private async Task DownloadBrowscapFileAsync(IPathResolver pathResolver, IProgress<string> logger)
            {
                const string browscapUrl = "https://browscap.org/stream?q=Lite_PHP_BrowsCapINI";
                const string targetFileName = "lite_php_browscap.ini";
                
                try
                {
                    logger?.Report("Downloading browscap.ini file...");
                    
                    // Get the PHP extra directory path.
                    var phpExtraDir = pathResolver.GetSubdirectoryPath(AppSettings.PackageNames.PHP, "extras");
                    var targetPath = Path.Combine(phpExtraDir, targetFileName);
                    
                    // Ensure the extra directory exists.
                    await FileHelper.CreateDirectoryIfNotExistsAsync(phpExtraDir);
                    
                    // Download the browscap file.
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.Timeout = TimeSpan.FromMinutes(5);
                        httpClient.DefaultRequestHeaders.Add("User-Agent", AppConstants.USER_AGENT);
                        
                        using (var response = await httpClient.GetAsync(browscapUrl))
                        {
                            response.EnsureSuccessStatusCode();
                            
                            var content = await response.Content.ReadAsByteArrayAsync();
                            using (var fileStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write))
                            {
                                await fileStream.WriteAsync(content, 0, content.Length);
                            }
                        }
                    }
                    
                    logger?.Report($"✓ Browscap.ini file downloaded to: {targetPath}");
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.LogExceptionInfo(ex);
                    logger?.Report($"✗ Failed to download browscap.ini file: {ex.Message}");
                    throw;
                }
            }
          

            protected override string GetTemplateFilePattern()
            {
                return "*.ini";
            }

            private async Task DownloadCurlCertificateAsync(IPathResolver pathResolver, IProgress<string> logger)
            {
                const string curlCertUrl = "https://curl.se/ca/cacert.pem";
                const string targetFileName = "curl-ca-bundle.crt";
                
                try
                {
                    logger?.Report("Downloading curl certificate bundle...");
                    
                    // Get the PHP extras/ssl directory path.
                    var phpExtrasDir = pathResolver.GetSubdirectoryPath(AppSettings.PackageNames.PHP, "extras");
                    var sslDir = Path.Combine(phpExtrasDir, "ssl");
                    var targetPath = Path.Combine(sslDir, targetFileName);
                    
                    // Ensure the ssl directory exists.
                    await FileHelper.CreateDirectoryIfNotExistsAsync(sslDir);
                    
                    // Download the certificate file.
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.Timeout = TimeSpan.FromMinutes(5);
                        httpClient.DefaultRequestHeaders.Add("User-Agent", AppConstants.USER_AGENT);
                        
                        using (var response = await httpClient.GetAsync(curlCertUrl))
                        {
                            response.EnsureSuccessStatusCode();
                            
                            var content = await response.Content.ReadAsByteArrayAsync();
                            using (var fileStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write))
                            {
                                await fileStream.WriteAsync(content, 0, content.Length);
                            }
                        }
                    }
                    
                    logger?.Report($"✓ Curl certificate bundle downloaded to: {targetPath}");
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.LogExceptionInfo(ex);
                    logger?.Report($"✗ Failed to download curl certificate bundle: {ex.Message}");
                    throw;
                }
            }
        }
    }
}