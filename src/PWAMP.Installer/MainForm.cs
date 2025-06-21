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
using PWAMP.Installer.UI;

namespace PWAMP.Installer
{
    public partial class MainForm : Form
    {
        private readonly InstallerManager _installerManager;
        private List<InstallablePackage> _availablePackages;
        private List<InstallablePackage> _selectedPackages;
        private CancellationTokenSource _cancellationTokenSource;
        private int _currentStep = 0;

        private Panel _welcomePanel;
        private Panel _locationPanel;
        private Panel _selectionPanel;
        private Panel _progressPanel;
        private Panel _completionPanel;

        private Button _nextButton;
        private Button _backButton;
        private Button _exitButton;

        private CheckedListBox _packageListBox;
        private ProgressBar _overallProgressBar;
        private ProgressBar _currentProgressBar;
        private Label _statusLabel;
        private Label _currentOperationLabel;
        private RichTextBox _logTextBox;

        private TextBox _installPathTextBox;
        private Button _browseButton;
        private string _installationPath = @"C:\PWAMP";

        public MainForm()
        {
            Text = "PWAMP Server Installer";
            _installerManager = new InstallerManager();
            _selectedPackages = new List<InstallablePackage>();
            _cancellationTokenSource = new CancellationTokenSource();

            InitializeComponent();
            SetupUI();
            SetupEventHandlers();
            ShowWelcomeStep();
        }

        private void SetupUI()
        {
            Size = new Size(600, 500);
            StartPosition = FormStartPosition.Manual;
            CenterToScreen();
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            SizeGripStyle = SizeGripStyle.Show;

            CreatePanels();
            CreateButtons();
            LayoutControls();
        }

        private void CreatePanels()
        {
            _welcomePanel = new Panel { Dock = DockStyle.Fill, Visible = true };
            _locationPanel = new Panel { Dock = DockStyle.Fill, Visible = false };
            _selectionPanel = new Panel { Dock = DockStyle.Fill, Visible = false };
            _progressPanel = new Panel { Dock = DockStyle.Fill, Visible = false };
            _completionPanel = new Panel { Dock = DockStyle.Fill, Visible = false };

            CreateWelcomePanel();
            CreateLocationPanel();
            CreateSelectionPanel();
            CreateProgressPanel();
            CreateCompletionPanel();

            this.Controls.Add(_welcomePanel);
            this.Controls.Add(_locationPanel);
            this.Controls.Add(_selectionPanel);
            this.Controls.Add(_progressPanel);
            this.Controls.Add(_completionPanel);
        }

        private void CreateWelcomePanel()
        {
            var titleLabel = new Label
            {
                Text = "Welcome to PWAMP Server Installer",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
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

        private void CreateLocationPanel()
        {
            var titleLabel = new Label
            {
                Text = "Select Installation Location",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(300, 25)
            };

            var descriptionLabel = new Label
            {
                Text = "Choose where to install PWAMP Server components. The installer will create the necessary subdirectories.",
                Location = new Point(20, 60),
                Size = new Size(520, 40),
                AutoSize = false
            };

            var pathLabel = new Label
            {
                Text = "Installation Path:",
                Location = new Point(20, 120),
                Size = new Size(100, 20)
            };

            _installPathTextBox = new TextBox
            {
                Location = new Point(20, 145),
                Size = new Size(420, 25),
                Text = _installationPath,
                Font = new Font("Segoe UI", 9)
            };

            _browseButton = new Button
            {
                Text = "Browse...",
                Location = new Point(450, 143),
                Size = new Size(80, 29),
                BackColor = Color.FromArgb(108, 117, 125),
                FlatAppearance = { BorderColor = Color.FromArgb(108, 117, 125) },
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            var warningLabel = new Label
            {
                Text = "Note: Make sure you have write permissions to the selected directory.",
                Location = new Point(20, 190),
                Size = new Size(520, 20),
                ForeColor = Color.Orange,
                Font = new Font("Segoe UI", 9, FontStyle.Italic)
            };

            var spaceLabel = new Label
            {
                Text = "Required disk space: ~500 MB",
                Location = new Point(20, 220),
                Size = new Size(520, 20),
                ForeColor = Color.Blue
            };

            _browseButton.Click += BrowseButton_Click;
            _installPathTextBox.TextChanged += InstallPathTextBox_TextChanged;

            _locationPanel.Controls.AddRange(new Control[] { 
                titleLabel, descriptionLabel, pathLabel, _installPathTextBox, _browseButton, warningLabel, spaceLabel 
            });
        }

        private void CreateSelectionPanel()
        {
            var titleLabel = new Label
            {
                Text = "Select Components to Install",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
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
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
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
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
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
                Size = new Size(75, 30),
                Location = new Point(350, 10),
                BackColor = Color.FromArgb(0, 123, 255),
                FlatAppearance = { BorderColor = Color.FromArgb(0, 123, 255) },
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            _backButton = new Button
            {
                Text = "< Back",
                Size = new Size(75, 30),
                Location = new Point(265, 10),
                BackColor = Color.FromArgb(108, 117, 125),
                FlatAppearance = { BorderColor = Color.FromArgb(108, 117, 125) },
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false,
                Enabled = false
            };

            _exitButton = new Button
            {
                Text = "Exit",
                Size = new Size(75, 30),
                Location = new Point(450, 10),
                BackColor = Color.FromArgb(205, 73, 73),
                FlatAppearance = { BorderColor = Color.FromArgb(220, 53, 69) },
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            buttonPanel.Controls.AddRange(new Control[] { _nextButton, _backButton, _exitButton });
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
                _welcomePanel, _locationPanel, _selectionPanel, _progressPanel, _completionPanel
            });

            this.Controls.Add(contentPanel);
        }

        private void SetupEventHandlers()
        {
            _nextButton.Click += NextButton_Click;
            _backButton.Click += BackButton_Click;
            _exitButton.Click += ExitButton_Click;

            _installerManager.DownloadProgressReported += OnDownloadProgress;
            _installerManager.InstallationProgressReported += OnInstallationProgress;
            _installerManager.ErrorOccurred += OnError;
            _installerManager.InstallationCompleted += OnInstallationCompleted;

            this.FormClosing += MainForm_FormClosing;
        }

        private async void NextButton_Click(object sender, EventArgs e)
        {
            switch (_currentStep)
            {
                case 0:
                    ShowLocationStep();
                    break;
                case 1:
                    if (ValidateInstallationPath())
                    {
                        await LoadAvailablePackages();
                        ShowSelectionStep();
                    }
                    break;
                case 2:
                    if (ValidateSelection())
                    {
                        ShowProgressStep();
                        await StartInstallation();
                    }
                    break;
                case 3:
                    break;
                case 4:
                    this.Close();
                    break;
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            switch (_currentStep)
            {
                case 1:
                    ShowWelcomeStep();
                    break;
                case 2:
                    ShowLocationStep();
                    break;
                case 3:
                    if (_overallProgressBar.Value == 0)
                        ShowSelectionStep();
                    break;
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            if (_currentStep == 3 && _overallProgressBar.Value > 0)
            {
                var result = MessageBox.Show("Installation is in progress. Are you sure you want to exit?", 
                    "Exit Installer", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    _cancellationTokenSource.Cancel();
                    this.Close();
                }
            }
            else
            {
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
                    var displayText = $"{package.Name} {package.Version} - {package.GetSizeText()}";
                    _packageListBox.Items.Add(displayText, package.Name.Contains("Apache") || package.Name.Contains("MariaDB"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load available packages: {ex.Message}", "Error", 
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
                    _nextButton.BackColor = Color.FromArgb(255, 193, 7);
                    _nextButton.FlatAppearance.BorderColor = Color.FromArgb(255, 193, 7);
                    _nextButton.ForeColor = Color.Black;
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
                AddLog($"Installation failed: {ex.Message}");
                MessageBox.Show($"Installation failed: {ex.Message}", "Installation Error", 
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
            _nextButton.BackColor = Color.FromArgb(0, 123, 255);
            _nextButton.FlatAppearance.BorderColor = Color.FromArgb(0, 123, 255);
            _nextButton.ForeColor = Color.White;
            _nextButton.Enabled = true;
            _backButton.Enabled = false;
        }

        private void ShowLocationStep()
        {
            _currentStep = 1;
            _welcomePanel.Visible = false;
            _locationPanel.Visible = true;
            _selectionPanel.Visible = false;
            _progressPanel.Visible = false;
            _completionPanel.Visible = false;

            _nextButton.Text = "Next >";
            _nextButton.BackColor = Color.FromArgb(0, 123, 255);
            _nextButton.FlatAppearance.BorderColor = Color.FromArgb(0, 123, 255);
            _nextButton.ForeColor = Color.White;
            _nextButton.Enabled = true;
            _backButton.Enabled = true;
        }

        private void ShowSelectionStep()
        {
            _currentStep = 2;
            _welcomePanel.Visible = false;
            _locationPanel.Visible = false;
            _selectionPanel.Visible = true;
            _progressPanel.Visible = false;
            _completionPanel.Visible = false;

            _nextButton.Text = "Install";
            _nextButton.BackColor = Color.FromArgb(78, 150, 85);
            _nextButton.FlatAppearance.BorderColor = Color.FromArgb(40, 167, 69);
            _nextButton.Enabled = true;
            _backButton.Enabled = true;
        }

        private void ShowProgressStep()
        {
            _currentStep = 3;
            _welcomePanel.Visible = false;
            _locationPanel.Visible = false;
            _selectionPanel.Visible = false;
            _progressPanel.Visible = true;
            _completionPanel.Visible = false;

            _nextButton.Enabled = false;
            _backButton.Enabled = false;
        }

        private void ShowCompletionStep()
        {
            _currentStep = 4;
            _welcomePanel.Visible = false;
            _locationPanel.Visible = false;
            _selectionPanel.Visible = false;
            _progressPanel.Visible = false;
            _completionPanel.Visible = true;

            _nextButton.Text = "Finish";
            _nextButton.BackColor = Color.FromArgb(0, 123, 255);
            _nextButton.FlatAppearance.BorderColor = Color.FromArgb(0, 123, 255);
            _nextButton.Enabled = true;
            _backButton.Enabled = false;
            _exitButton.Text = "Close";
        }

        private void OnDownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnDownloadProgress(sender, e)));
                return;
            }

            _currentProgressBar.Value = (int)Math.Min(100, e.PercentComplete);
            _statusLabel.Text = $"Downloading {e.PackageName}";
            _currentOperationLabel.Text = e.Status ?? "";
            
            AddLog($"Download progress: {e.PackageName} - {e.PercentComplete:F1}%");
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
            
            AddLog($"Installation: {e.CurrentOperation}");
        }

        private void OnError(object sender, InstallerErrorEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => OnError(sender, e)));
                return;
            }

            AddLog($"ERROR: {e.Message}", Color.Red);
            
            if (e.IsFatal)
            {
                MessageBox.Show($"Fatal error occurred: {e.Message}", "Installation Error", 
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
            AddLog($"Package {e.PackageName}: {e.Message}", color);
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
            _logTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            _logTextBox.SelectionColor = _logTextBox.ForeColor;
            _logTextBox.ScrollToCaret();

            // Limit log size
            if (_logTextBox.Lines.Length > 1000)
            {
                var lines = _logTextBox.Lines;
                var newLines = new string[500];
                Array.Copy(lines, lines.Length - 500, newLines, 0, 500);
                _logTextBox.Lines = newLines;
            }
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select installation directory for PWAMP Server";
                folderDialog.SelectedPath = _installationPath;
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    _installationPath = folderDialog.SelectedPath;
                    _installPathTextBox.Text = _installationPath;
                }
            }
        }

        private void InstallPathTextBox_TextChanged(object sender, EventArgs e)
        {
            _installationPath = _installPathTextBox.Text;
        }

        private bool ValidateInstallationPath()
        {
            if (string.IsNullOrWhiteSpace(_installationPath))
            {
                MessageBox.Show("Please select an installation directory.", "Invalid Path", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                if (!System.IO.Directory.Exists(_installationPath))
                {
                    var result = MessageBox.Show($"The directory '{_installationPath}' does not exist. Would you like to create it?", 
                        "Create Directory", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        System.IO.Directory.CreateDirectory(_installationPath);
                    }
                    else
                    {
                        return false;
                    }
                }

                var testFile = System.IO.Path.Combine(_installationPath, "write_test.tmp");
                System.IO.File.WriteAllText(testFile, "test");
                System.IO.File.Delete(testFile);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("You don't have write permissions to the selected directory. Please choose a different location or run the installer as administrator.", 
                    "Permission Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid installation path: {ex.Message}", "Invalid Path", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public string InstallationPath => _installationPath;

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_currentStep == 3 && _overallProgressBar.Value > 0 && _overallProgressBar.Value < 100)
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
