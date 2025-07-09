using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PWAMP.Installer.Neo.Core.Events;
using PWAMP.Installer.Neo.Core.Installation;
using PWAMP.Installer.Neo.Core.Paths;
using PWAMP.Installer.Neo.Helpers.Common;

namespace PWAMP.Installer.Neo.Core
{
    public class InstallManager : IDisposable
    {
        public event EventHandler<InstallProgressEventArgs> ProgressChanged;
        public event EventHandler<InstallErrorEventArgs> ErrorOccurred;
        public event EventHandler<EventArgs> InstallationCompleted;

        private readonly PackageManager _packageManager;
        private readonly IInstallationCoordinator _installationCoordinator;
        private readonly IInstallationValidator _installationValidator;
        private readonly List<string> _selectedPackages;
        private int _totalSteps;
        private int _currentStep;
        private bool _disposed = false;
        private IPathResolver _pathResolver;

        public InstallManager()
        {
            _packageManager = new PackageManager();
            _selectedPackages = new List<string>();
            _installationValidator = new InstallationValidator();
        }

        public async Task InstallAsync(InstallOptions options, CancellationToken cancellationToken = default)
        {
            try
            {
                _currentStep = 0;
                _totalSteps = CalculateTotalSteps(options);
                _selectedPackages.Clear();

                // Initialize path resolver and installation coordinator
                _pathResolver = PathResolverFactory.CreatePathResolver(options.InstallPath);
                var packageInstaller = new PackageInstaller(_packageManager, _pathResolver);
                var installationCoordinator = new InstallationCoordinator(packageInstaller, _pathResolver);

                ReportProgress("Starting PWAMP installation...", 0, "Initialization");

                // Validate installation directory
                await _installationValidator.ValidateInstallationPathAsync(options.InstallPath);
                ReportProgress($"Installation path validated: {options.InstallPath}", GetProgressPercentage(), "Validation");

                // Create base directories
                await CreateBaseDirectoriesAsync();

                // Phase 1: Install all selected packages
                var installProgress = new Progress<string>(message => 
                    ReportProgress(message, GetProgressPercentage(), "Package Installation"));
                await installationCoordinator.ExecuteInstallationAsync(options, installProgress, cancellationToken);
                
                // Track selected packages for configuration
                TrackSelectedPackages(options);

                // Phase 2: Configure all installed packages
                var configProgress = new Progress<string>(message => 
                    ReportProgress(message, GetProgressPercentage(), "Package Configuration"));
                await installationCoordinator.ExecuteConfigurationAsync(_selectedPackages.ToArray(), configProgress, cancellationToken);

                // Final validation
                var isValid = await _installationValidator.ValidateCompleteInstallationAsync(options);
                if (!isValid)
                {
                    throw new Exception("Installation validation failed");
                }
                ReportProgress("Installation validation completed", GetProgressPercentage(), "Final Validation");

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

        private void TrackSelectedPackages(InstallOptions options)
        {
            if (options.InstallApache) _selectedPackages.Add(PackageNames.Apache);
            if (options.InstallMariaDB) _selectedPackages.Add(PackageNames.MariaDB);
            if (options.InstallPHP) _selectedPackages.Add(PackageNames.PHP);
            if (options.InstallPhpMyAdmin) _selectedPackages.Add(PackageNames.PhpMyAdmin);
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