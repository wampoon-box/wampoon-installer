using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Wampoon.Installer.Core.Paths;

namespace Wampoon.Installer.Core.Installation
{
    public class InstallationCoordinator : IInstallationCoordinator
    {
        private readonly IPackageInstaller _packageInstaller;
        private readonly IPathResolver _pathResolver;

        public InstallationCoordinator(IPackageInstaller packageInstaller, IPathResolver pathResolver)
        {
            _packageInstaller = packageInstaller ?? throw new ArgumentNullException(nameof(packageInstaller));
            _pathResolver = pathResolver ?? throw new ArgumentNullException(nameof(pathResolver));
        }

        public async Task ExecuteInstallationAsync(InstallOptions options, IProgress<string> progress, CancellationToken cancellationToken)
        {
            progress?.Report("Installing selected packages...");

            if (options.InstallApache)
            {
                await InstallPackageAsync(PackageNames.Apache, options.InstallPath, progress, cancellationToken);
            }

            if (options.InstallMariaDB)
            {
                await InstallPackageAsync(PackageNames.MariaDB, options.InstallPath, progress, cancellationToken);
            }

            if (options.InstallPHP)
            {
                await InstallPackageAsync(PackageNames.PHP, options.InstallPath, progress, cancellationToken);
            }

            if (options.InstallPhpMyAdmin)
            {
                await InstallPackageAsync(PackageNames.PhpMyAdmin, options.InstallPath, progress, cancellationToken);
            }

            progress?.Report("Package installation completed");
        }

        public async Task ExecuteConfigurationAsync(string[] packageNames, IProgress<string> progress, CancellationToken cancellationToken)
        {
            progress?.Report("Configuring installed packages...");

            foreach (var packageName in packageNames)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _packageInstaller.ConfigureAsync(packageName, _pathResolver, progress, cancellationToken);
            }

            progress?.Report("Package configuration completed");
        }

        public async Task<bool> ValidateInstallationAsync(InstallOptions options)
        {
            var selectedPackages = options.GetSelectedPackages();
            var installPath = options.InstallPath;

            foreach (var package in selectedPackages)
            {
                var isValid = await _packageInstaller.ValidateAsync(package, installPath);
                if (!isValid)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task InstallPackageAsync(string packageName, string installPath, IProgress<string> progress, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _packageInstaller.InstallAsync(packageName, installPath, progress, cancellationToken);
        }
    }
}