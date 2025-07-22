using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Wampoon.Installer.Core.Paths;
using Wampoon.Installer.Helpers;
using Wampoon.Installer.Helpers.Common;

namespace Wampoon.Installer.Core.Installation
{
    public class PackageInstaller : IPackageInstaller
    {
        private readonly PackageManager _packageManager;
        private readonly IPathResolver _pathResolver;

        public PackageInstaller(PackageManager packageManager, IPathResolver pathResolver)
        {
            _packageManager = packageManager ?? throw new ArgumentNullException(nameof(packageManager));
            _pathResolver = pathResolver ?? throw new ArgumentNullException(nameof(pathResolver));
        }

        public async Task InstallAsync(string packageName, string installPath, IProgress<string> progress, CancellationToken cancellationToken)
        {
            if (!CanInstall(packageName))
            {
                throw new ArgumentException($"Package '{packageName}' is not supported for installation");
            }

            progress?.Report($"Installing {GetPackageDisplayName(packageName)}...");
            
            await _packageManager.DownloadAndExtractPackageAsync(packageName, installPath, progress, cancellationToken);
            
            if (RequiresPostInstallProcessing(packageName))
            {
                await PostInstallProcessAsync(packageName, installPath, progress);
            }
        }

        public async Task ConfigureAsync(string packageName, IPathResolver pathResolver, IProgress<string> progress, CancellationToken cancellationToken)
        {
            if (!CanInstall(packageName))
            {
                throw new ArgumentException($"Package '{packageName}' is not supported for configuration");
            }

            progress?.Report($"Configuring {GetPackageDisplayName(packageName)}...");

            switch (packageName.ToLower())
            {
                case AppSettings.PackageNames.Apache:
                    await ApacheConfigHelper.ConfigureApacheAsync(pathResolver, progress);
                    break;
                case AppSettings.PackageNames.MariaDB:
                    await MariaDBConfigHelper.ConfigureMariaDBAsync(pathResolver, progress);
                    await MariaDBConfigHelper.InitializeMariaDBDataDirectoryAsync(pathResolver, progress);
                    break;
                case AppSettings.PackageNames.PHP:
                    await PHPConfigHelper.ConfigurePHPAsync(pathResolver, progress);
                    break;
                case AppSettings.PackageNames.PhpMyAdmin:
                    await PhpMyAdminConfigHelper.ConfigurePhpMyAdminAsync(pathResolver, progress);
                    break;
                case AppSettings.PackageNames.Xdebug:
                    await XdebugConfigHelper.ConfigureXdebugAsync(pathResolver, progress);
                    break;
                default:
                    throw new ArgumentException($"Configuration not supported for package '{packageName}'");
            }
        }

        public async Task<bool> ValidateAsync(string packageName, string installPath)
        {
            return await FileHelper.ValidatePackageConfigurationAsync(installPath, packageName);
        }

        public bool CanInstall(string packageName)
        {
            if (packageName == null)
                return false;
                
            switch (packageName.ToLower())
            {
                case AppSettings.PackageNames.Apache:
                case AppSettings.PackageNames.MariaDB:
                case AppSettings.PackageNames.PHP:
                case AppSettings.PackageNames.PhpMyAdmin:
                case AppSettings.PackageNames.Dashboard:
                case AppSettings.PackageNames.ControlPanel:
                case AppSettings.PackageNames.Xdebug:
                    return true;
                default:
                    return false;
            }
        }

        public bool RequiresPostInstallProcessing(string packageName)
        {
            if (packageName == null)
                return false;
                
            switch (packageName.ToLower())
            {
                case AppSettings.PackageNames.Apache:
                    return true; // Needs Apache24 folder processing
                default:
                    return false;
            }
        }

        public async Task PostInstallProcessAsync(string packageName, string installPath, IProgress<string> progress)
        {
            switch (packageName.ToLower())
            {
                case AppSettings.PackageNames.Apache:
                    await ProcessApacheInstallationAsync(installPath, progress);
                    break;
                default:
                    break;
            }
        }

        private async Task ProcessApacheInstallationAsync(string installPath, IProgress<string> progress)
        {
            // Handle Apache24 nested folder - move contents to parent apache folder
            var apacheInstallPath = _pathResolver.GetPackageDirectory(AppSettings.PackageNames.Apache);
            var apache24Path = Path.Combine(apacheInstallPath, "Apache24");
            
            if (Directory.Exists(apache24Path))
            {
                progress?.Report("Moving Apache24 contents to parent folder...");
                await FileHelper.MoveDirectoryContentsAsync(apache24Path, apacheInstallPath);
                Directory.Delete(apache24Path, true);
                progress?.Report("Apache24 folder structure normalized");
            }
        }

        private string GetPackageDisplayName(string packageName)
        {
            if (packageName == null)
                return null;
                
            switch (packageName.ToLower())
            {
                case AppSettings.PackageNames.Apache:
                    return "Apache HTTP Server";
                case AppSettings.PackageNames.MariaDB:
                    return "MariaDB Database";
                case AppSettings.PackageNames.PHP:
                    return "PHP Scripting Language";
                case AppSettings.PackageNames.PhpMyAdmin:
                    return "phpMyAdmin Database Manager";
                case AppSettings.PackageNames.Dashboard:
                    return "Wampoon Dashboard";
                case AppSettings.PackageNames.ControlPanel:
                    return "Wampoon Control Panel";
                case AppSettings.PackageNames.Xdebug:
                    return "Xdebug PHP Extension";
                default:
                    return packageName;
            }
        }
    }
}