using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PWAMP.Installer.Core;
using PWAMP.Installer.Events;
using PWAMP.Installer.Models;

namespace PWAMP.Installer.UI
{
    public partial class InstallerWizardForm : Form
    {
        private readonly InstallerManager _installerManager;
        private List<InstallablePackage> _availablePackages;
        private List<InstallablePackage> _selectedPackages;
        private CancellationTokenSource _cancellationTokenSource;
        private int _currentStep = 0;

        private Panel _welcomePanel;
        private Panel _selectionPanel;
        private Panel _progressPanel;
        private Panel _completionPanel;

        private Button _nextButton;
        private Button _backButton;
        private Button _cancelButton;

        private CheckedListBox _packageListBox;
        private ProgressBar _overallProgressBar;
        private ProgressBar _currentProgressBar;
        private Label _statusLabel;
        private Label _currentOperationLabel;
        private RichTextBox _logTextBox;

        public InstallerWizardForm()
        {
            _installerManager = new InstallerManager();
            _selectedPackages = new List<InstallablePackage>();
            _cancellationTokenSource = new CancellationTokenSource();

            InitializeComponent();
            SetupEventHandlers();
            ShowWelcomeStep();
        }

        private void InitializeComponent()
        {
            this.Text = "PWAMP Server Installer";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            CreatePanels();
            CreateButtons();
            LayoutControls();
        }

        private void CreatePanels()
        {
            _welcomePanel = new Panel { Dock = DockStyle.Fill, Visible = true };
            _selectionPanel = new Panel { Dock = DockStyle.Fill, Visible = false };
            _progressPanel = new Panel { Dock = DockStyle.Fill, Visible = false };
            _completionPanel = new Panel { Dock = DockStyle.Fill, Visible = false };

            CreateWelcomePanel();
            CreateSelectionPanel();
            CreateProgressPanel();
            CreateCompletionPanel();

            this.Controls.Add(_welcomePanel);
            this.Controls.Add(_selectionPanel);
            this.Controls.Add(_progressPanel);
            this.Controls.Add(_completionPanel);
        }

        private void CreateWelcomePanel()
        {
            var titleLabel = new Label
            {
                Text = "Welcome to PWAMP Server Installer",
                Font = new Font("Microsoft Sans Serif", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(400, 30)
            };

            var descriptionLabel = new Label
            {
                Text = "This wizard will help you install Apache HTTP Server, MariaDB, PHP, and phpMyAdmin components for your PWAMP development environment.\n\nClick Next to continue.",
                Location = new Point(20, 70),
                Size = new Size(520, 100),
                AutoSize = false
            };

            var requirementsLabel = new Label
            {
                Text = "System Requirements:\n• Windows 7 or later\n• Administrator privileges\n• Internet connection\n• 500MB free disk space",
                Location = new Point(20, 180),
                Size = new Size(520, 100),
                AutoSize = false
            };

            _welcomePanel.Controls.AddRange(new Control[] { titleLabel, descriptionLabel, requirementsLabel });
        }

        private void CreateSelectionPanel()
        {
            var titleLabel = new Label
            {
                Text = "Select Components to Install",
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(300, 25)
            };

            _packageListBox = new CheckedListBox
            {
                Location = new Point(20, 60),
                Size = new Size(520, 200),
                CheckOnClick = true
            };

            var infoLabel = new Label
            {
                Text = "Dependencies will be automatically selected and installed.",
                Location = new Point(20, 270),
                Size = new Size(520, 20),
                ForeColor = Color.Blue
            };

            _selectionPanel.Controls.AddRange(new Control[] { titleLabel, _packageListBox, infoLabel });
        }

        private void CreateProgressPanel()
        {
            var titleLabel = new Label
            {
                Text = "Installing Components",
                Font = new Font("Microsoft Sans Serif", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(300, 25)
            };

            var overallLabel = new Label
            {
                Text = "Overall Progress:",
                Location = new Point(20, 60),
                Size = new Size(100, 20)
            };

            _overallProgressBar = new ProgressBar
            {
                Location = new Point(20, 85),
                Size = new Size(520, 25),
                Style = ProgressBarStyle.Continuous
            };

            var currentLabel = new Label
            {
                Text = "Current Package:",
                Location = new Point(20, 120),
                Size = new Size(100, 20)
            };

            _currentProgressBar = new ProgressBar
            {
                Location = new Point(20, 145),
                Size = new Size(520, 25),
                Style = ProgressBarStyle.Continuous
            };

            _statusLabel = new Label
            {
                Text = "Initializing...",
                Location = new Point(20, 180),
                Size = new Size(520, 20)
            };

            _currentOperationLabel = new Label
            {
                Text = "",
                Location = new Point(20, 205),
                Size = new Size(520, 20),
                ForeColor = Color.Gray
            };

            _logTextBox = new RichTextBox
            {
                Location = new Point(20, 230),
                Size = new Size(520, 150),
                ScrollBars = RichTextBoxScrollBars.Vertical,
                ReadOnly = true,
                BackColor = Color.White
            };

            _progressPanel.Controls.AddRange(new Control[] {
                titleLabel, overallLabel, _overallProgressBar, currentLabel, _currentProgressBar,
                _statusLabel, _currentOperationLabel, _logTextBox
            });
        }

        private void CreateCompletionPanel()
        {
            var titleLabel = new Label
            {
                Text = "Installation Complete",
                Font = new Font("Microsoft Sans Serif", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(400, 30)
            };

            var messageLabel = new Label
            {
                Text = "The PWAMP server components have been successfully installed.\n\nYou can now use the PWAMP Control Panel to manage your servers.",
                Location = new Point(20, 70),
                Size = new Size(520, 100),
                AutoSize = false
            };

            _completionPanel.Controls.AddRange(new Control[] { titleLabel, messageLabel });
        }

        private void CreateButtons()
        {
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };

            _nextButton = new Button
            {
                Text = "Next >",
                Size = new Size(75, 25),
                Location = new Point(350, 12),
                UseVisualStyleBackColor = true
            };

            _backButton = new Button
            {
                Text = "< Back",
                Size = new Size(75, 25),
                Location = new Point(265, 12),
                UseVisualStyleBackColor = true,
                Enabled = false
            };

            _cancelButton = new Button
            {
                Text = "Cancel",
                Size = new Size(75, 25),
                Location = new Point(450, 12),
                UseVisualStyleBackColor = true
            };

            buttonPanel.Controls.AddRange(new Control[] { _nextButton, _backButton, _cancelButton });
            this.Controls.Add(buttonPanel);
        }

        private void LayoutControls()
        {
            var contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 0, 50)
            };

            contentPanel.Controls.AddRange(new Control[] {
                _welcomePanel, _selectionPanel, _progressPanel, _completionPanel
            });

            this.Controls.Add(contentPanel);
        }

        private void SetupEventHandlers()
        {
            _nextButton.Click += NextButton_Click;
            _backButton.Click += BackButton_Click;
            _cancelButton.Click += CancelButton_Click;

            _installerManager.DownloadProgressReported += OnDownloadProgress;
            _installerManager.InstallationProgressReported += OnInstallationProgress;
            _installerManager.ErrorOccurred += OnError;
            _installerManager.InstallationCompleted += OnInstallationCompleted;

            this.FormClosing += InstallerWizardForm_FormClosing;
        }

        private async void NextButton_Click(object sender, EventArgs e)
        {
            switch (_currentStep)
            {
                case 0: // Welcome -> Selection
                    await LoadAvailablePackages();
                    ShowSelectionStep();
                    break;
                case 1: // Selection -> Progress
                    if (ValidateSelection())
                    {
                        ShowProgressStep();
                        await StartInstallation();
                    }
                    break;
                case 2: // Progress -> Completion (handled automatically)
                    break;
                case 3: // Completion -> Close
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    break;
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            switch (_currentStep)
            {
                case 1: // Selection -> Welcome
                    ShowWelcomeStep();
                    break;
                case 2: // Progress -> Selection (if not started)
                    if (_overallProgressBar.Value == 0)
                        ShowSelectionStep();
                    break;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (_currentStep == 2 && _overallProgressBar.Value > 0)
            {
                var result = MessageBox.Show("Installation is in progress. Are you sure you want to cancel?", 
                    "Cancel Installation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    _cancellationTokenSource.Cancel();
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private async Task LoadAvailablePackages()
        {
            try
            {
                _nextButton.Enabled = false;
                _nextButton.Text = "Loading...";

                _availablePackages = await _installerManager.GetAvailablePackagesAsync();
                
                _packageListBox.Items.Clear();
                foreach (var package in _availablePackages)
                {
                    var displayText = string.Format("{0} {1} - {2}", package.Name, package.Version, package.GetSizeText());
                    _packageListBox.Items.Add(displayText, package.Name.Contains("Apache") || package.Name.Contains("MariaDB"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to load available packages: {0}", ex.Message), "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _nextButton.Enabled = true;
                _nextButton.Text = "Next >";
            }
        }

        private bool ValidateSelection()
        {
            _selectedPackages.Clear();
            for (int i = 0; i < _packageListBox.CheckedItems.Count; i++)
            {
                var checkedIndex = _packageListBox.CheckedIndices[i];
                _selectedPackages.Add(_availablePackages[checkedIndex]);
            }

            if (_selectedPackages.Count == 0)
            {
                MessageBox.Show("Please select at least one component to install.", "No Selection", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private async Task StartInstallation()
        {
            try
            {
                _nextButton.Enabled = false;
                _backButton.Enabled = false;

                AddLog("Starting installation process...");
                
                var success = await _installerManager.InstallPackagesAsync(_selectedPackages, _cancellationTokenSource.Token);
                
                if (success)
                {
                    ShowCompletionStep();
                }
                else
                {
                    AddLog("Installation failed or was cancelled.");
                    _nextButton.Text = "Retry";
                    _nextButton.Enabled = true;
                    _backButton.Enabled = true;
                }
            }
            catch (OperationCanceledException)
            {
                AddLog("Installation was cancelled by user.");
            }
            catch (Exception ex)
            {
                AddLog(string.Format("Installation failed: {0}", ex.Message));
                MessageBox.Show(string.Format("Installation failed: {0}", ex.Message), "Installation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowWelcomeStep()
        {
            _currentStep = 0;
            _welcomePanel.Visible = true;
            _selectionPanel.Visible = false;
            _progressPanel.Visible = false;
            _completionPanel.Visible = false;

            _nextButton.Text = "Next >";
            _nextButton.Enabled = true;
            _backButton.Enabled = false;
        }

        private void ShowSelectionStep()
        {
            _currentStep = 1;
            _welcomePanel.Visible = false;
            _selectionPanel.Visible = true;
            _progressPanel.Visible = false;
            _completionPanel.Visible = false;

            _nextButton.Text = "Install";
            _nextButton.Enabled = true;
            _backButton.Enabled = true;
        }

        private void ShowProgressStep()
        {
            _currentStep = 2;
            _welcomePanel.Visible = false;
            _selectionPanel.Visible = false;
            _progressPanel.Visible = true;
            _completionPanel.Visible = false;

            _nextButton.Enabled = false;
            _backButton.Enabled = false;
        }

        private void ShowCompletionStep()
        {
            _currentStep = 3;
            _welcomePanel.Visible = false;
            _selectionPanel.Visible = false;
            _progressPanel.Visible = false;
            _completionPanel.Visible = true;

            _nextButton.Text = "Finish";
            _nextButton.Enabled = true;
            _backButton.Enabled = false;
            _cancelButton.Text = "Close";
        }

        private void OnDownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnDownloadProgress(sender, e)));
                return;
            }

            _currentProgressBar.Value = (int)Math.Min(100, e.PercentComplete);
            _statusLabel.Text = string.Format("Downloading {0}", e.PackageName);
            _currentOperationLabel.Text = e.Status ?? "";
            
            AddLog(string.Format("Download progress: {0} - {1:F1}%", e.PackageName, e.PercentComplete));
        }

        private void OnInstallationProgress(object sender, InstallationProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnInstallationProgress(sender, e)));
                return;
            }

            if (e.TotalSteps > 0)
            {
                _overallProgressBar.Value = (int)Math.Min(100, (double)e.CompletedSteps / e.TotalSteps * 100);
            }

            _statusLabel.Text = e.PackageName ?? "Processing...";
            _currentOperationLabel.Text = e.CurrentOperation ?? "";
            
            AddLog(string.Format("Installation: {0}", e.CurrentOperation));
        }

        private void OnError(object sender, InstallerErrorEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnError(sender, e)));
                return;
            }

            AddLog(string.Format("ERROR: {0}", e.Message), Color.Red);
            
            if (e.IsFatal)
            {
                MessageBox.Show(string.Format("Fatal error occurred: {0}", e.Message), "Installation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnInstallationCompleted(object sender, InstallationCompletedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnInstallationCompleted(sender, e)));
                return;
            }

            var color = e.Success ? Color.Green : Color.Red;
            AddLog(string.Format("Package {0}: {1}", e.PackageName, e.Message), color);
        }

        private void AddLog(string message, Color? color = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddLog(message, color)));
                return;
            }

            _logTextBox.SelectionStart = _logTextBox.TextLength;
            _logTextBox.SelectionLength = 0;
            _logTextBox.SelectionColor = color ?? Color.Black;
            _logTextBox.AppendText(string.Format("{0:HH:mm:ss} - {1}\n", DateTime.Now, message));
            _logTextBox.SelectionColor = _logTextBox.ForeColor;
            _logTextBox.ScrollToCaret();
        }

        private void InstallerWizardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_currentStep == 2 && _overallProgressBar.Value > 0 && _overallProgressBar.Value < 100)
            {
                var result = MessageBox.Show("Installation is in progress. Are you sure you want to close?", 
                    "Close Installer", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (_cancellationTokenSource != null)
                _cancellationTokenSource.Cancel();
            if (_installerManager != null)
                _installerManager.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_cancellationTokenSource != null)
                    _cancellationTokenSource.Dispose();
                if (_installerManager != null)
                    _installerManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}