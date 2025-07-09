using System;
using System.Collections.Generic;
using System.IO;
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
        public event EventHandler<ExistingPackagesEventArgs> ExistingPackagesDetected;

        private readonly PackageManager _packageManager;
        private readonly IInstallationCoordinator _installationCoordinator;
        private readonly IInstallationValidator _installationValidator;
        private readonly List<string> _selectedPackages;
        private int _totalSteps;
        private int _currentStep;
        private bool _disposed = false;
        private IPathResolver _pathResolver;
        private int _totalPackages;
        private int _currentPackageIndex;
        private double _currentPackageProgress;

        public InstallManager()
        {
            _packageManager = new PackageManager();
            _selectedPackages = new List<string>();
            _installationValidator = new InstallationValidator();
            
            // Subscribe to package manager progress events
            SubscribeToPackageManagerEvents();
        }

        public async Task InstallAsync(InstallOptions options, CancellationToken cancellationToken = default)
        {
            try
            {
                _currentStep = 0;
                _totalSteps = CalculateTotalSteps(options);
                _selectedPackages.Clear();
                
                // Initialize package progress tracking
                _totalPackages = CountSelectedPackages(options);
                _currentPackageIndex = 0;
                _currentPackageProgress = 0;

                // Initialize path resolver and installation coordinator
                _pathResolver = PathResolverFactory.CreatePathResolver(options.InstallPath);
                var packageInstaller = new PackageInstaller(_packageManager, _pathResolver);
                var installationCoordinator = new InstallationCoordinator(packageInstaller, _pathResolver);

                ReportProgress("Starting PWAMP installation...", 0, "Initialization");

                // Validate installation directory
                await _installationValidator.ValidateInstallationPathAsync(options.InstallPath);
                ReportProgress($"Installation path validated: {options.InstallPath}", GetProgressPercentage(), "Validation");

                // Check for existing packages
                var selectedPackages = options.GetSelectedPackages();
                var existingPackages = await CheckForExistingPackagesAsync(options.InstallPath, selectedPackages);
                
                if (existingPackages.Length > 0)
                {
                    // Notify caller about existing packages and wait for confirmation
                    var existingPackagesEventArgs = new ExistingPackagesEventArgs(existingPackages);
                    ExistingPackagesDetected?.Invoke(this, existingPackagesEventArgs);
                    
                    // If the caller doesn't set OverwriteRequested to true, cancel the installation
                    if (!existingPackagesEventArgs.OverwriteRequested)
                    {
                        ReportProgress("Installation cancelled due to existing packages", 0, "Cancelled");
                        return;
                    }
                    
                    ReportProgress($"Continuing with installation - overwriting existing packages: {string.Join(", ", existingPackages)}", GetProgressPercentage(), "Validation");
                }

                // Create base directories
                await CreateBaseDirectoriesAsync();

                // Phase 1: Install all selected packages
                var installProgress = new Progress<string>(message => 
                    ReportProgress(message, GetDetailedProgressPercentage(), "Package Installation"));
                await installationCoordinator.ExecuteInstallationAsync(options, installProgress, cancellationToken);
                
                // Track selected packages for configuration
                TrackSelectedPackages(options);

                // Phase 2: Configure all installed packages
                var configProgress = new Progress<string>(message => 
                    ReportProgress(message, GetProgressPercentage(), "Package Configuration"));
                await installationCoordinator.ExecuteConfigurationAsync(_selectedPackages.ToArray(), configProgress, cancellationToken);

                // Clean up downloads folder
                await CleanupDownloadsFolderAsync(options.InstallPath);

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
                ResetProgressTracking();
                ReportProgress("Installation was cancelled by user", 0, "Cancelled");
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

        private int GetDetailedProgressPercentage()
        {
            // Base progress from completed steps
            int baseProgress = (_currentStep * 100) / _totalSteps;
            
            // Add package progress if in installation phase
            if (_totalPackages > 0)
            {
                // Calculate the weight of each package in the installation phase
                // Installation phase is roughly 60% of total progress (packages + configuration)
                double packageWeight = 60.0 / _totalPackages;
                
                // Progress from completed packages
                double completedPackagesProgress = _currentPackageIndex * packageWeight;
                
                // Progress from current package (if not all packages are done)
                double currentPackageContribution = 0;
                if (_currentPackageIndex < _totalPackages)
                {
                    currentPackageContribution = (_currentPackageProgress * packageWeight) / 100.0;
                }
                
                double totalPackageProgress = completedPackagesProgress + currentPackageContribution;
                
                return (int)Math.Min(100, baseProgress + totalPackageProgress);
            }
            
            return baseProgress;
        }

        private int CountSelectedPackages(InstallOptions options)
        {
            int count = 0;
            if (options.InstallApache) count++;
            if (options.InstallMariaDB) count++;
            if (options.InstallPHP) count++;
            if (options.InstallPhpMyAdmin) count++;
            return count;
        }

        private void SubscribeToPackageManagerEvents()
        {
            // Subscribe to download progress events
            _packageManager.DownloadProgressReported += (sender, e) =>
            {
                _currentPackageProgress = e.PercentComplete;
                var message = $"Downloading {e.PackageName}: {e.Status}";
                if (e.PercentComplete > 0)
                {
                    message += $" ({e.PercentComplete:F1}%)";
                }
                ReportProgress(message, GetDetailedProgressPercentage(), "Package Installation");
            };

            // Subscribe to extraction progress events
            _packageManager.ExtractionProgressReported += (sender, e) =>
            {
                _currentPackageProgress = e.PercentComplete;
                var message = $"Extracting {e.PackageName}: {e.CurrentOperation}";
                if (e.PercentComplete > 0)
                {
                    message += $" ({e.PercentComplete}%)";
                }
                ReportProgress(message, GetDetailedProgressPercentage(), "Package Installation");
            };

            // Subscribe to package completion events
            _packageManager.PackageInstallationCompleted += (sender, e) =>
            {
                _currentPackageIndex++;
                _currentPackageProgress = 0;
                ReportProgress($"Package {e.PackageName} installation completed", GetDetailedProgressPercentage(), "Package Installation");
            };
        }

        private void ResetProgressTracking()
        {
            _currentStep = 0;
            _currentPackageIndex = 0;
            _currentPackageProgress = 0;
            _totalPackages = 0;
            _totalSteps = 0;
        }

        private void ReportProgress(string message, int percentComplete = 0, string currentStep = "")
        {
            ProgressChanged?.Invoke(this, new InstallProgressEventArgs(message, percentComplete, currentStep));
        }

        private void ReportError(string message, Exception exception = null, string component = "")
        {
            ErrorOccurred?.Invoke(this, new InstallErrorEventArgs(message, exception, component));
        }

        private async Task CleanupDownloadsFolderAsync(string installPath)
        {
            var downloadsPath = Path.Combine(installPath, "downloads");
            
            if (Directory.Exists(downloadsPath))
            {
                try
                {
                    ReportProgress("Cleaning up downloads folder...", GetProgressPercentage(), "Cleanup");
                    
                    // Delete all files and subdirectories in the downloads folder
                    var files = Directory.GetFiles(downloadsPath, "*", SearchOption.AllDirectories);
                    var directories = Directory.GetDirectories(downloadsPath, "*", SearchOption.AllDirectories);
                    
                    // Delete all files
                    foreach (var file in files)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch
                        {
                            // Ignore individual file deletion errors
                        }
                    }
                    
                    // Delete all directories (in reverse order to handle nested directories)
                    Array.Reverse(directories);
                    foreach (var directory in directories)
                    {
                        try
                        {
                            Directory.Delete(directory, false);
                        }
                        catch
                        {
                            // Ignore individual directory deletion errors
                        }
                    }
                    
                    // Finally, delete the downloads folder itself
                    Directory.Delete(downloadsPath, false);
                    
                    ReportProgress("Downloads folder cleaned up successfully", GetProgressPercentage(), "Cleanup");
                }
                catch (Exception ex)
                {
                    // Log the error but don't fail the installation
                    ReportProgress($"Warning: Could not fully clean up downloads folder: {ex.Message}", GetProgressPercentage(), "Cleanup");
                }
            }
        }

        private async Task<string[]> CheckForExistingPackagesAsync(string installPath, string[] selectedPackages)
        {
            var existingPackages = new List<string>();
            var appsDirectory = Path.Combine(installPath, "apps");
            
            if (!Directory.Exists(appsDirectory))
            {
                return existingPackages.ToArray();
            }
            
            foreach (var packageName in selectedPackages)
            {
                var packagePath = Path.Combine(appsDirectory, packageName);
                if (Directory.Exists(packagePath))
                {
                    // Check if the package has its key binary files to confirm it's actually installed
                    var packageExists = await FileHelper.ValidatePackagePrerequisitesAsync(installPath, packageName);
                    if (packageExists)
                    {
                        existingPackages.Add(packageName);
                    }
                }
            }
            
            return existingPackages.ToArray();
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