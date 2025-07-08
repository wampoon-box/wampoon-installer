using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PWAMP.Installer.Neo.Core.Events;
using PWAMP.Installer.Neo.Core.Paths;
using PWAMP.Installer.Neo.Helpers;
using PWAMP.Installer.Neo.Helpers.Common;

namespace PWAMP.Installer.Neo.Core
{
    public class InstallManager : IDisposable
    {
        public event EventHandler<InstallProgressEventArgs> ProgressChanged;
        public event EventHandler<InstallErrorEventArgs> ErrorOccurred;
        public event EventHandler<EventArgs> InstallationCompleted;

        private readonly PackageManager _packageManager;
        private readonly List<string> _selectedPackages;
        private int _totalSteps;
        private int _currentStep;
        private bool _disposed = false;
        private IPathResolver _pathResolver;

        public InstallManager()
        {
            _packageManager = new PackageManager();
            _selectedPackages = new List<string>();
        }

        public async Task InstallAsync(InstallOptions options, CancellationToken cancellationToken = default)
        {
            try
            {
                _currentStep = 0;
                _totalSteps = CalculateTotalSteps(options);
                _selectedPackages.Clear();

                // Initialize path resolver
                _pathResolver = PathResolverFactory.CreatePathResolver(options.InstallPath);

                ReportProgress("Starting PWAMP installation...", 0, "Initialization");

                // Validate installation directory
                await ValidateInstallationPathAsync(options.InstallPath);

                // Create base directories
                await CreateBaseDirectoriesAsync();

                // Phase 1: Install all selected packages
                await InstallPackagesAsync(options, cancellationToken);

                //FIXME: uncomment the previous line and remove the following:
                //_selectedPackages.Add(PackageNames.Apache);
                // Phase 2: Configure all installed packages
                await ConfigurePackagesAsync(cancellationToken);

                // Final validation
                await ValidateInstallationAsync(options);

                ReportProgress("PWAMP installation completed successfully!", 100, "Completed");
                InstallationCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (OperationCanceledException)
            {
                ReportProgress("Installation was cancelled by user", _currentStep * 100 / _totalSteps, "Cancelled");
                throw;
            }
            catch (Exception ex)
            {
                ReportError($"Installation failed: {ex.Message}", ex);
                throw;
            }
        }

        private async Task ValidateInstallationPathAsync(string installPath)
        {
            ReportProgress("Validating installation path...", GetProgressPercentage(), "Validation");
            
            try
            {
                await FileHelper.CreateDirectoryIfNotExistsAsync(installPath);
                ReportProgress($"Installation path validated: {installPath}", GetProgressPercentage(), "Validation");
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot create installation directory: {ex.Message}");
            }
        }

        private async Task CreateBaseDirectoriesAsync()
        {
            ReportProgress("Creating base directories...", GetProgressPercentage(), "Directory Creation");
            
            var directories = new[]
            {
                _pathResolver.GetAppsDirectory(),
                _pathResolver.GetHtdocsDirectory(),
            };

            foreach (var directory in directories)
            {
                await FileHelper.CreateDirectoryIfNotExistsAsync(directory);
            }
            
            ReportProgress("Base directories created", GetProgressPercentage(), "Directory Creation");
        }

        private async Task InstallPackagesAsync(InstallOptions options, CancellationToken cancellationToken)
        {
            ReportProgress("Installing selected packages...", GetProgressPercentage(), "Package Installation");

            if (options.InstallApache)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await InstallApacheAsync(options.InstallPath, cancellationToken);
                _selectedPackages.Add("apache");
            }
            
            if (options.InstallMariaDB)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await InstallMariaDBAsync(options.InstallPath, cancellationToken);
                _selectedPackages.Add("mariadb");
            }

            if (options.InstallPHP)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await InstallPHPAsync(options.InstallPath, cancellationToken);
                _selectedPackages.Add("php");
            }

            if (options.InstallPhpMyAdmin)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await InstallPhpMyAdminAsync(options.InstallPath, cancellationToken);
                _selectedPackages.Add("phpmyadmin");
            }

            ReportProgress("Package installation completed", GetProgressPercentage(), "Package Installation");
        }

        private async Task ConfigurePackagesAsync(CancellationToken cancellationToken)
        {
            ReportProgress("Configuring installed packages...", GetProgressPercentage(), "Package Configuration");

            foreach (var package in _selectedPackages)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ConfigurePackageAsync(package);
            }

            ReportProgress("Package configuration completed", GetProgressPercentage(), "Package Configuration");
        }

        private async Task ConfigurePackageAsync(string packageName)
        {
            var progress = new Progress<string>(message => 
                ReportProgress(message, GetProgressPercentage(), $"{packageName} Configuration"));

            switch (packageName.ToLower())
            {
                case PackageNames.Apache:
                    await ApacheConfigHelper.ConfigureApacheAsync(_pathResolver, progress);
                    break;
                case PackageNames.MariaDB:
                    await MariaDBConfigHelper.ConfigureMariaDBAsync(_pathResolver, progress);
                    break;
                case PackageNames.PHP:
                    await PHPConfigHelper.ConfigurePHPAsync(_pathResolver, progress);
                    break;
                case PackageNames.PhpMyAdmin:
                    await PhpMyAdminConfigHelper.ConfigurePhpMyAdminAsync(_pathResolver, progress);
                    break;
            }
        }

        private async Task InstallApacheAsync(string installPath, CancellationToken cancellationToken)
        {
            _currentStep++;
            ReportProgress("Installing Apache HTTP Server...", GetProgressPercentage(), "Apache Installation");
            
            var progress = new Progress<string>(message => 
                ReportProgress(message, GetProgressPercentage(), "Apache Installation"));
            
            await _packageManager.DownloadAndExtractPackageAsync(PackageNames.Apache, installPath, progress, cancellationToken);
            
            // Handle Apache24 nested folder - move contents to parent apache folder
            var apacheInstallPath = _pathResolver.GetPackageDirectory(PackageNames.Apache);
            var apache24Path = System.IO.Path.Combine(apacheInstallPath, "Apache24");
            
            if (System.IO.Directory.Exists(apache24Path))
            {
                ((IProgress<string>)progress).Report("Moving Apache24 contents to parent folder...");
                await FileHelper.MoveDirectoryContentsAsync(apache24Path, apacheInstallPath);
                System.IO.Directory.Delete(apache24Path, true);
                ((IProgress<string>)progress).Report("Apache24 folder structure normalized");
            }
        }

        private async Task InstallMariaDBAsync(string installPath, CancellationToken cancellationToken)
        {
            _currentStep++;
            ReportProgress("Installing MariaDB Database...", GetProgressPercentage(), "MariaDB Installation");
            
            var progress = new Progress<string>(message => 
                ReportProgress(message, GetProgressPercentage(), "MariaDB Installation"));
            
            await _packageManager.DownloadAndExtractPackageAsync(PackageNames.MariaDB, installPath, progress, cancellationToken);
        }

        private async Task InstallPHPAsync(string installPath, CancellationToken cancellationToken)
        {
            _currentStep++;
            ReportProgress("Installing PHP Scripting Language...", GetProgressPercentage(), "PHP Installation");
            
            var progress = new Progress<string>(message => 
                ReportProgress(message, GetProgressPercentage(), "PHP Installation"));
            
            await _packageManager.DownloadAndExtractPackageAsync(PackageNames.PHP, installPath, progress, cancellationToken);
        }

        private async Task InstallPhpMyAdminAsync(string installPath, CancellationToken cancellationToken)
        {
            _currentStep++;
            ReportProgress("Installing phpMyAdmin Database Manager...", GetProgressPercentage(), "phpMyAdmin Installation");
            
            var progress = new Progress<string>(message => 
                ReportProgress(message, GetProgressPercentage(), "phpMyAdmin Installation"));
            
            await _packageManager.DownloadAndExtractPackageAsync(PackageNames.PhpMyAdmin, installPath, progress, cancellationToken);
        }

        private async Task ValidateInstallationAsync(InstallOptions options)
        {
            ReportProgress("Validating installation...", GetProgressPercentage(), "Final Validation");
            
            var selectedPackages = options.GetSelectedPackages();
            foreach (var package in selectedPackages)
            {
                var isValid = await FileHelper.ValidatePackageConfigurationAsync(options.InstallPath, package);
                if (!isValid)
                {
                    throw new Exception($"Installation validation failed for {package}");
                }
            }
            
            ReportProgress("Installation validation completed", GetProgressPercentage(), "Final Validation");
        }

        private int CalculateTotalSteps(InstallOptions options)
        {
            int steps = 4; // Base setup + installation phase + configuration phase + validation
            
            int packageCount = 0;
            if (options.InstallApache) packageCount++;
            if (options.InstallMariaDB) packageCount++;
            if (options.InstallPHP) packageCount++;
            if (options.InstallPhpMyAdmin) packageCount++;
            
            steps += packageCount * 2; // Each package has install + configure step
            
            return steps;
        }

        private int GetProgressPercentage()
        {
            return (_currentStep * 100) / _totalSteps;
        }

        private void ReportProgress(string message, int percentComplete = 0, string currentStep = "")
        {
            ProgressChanged?.Invoke(this, new InstallProgressEventArgs(message, percentComplete, currentStep));
        }

        private void ReportError(string message, Exception exception = null, string component = "")
        {
            ErrorOccurred?.Invoke(this, new InstallErrorEventArgs(message, exception, component));
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _packageManager?.Dispose();
                _disposed = true;
            }
        }
    }
}