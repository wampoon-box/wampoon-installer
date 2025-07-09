using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wampoon.Installer.Core;
using Wampoon.Installer.Core.Events;
using Wampoon.Installer.Core.PackageDiscovery;
using Wampoon.Installer.Models;
using Frostybee.Pwamp.UI;
using PWAMP.Installer.Helpers;

namespace Wampoon.Installer.UI
{
    public partial class MainForm : Form
    {
        private InstallManager _installManager;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isInstalling;
        private PackageDiscoveryService _packageDiscoveryService;
        private PackageRepository _packageRepository;
        private System.Windows.Forms.Timer _progressBarTimer;


        public MainForm()
        {
            InitializeComponent();
            InitializeInstallManager();
        }


        private void InitializeInstallManager()
        {
            _installManager = new InstallManager();
            _installManager.ProgressChanged += InstallManager_ProgressChanged;
            _installManager.ErrorOccurred += InstallManager_ErrorOccurred;
            _installManager.InstallationCompleted += InstallManager_InstallationCompleted;
            _installManager.ExistingPackagesDetected += InstallManager_ExistingPackagesDetected;
            
            _packageRepository = new PackageRepository();
            _packageDiscoveryService = new PackageDiscoveryService(_packageRepository);
            
            // This should now be fast since it loads from local file first
            UpdateComponentVersions();
            
            Text = AppConstants.APP_FULL_NAME;
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select installation directory";
                folderDialog.SelectedPath = _installPathTextBox.Text;
                
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    _installPathTextBox.Text = folderDialog.SelectedPath;
                }
            }
        }

        private async void InstallButton_Click(object sender, EventArgs e)
        {
            if (_isInstalling) return;

            try
            {
                // Validate at least one component is selected
                if (!_apacheCheckBox.Checked && !_mariadbCheckBox.Checked && 
                    !_phpCheckBox.Checked && !_phpmyadminCheckBox.Checked)
                {
                    MessageBox.Show("Please select at least one component to install.", 
                        "No Components Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validate installation path
                if (string.IsNullOrWhiteSpace(_installPathTextBox.Text))
                {
                    MessageBox.Show("Please specify an installation directory.", 
                        "Invalid Path", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _isInstalling = true;
                _cancellationTokenSource = new CancellationTokenSource();
                
                // Update UI state
                _installButton.Enabled = false;
                _cancelButton.Enabled = true;
                _progressBar.Value = 0;
                _logTextBox.Clear();
                
                // Create install options
                var options = new InstallOptions
                {
                    InstallPath = _installPathTextBox.Text,
                    InstallApache = _apacheCheckBox.Checked,
                    InstallMariaDB = _mariadbCheckBox.Checked,
                    InstallPHP = _phpCheckBox.Checked,
                    InstallPhpMyAdmin = _phpmyadminCheckBox.Checked
                };

                // Start installation
                await _installManager.InstallAsync(options, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                LogMessage("Installation was cancelled by user", Color.Yellow);
            }
            catch (Exception ex)
            {
                LogMessage($"Installation failed: {ex.Message}", Color.Red);
                MessageBox.Show($"Installation failed: {ex.Message}", 
                    "Installation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isInstalling = false;
                
                // Reset progress bar on cancellation or error
                _progressBar.Value = 0;
                _progressLabel.Text = "Ready to install";
                
                // Only reset install button text if installation didn't complete successfully
                if (_installButton.Text != "Install Again")
                {
                    _installButton.Enabled = true;
                }
                _cancelButton.Enabled = false;
                _cancellationTokenSource?.Dispose();
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (_isInstalling && _cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                LogMessage("Cancelling installation...", Color.Yellow);
            }
        }

        private void ExportLogButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    saveFileDialog.DefaultExt = "txt";
                    saveFileDialog.FileName = $"PWAMP_Install_Log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                    saveFileDialog.Title = "Save Installation Log";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(saveFileDialog.FileName, _logTextBox.Text);
                        MessageBox.Show($"Log exported successfully to:\n{saveFileDialog.FileName}", 
                            "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export log: {ex.Message}", 
                    "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OpenFolderButton_Click(object sender, EventArgs e)
        {
            try
            {
                var installPath = _installPathTextBox.Text;
                
                if (string.IsNullOrWhiteSpace(installPath))
                {
                    MessageBox.Show("Please specify an installation directory first.", 
                        "No Path Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                if (Directory.Exists(installPath))
                {
                    Process.Start("explorer.exe", installPath);
                }
                else
                {
                    MessageBox.Show($"Directory does not exist: {installPath}", 
                        "Directory Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open directory: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            // Close the application
            this.Close();
        }

        private void AboutButton_Click(object sender, EventArgs e)
        {
            var aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        private void InstallManager_ProgressChanged(object sender, InstallProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => InstallManager_ProgressChanged(sender, e)));
                return;
            }

            _progressLabel.Text = e.CurrentStep;
            var progressValue = Math.Min(e.PercentComplete, 100);
            _progressBar.Value = progressValue;
            
            /*
            // Debug: Log progress updates, especially near completion
            if (e.PercentComplete >= 90)
            {
                LogMessage($"DEBUG: Progress update - {e.PercentComplete}% (setting bar to {progressValue}%)", Color.Cyan);
            }*/
            
            // Use green color for success messages (those with checkmark).
            Color messageColor = e.Message.StartsWith("✓") ? Color.Green : Color.White;
            LogMessage(e.Message, messageColor);
        }

        private void InstallManager_ErrorOccurred(object sender, InstallErrorEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => InstallManager_ErrorOccurred(sender, e)));
                return;
            }

            LogMessage(e.ErrorMessage, Color.Red);
        }

        private void InstallManager_InstallationCompleted(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => InstallManager_InstallationCompleted(sender, e)));
                return;
            }

            // MULTIPLE HACKS: Fix WinForms ProgressBar rendering bug
            try
            {
                // Method 1: Force refresh
                _progressBar.Value = 99;
                _progressBar.Refresh();
                _progressBar.Value = 100;
                _progressBar.Refresh();
                
                // Method 2: Style change
                _progressBar.Style = ProgressBarStyle.Continuous;
                
                // Method 3: Force invalidate and update
                _progressBar.Invalidate();
                _progressBar.Update();
                
                // Method 4: SetWindowPos hack to force redraw
                this.Refresh();
                Application.DoEvents();
                
                // Method 5: Delayed final update
                _progressBarTimer?.Stop();
                _progressBarTimer?.Dispose();
                _progressBarTimer = new System.Windows.Forms.Timer();
                _progressBarTimer.Interval = 100; // 100ms delay
                _progressBarTimer.Tick += (s, args) =>
                {
                    _progressBarTimer.Stop();
                    _progressBar.Value = 100;
                    _progressBar.Refresh();
                    LogMessage("Final progress bar update applied", Color.Cyan);
                };
                _progressBarTimer.Start();
            }
            catch (Exception ex)
            {
                LogMessage($"Progress bar update error: {ex.Message}", Color.Red);
            }
            _progressLabel.Text = "Installation Completed Successfully";
            
            // Update button states for completion
            _installButton.Enabled = true;
            _installButton.Text = "Install Again";
            _cancelButton.Enabled = false;
            
            // Debug: Log that we're showing the quit button
            LogMessage("Quit button should now be visible", Color.Yellow);
            
            LogMessage("Installation completed successfully!", Color.Green);
            MessageBox.Show("PWAMP installation completed successfully!", 
                "Installation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LogMessage(string message, Color color)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            _logTextBox.SelectionStart = _logTextBox.TextLength;
            _logTextBox.SelectionLength = 0;
            _logTextBox.SelectionColor = Color.Gray;
            _logTextBox.AppendText($"[{timestamp}] ");
            _logTextBox.SelectionColor = color;
            _logTextBox.AppendText($"{message}\n");
            _logTextBox.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_isInstalling)
            {
                var result = MessageBox.Show(
                    "Installation is in progress. Are you sure you want to exit?",
                    "Installation in Progress",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

                _cancellationTokenSource?.Cancel();
            }

            // Cleanup resources
            _cancellationTokenSource?.Dispose();
            
            // Unsubscribe from events to prevent memory leaks
            if (_installManager != null)
            {
                _installManager.ProgressChanged -= InstallManager_ProgressChanged;
                _installManager.ErrorOccurred -= InstallManager_ErrorOccurred;
                _installManager.InstallationCompleted -= InstallManager_InstallationCompleted;
                _installManager.ExistingPackagesDetected -= InstallManager_ExistingPackagesDetected;
                _installManager.Dispose();
            }
            
            // Dispose timer
            _progressBarTimer?.Stop();
            _progressBarTimer?.Dispose();
            
            // Dispose package repository
            _packageRepository?.Dispose();

            base.OnFormClosing(e);
        }

        private void InstallManager_ExistingPackagesDetected(object sender, ExistingPackagesEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => InstallManager_ExistingPackagesDetected(sender, e)));
                return;
            }

            var packageList = string.Join(", ", e.ExistingPackages);
            var message = $"The following packages are already installed in the target directory:\n\n{packageList}\n\nDo you want to overwrite the existing packages and continue with the installation?";
            
            var result = MessageBox.Show(
                message,
                "Existing Packages Found",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                e.OverwriteRequested = true;
                LogMessage($"User confirmed to overwrite existing packages: {packageList}", Color.Yellow);
            }
            else
            {
                LogMessage("Installation cancelled by user due to existing packages", Color.Yellow);
            }
        }

        private void SetDefaultComponentText()
        {
            // Set default text immediately (non-blocking)
            _apacheCheckBox.Text = "🌐 Apache HTTP Server";
            _mariadbCheckBox.Text = "🗄️ MariaDB Database Server";
            _phpCheckBox.Text = "🐘 PHP Scripting Language";
            _phpmyadminCheckBox.Text = "🔧 phpMyAdmin Database Manager";
        }

        private async void UpdateComponentVersions()
        {
            try
            {
                // Add a timeout to prevent hanging on startup
                var timeout = TimeSpan.FromSeconds(10);
                var loadVersionsTask = Task.Run(async () =>
                {
                    // Run all package lookups in parallel for better performance
                    var tasks = new[]
                    {
                        _packageDiscoveryService.GetPackageByNameAsync(PackageNames.Apache),
                        _packageDiscoveryService.GetPackageByNameAsync(PackageNames.MariaDB),
                        _packageDiscoveryService.GetPackageByNameAsync(PackageNames.PHP),
                        _packageDiscoveryService.GetPackageByNameAsync(PackageNames.PhpMyAdmin)
                    };

                    return await Task.WhenAll(tasks);
                });

                var timeoutTask = Task.Delay(timeout);
                var completedTask = await Task.WhenAny(loadVersionsTask, timeoutTask);

                if (completedTask == timeoutTask)
                {
                    // Timeout occurred
                    System.Diagnostics.Debug.WriteLine("Version loading timed out - using default component text");
                    return;
                }

                var packages = await loadVersionsTask;
                var apachePackage = packages[0];
                var mariadbPackage = packages[1];
                var phpPackage = packages[2];
                var phpmyadminPackage = packages[3];

                // Update UI on the main thread
                if (InvokeRequired)
                {
                    Invoke(new Action(() => UpdateComponentTextWithVersions(apachePackage, mariadbPackage, phpPackage, phpmyadminPackage)));
                }
                else
                {
                    UpdateComponentTextWithVersions(apachePackage, mariadbPackage, phpPackage, phpmyadminPackage);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't show to user as this is not critical
                System.Diagnostics.Debug.WriteLine($"Error updating component versions: {ex.Message}");
                // UI will keep the default text if version loading fails
            }
        }

        private void UpdateComponentTextWithVersions(InstallablePackage apachePackage, InstallablePackage mariadbPackage, InstallablePackage phpPackage, InstallablePackage phpmyadminPackage)
        {
            if (apachePackage != null)
                _apacheCheckBox.Text = $"🌐 Apache HTTP Server (v{apachePackage.Version})";
                
            if (mariadbPackage != null)
                _mariadbCheckBox.Text = $"🗄️ MariaDB Database Server (v{mariadbPackage.Version})";
                
            if (phpPackage != null)
                _phpCheckBox.Text = $"🐘 PHP Scripting Language (v{phpPackage.Version})";
                
            if (phpmyadminPackage != null)
                _phpmyadminCheckBox.Text = $"🔧 phpMyAdmin Database Manager (v{phpmyadminPackage.Version})";
        }
    }
}