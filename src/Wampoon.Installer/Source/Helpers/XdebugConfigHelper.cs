using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wampoon.Installer.Core;
using Wampoon.Installer.Core.Paths;
using Wampoon.Installer.Helpers.Common;
using Wampoon.Installer.Helpers.Logging;

namespace Wampoon.Installer.Helpers
{
    public static class XdebugConfigHelper
    {
        private static readonly XdebugConfigHelperImpl _impl = new XdebugConfigHelperImpl();

        public static async Task ConfigureXdebugAsync(string installPath, IProgress<string> logger)
        {
            await _impl.ConfigureAsync(installPath, logger);
        }

        public static async Task ConfigureXdebugAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            await _impl.ConfigureAsync(pathResolver, logger);
        }

        private class XdebugConfigHelperImpl
        {
            public async Task ConfigureAsync(string installPath, IProgress<string> logger)
            {
                var pathResolver = PathResolverFactory.CreatePathResolver(installPath);
                await ConfigureAsync(pathResolver, logger);
            }

            public async Task ConfigureAsync(IPathResolver pathResolver, IProgress<string> logger)
            {
                try
                {
                    logger?.Report("Configuring Xdebug PHP Extension...");

                    // Validate that PHP is installed.
                    await ValidatePhpInstallationAsync(pathResolver, logger);

                    // Move Xdebug DLL to PHP ext directory.
                    await InstallXdebugDllAsync(pathResolver, logger);

                    // Update php.ini to enable Xdebug.
                    await UpdatePhpIniForXdebugAsync(pathResolver, logger);

                    logger?.Report("✓ Xdebug configuration completed successfully");
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.LogExceptionInfo(ex);
                    logger?.Report($"⚠️ Xdebug installation failed: {ex.Message}");
                    logger?.Report("⚠️ Xdebug can be installed manually by downloading the DLL and placing it in PHP's ext directory");
                    logger?.Report("⚠️ Visit https://xdebug.org/download for manual installation instructions");
                    // DO NOT throw - just log the error and continue.
                }
            }

            private async Task ValidatePhpInstallationAsync(IPathResolver pathResolver, IProgress<string> logger)
            {
                logger?.Report("Validating PHP installation...");

                var phpDir = pathResolver.GetPackageDirectory(AppSettings.PackageNames.PHP);
                var phpExtDir = Path.Combine(phpDir, "ext");
                var phpIniPath = pathResolver.GetConfigPath(AppSettings.PackageNames.PHP, AppSettings.PHPFiles.PhpIni);

                if (!Directory.Exists(phpDir))
                {
                    throw new Exception("PHP installation directory not found. Xdebug requires PHP to be installed first.");
                }

                if (!File.Exists(phpIniPath))
                {
                    throw new Exception("PHP configuration file (php.ini) not found. Please ensure PHP is properly configured.");
                }

                // Ensure PHP ext directory exists.
                await FileHelper.CreateDirectoryIfNotExistsAsync(phpExtDir);

                logger?.Report("✓ PHP installation validated");
            }

            private async Task InstallXdebugDllAsync(IPathResolver pathResolver, IProgress<string> logger)
            {
                await Task.Run(() =>
                {
                    logger?.Report("Installing Xdebug DLL...");

                    var installPath = Path.GetDirectoryName(pathResolver.GetAppsDirectory()); // Get root install path.
                    var phpExtDir = pathResolver.GetSubdirectoryPath(AppSettings.PackageNames.PHP, "ext");
                    var downloadsDir = Path.Combine(installPath, "downloads");



                    /*
                     logger?.Report($"DEBUG: Looking for Xdebug DLL in: {downloadsDir}");
                    logger?.Report($"DEBUG: Downloads directory exists: {Directory.Exists(downloadsDir)}");
                     if (Directory.Exists(downloadsDir))
                    {
                        var allFiles = Directory.GetFiles(downloadsDir);
                        logger?.Report($"DEBUG: Files in downloads directory: {string.Join(", ", allFiles.Select(Path.GetFileName))}");
                    }*/

                    // Find the downloaded Xdebug DLL file (it's downloaded as a single file, not extracted).
                    var xdebugDllFiles = Directory.GetFiles(downloadsDir, "*xdebug*.dll", SearchOption.TopDirectoryOnly);

                    if (xdebugDllFiles.Length == 0)
                    {
                        throw new Exception($"Xdebug DLL file not found in downloads directory: {downloadsDir}");
                    }

                    var sourceDllPath = xdebugDllFiles[0];
                    var targetDllPath = Path.Combine(phpExtDir, "php_xdebug.dll");

                    // Copy the DLL to PHP's ext directory.
                    File.Copy(sourceDllPath, targetDllPath, overwrite: true);

                    logger?.Report($"✓ Xdebug DLL copied from {sourceDllPath} to {targetDllPath}");

                    // Clean up: delete the downloaded DLL file and remove downloads directory if empty.
                    try
                    {
                        File.Delete(sourceDllPath);
                        logger?.Report($"✓ Cleaned up downloaded Xdebug DLL: {Path.GetFileName(sourceDllPath)}");

                        // Remove downloads directory if it's empty.
                        if (Directory.Exists(downloadsDir) && !Directory.EnumerateFileSystemEntries(downloadsDir).Any())
                        {
                            Directory.Delete(downloadsDir);
                            logger?.Report("✓ Removed empty downloads directory");
                        }

                        // Clean up temp/xdebug directory.
                        var rootInstallPath = Path.GetDirectoryName(pathResolver.GetAppsDirectory());
                        var xdebugTempDir = Path.Combine(rootInstallPath, "temp", "xdebug");
                        if (Directory.Exists(xdebugTempDir))
                        {
                            Directory.Delete(xdebugTempDir, true);
                            logger?.Report("✓ Removed temp/xdebug directory");
                        }

                        // Remove temp directory if it's empty.
                        var tempDir = Path.Combine(rootInstallPath, "temp");
                        if (Directory.Exists(tempDir) && !Directory.EnumerateFileSystemEntries(tempDir).Any())
                        {
                            Directory.Delete(tempDir);
                            logger?.Report("✓ Removed empty temp directory");
                        }
                    }
                    catch (Exception cleanupEx)
                    {
                        // Don't fail if cleanup fails, just log it.
                        logger?.Report($"Note: Could not clean up download files: {cleanupEx.Message}");
                    }
                });
            }

            private async Task UpdatePhpIniForXdebugAsync(IPathResolver pathResolver, IProgress<string> logger)
            {
                await Task.Run(() =>
                {
                    logger?.Report("Updating php.ini for Xdebug...");

                    var phpIniPath = pathResolver.GetConfigPath(AppSettings.PackageNames.PHP, AppSettings.PHPFiles.PhpIni);

                    // Read the current php.ini content.
                    var phpIniContent = File.ReadAllText(phpIniPath);

                    // Check if Xdebug is already configured.
                    if (phpIniContent.Contains("zend_extension") && phpIniContent.Contains("xdebug"))
                    {
                        logger?.Report("✓ Xdebug already configured in php.ini");
                        return;
                    }

                    // Add Xdebug configuration to php.ini.
                    var xdebugConfig = Environment.NewLine + Environment.NewLine +
                        "; Xdebug Configuration" + Environment.NewLine +
                        "zend_extension=php_xdebug.dll" + Environment.NewLine +
                        "xdebug.mode=debug,develop" + Environment.NewLine +
                        "xdebug.start_with_request=yes" + Environment.NewLine +
                        "xdebug.client_port=9003" + Environment.NewLine;
                    //"xdebug.client_host=127.0.0.1" + Environment.NewLine +
                    //"xdebug.log_level=0" + Environment.NewLine;

                    var updatedContent = phpIniContent + xdebugConfig;
                    File.WriteAllText(phpIniPath, updatedContent);

                    logger?.Report("✓ php.ini updated with Xdebug configuration");
                });
            }
        }
    }
}