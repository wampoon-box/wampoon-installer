using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PWAMP.Installer.Neo.Core;
using PWAMP.Installer.Neo.Core.Events;

namespace PWAMP.Installer.Neo.UI
{
    public partial class MainForm : Form
    {
        private InstallManager _installManager;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isInstalling;


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

        private void QuitButton_Click(object sender, EventArgs e)
        {
            // Close the application
            this.Close();
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
            
            // Debug: Log progress updates, especially near completion
            if (e.PercentComplete >= 90)
            {
                LogMessage($"DEBUG: Progress update - {e.PercentComplete}% (setting bar to {progressValue}%)", Color.Cyan);
            }
            
            // Use green color for success messages (those with checkmark)
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
                var timer = new System.Windows.Forms.Timer();
                timer.Interval = 100; // 100ms delay
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    _progressBar.Value = 100;
                    _progressBar.Refresh();
                    LogMessage("Final progress bar update applied", Color.Cyan);
                };
                timer.Start();
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
            _installManager?.Dispose();

            base.OnFormClosing(e);
        }
    }
}