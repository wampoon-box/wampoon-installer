using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PWAMPPConsole
{
    public partial class MainForm : Form
    {
        private string baseDir;
        private string downloadUrl = "https://www.apachelounge.com/download/VS17/binaries/httpd-2.4.63-250207-win64-VS17.zip";
        private string zipFilePath;
        private string extractPath;
        private string apachePath;
        private BackgroundWorker worker;

        public MainForm()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
        }

        private void InitializeComponent()
        {
            this.txtInstallLocation = new TextBox();
            this.btnBrowse = new Button();
            this.lblInstallLocation = new Label();
            this.txtPort = new TextBox();
            this.lblPort = new Label();
            this.progressBar = new ProgressBar();
            this.lblStatus = new Label();
            this.btnInstall = new Button();
            this.chkCreateShortcuts = new CheckBox();
            this.lblTitle = new Label();
            this.lblVersion = new Label();

            // MainForm
            this.ClientSize = new Size(550, 350);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Apache Portable Installer";
            this.Font = new Font("Segoe UI", 9F);

            // Title Label
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblTitle.Location = new Point(20, 20);
            this.lblTitle.Text = "Apache Portable Installer";
            this.Controls.Add(this.lblTitle);

            // Version Label
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new Point(22, 50);
            this.lblVersion.Text = "This will install Apache HTTP Server 2.4.58 as a portable application";
            this.Controls.Add(this.lblVersion);

            // Install Location Label
            this.lblInstallLocation.AutoSize = true;
            this.lblInstallLocation.Location = new Point(20, 90);
            this.lblInstallLocation.Text = "Installation Location:";
            this.Controls.Add(this.lblInstallLocation);

            // Install Location TextBox
            this.txtInstallLocation.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.txtInstallLocation.Location = new Point(20, 110);
            this.txtInstallLocation.Size = new Size(420, 23);
            this.txtInstallLocation.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "ApachePortable");
            this.Controls.Add(this.txtInstallLocation);

            // Browse Button
            this.btnBrowse.Location = new Point(450, 110);
            this.btnBrowse.Size = new Size(80, 23);
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new EventHandler(this.btnBrowse_Click);
            this.Controls.Add(this.btnBrowse);

            // Port Label
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new Point(20, 150);
            this.lblPort.Text = "HTTP Port:";
            this.Controls.Add(this.lblPort);

            // Port TextBox
            this.txtPort.Location = new Point(20, 170);
            this.txtPort.Size = new Size(100, 23);
            this.txtPort.Text = "80";
            this.Controls.Add(this.txtPort);

            // Create Shortcuts Checkbox
            this.chkCreateShortcuts.AutoSize = true;
            this.chkCreateShortcuts.Location = new Point(20, 210);
            this.chkCreateShortcuts.Text = "Create desktop shortcuts";
            this.chkCreateShortcuts.Checked = true;
            this.Controls.Add(this.chkCreateShortcuts);

            // Progress Bar
            this.progressBar.Location = new Point(20, 250);
            this.progressBar.Size = new Size(510, 23);
            this.progressBar.Visible = false;
            this.Controls.Add(this.progressBar);

            // Status Label
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new Point(20, 280);
            this.lblStatus.Size = new Size(510, 20);
            this.lblStatus.Text = "Ready to install";
            this.Controls.Add(this.lblStatus);

            // Install Button
            this.btnInstall.Location = new Point(425, 300);
            this.btnInstall.Size = new Size(105, 30);
            this.btnInstall.Text = "Install";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new EventHandler(this.btnInstall_Click);
            this.Controls.Add(this.btnInstall);
        }

        private void InitializeBackgroundWorker()
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select installation directory";
                //folderDialog.UseDescriptionForTitle = true;
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtInstallLocation.Text = Path.Combine(folderDialog.SelectedPath, "ApachePortable");
                }
            }
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInstallLocation.Text))
            {
                MessageBox.Show("Please select an installation location.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int port;
            if (!int.TryParse(txtPort.Text, out port) || port < 1 || port > 65535)
            {
                MessageBox.Show("Please enter a valid port number (1-65535).", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            baseDir = txtInstallLocation.Text;
            zipFilePath = Path.Combine(baseDir, "apache.zip");
            extractPath = Path.Combine(baseDir, "extract");
            apachePath = Path.Combine(baseDir, "Apache24");

            // Disable controls during installation
            txtInstallLocation.Enabled = false;
            btnBrowse.Enabled = false;
            txtPort.Enabled = false;
            chkCreateShortcuts.Enabled = false;
            btnInstall.Enabled = false;

            // Show progress bar
            progressBar.Visible = true;
            progressBar.Value = 0;

            // Start the installation process
            worker.RunWorkerAsync(port);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int port = (int)e.Argument;

            try
            {
                // Step 1: Create directories
                worker.ReportProgress(5, "Creating directories...");
                Directory.CreateDirectory(baseDir);
                Directory.CreateDirectory(extractPath);

                // Step 2: Download Apache
                worker.ReportProgress(10, "Downloading Apache...");
                DownloadFile(downloadUrl, zipFilePath, worker);

                // Step 3: Extract files
                worker.ReportProgress(60, "Extracting Apache...");
                ExtractZipFile(zipFilePath, extractPath);

                // Move Apache24 folder to the base directory
                string extractedApachePath = Path.Combine(extractPath, "Apache24");
                if (Directory.Exists(apachePath))
                {
                    Directory.Delete(apachePath, true);
                }
                Directory.Move(extractedApachePath, apachePath);

                // Step 4: Make Apache portable
                worker.ReportProgress(80, "Configuring Apache...");
                MakeApachePortable(apachePath, port);

                // Step 5: Create batch files
                worker.ReportProgress(90, "Creating startup scripts...");
                CreateBatchFiles(baseDir, port);

                // Step 6: Create shortcuts if requested
                if (chkCreateShortcuts.Checked)
                {
                    worker.ReportProgress(95, "Creating shortcuts...");
                    CreateShortcuts(baseDir);
                }

                // Complete the installation
                worker.ReportProgress(100, "Installation complete!");
                e.Result = true;
            }
            catch (Exception ex)
            {
                worker.ReportProgress(0, $"Error: {ex.Message}");
                e.Result = false;
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            lblStatus.Text = e.UserState as string;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result)
            {
                MessageBox.Show(
                    "Apache has been successfully installed!\n\n" +
                    $"You can start it by running start_apache.bat in {baseDir}",
                    "Installation Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                if (MessageBox.Show(
                    "Do you want to start Apache now?",
                    "Start Apache",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Process.Start(Path.Combine(baseDir, "start_apache.bat"));
                }

                // Close the installer
                this.Close();
            }
            else
            {
                // Re-enable controls if installation failed
                txtInstallLocation.Enabled = true;
                btnBrowse.Enabled = true;
                txtPort.Enabled = true;
                chkCreateShortcuts.Enabled = true;
                btnInstall.Enabled = true;
            }
        }

        private void DownloadFile(string url, string destinationPath, BackgroundWorker worker)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                // Add download progress reporting
                client.DownloadProgressChanged += (s, e) =>
                {
                    int progressPercentage = 10 + (int)(e.ProgressPercentage * 0.5); // Scale to 10-60%
                    worker.ReportProgress(progressPercentage, $"Downloading: {e.ProgressPercentage}%");
                };

                client.DownloadFileCompleted += (s, e) =>
                {
                    if (e.Error != null)
                    {
                        throw e.Error;
                    }
                };

                // Download the file synchronously (the worker is already on a background thread)
                client.DownloadFile(url, destinationPath);
            }
        }

        private void ExtractZipFile(string zipFilePath, string extractPath)
        {
            ZipFile.ExtractToDirectory(zipFilePath, extractPath);
        }

        private void MakeApachePortable(string apachePath, int port)
        {
            // Modify httpd.conf to use relative paths and custom port
            string confPath = Path.Combine(apachePath, "conf", "httpd.conf");
            string confContent = File.ReadAllText(confPath);

            // Replace absolute paths with relative paths
            confContent = Regex.Replace(confContent,
                @"Define SRVROOT\s+""[^""]*""",
                "Define SRVROOT \"${APACHE_DIR}\"");

            // Ensure ServerRoot is set to use the SRVROOT variable
            confContent = Regex.Replace(confContent,
                @"ServerRoot\s+""[^""]*""",
                "ServerRoot \"${SRVROOT}\"");

            // Set custom port if not the default 80
            if (port != 80)
            {
                confContent = Regex.Replace(confContent,
                    @"Listen\s+80",
                    $"Listen {port}");
            }

            // Write the modified config back
            File.WriteAllText(confPath, confContent, Encoding.UTF8);

            // Create .htaccess to allow directory listing
            string htaccessPath = Path.Combine(apachePath, "htdocs", ".htaccess");
            File.WriteAllText(htaccessPath, "Options +Indexes", Encoding.UTF8);

            // Create a test index.html
            string indexPath = Path.Combine(apachePath, "htdocs", "index.html");
            string indexContent = $@"<!DOCTYPE html>
<html>
<head>
    <title>Apache Portable</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; line-height: 1.6; }}
        h1 {{ color: #333; }}
        .container {{ max-width: 800px; margin: 0 auto; }}
        .info {{ background-color: #f5f5f5; padding: 20px; border-radius: 5px; }}
        .success {{ color: green; }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>Apache Portable Server is running! <span class='success'>✓</span></h1>
        <div class='info'>
            <p>If you can see this page, your portable Apache server is working correctly.</p>
            <p>You can place your website files in the 'htdocs' directory.</p>
            <p><strong>Server Information:</strong></p>
            <ul>
                <li>Port: {port}</li>
                <li>Document Root: htdocs folder</li>
                <li>Installation Directory: Portable</li>
            </ul>
            <p>Thank you for using Apache Portable Installer!</p>
        </div>
    </div>
</body>
</html>";
            File.WriteAllText(indexPath, indexContent, Encoding.UTF8);
        }

        private void CreateBatchFiles(string baseDir, int port)
        {
            // Create start_apache.bat
            string startBatchContent = @"@echo off
set CURRENT_DIR=%~dp0
set APACHE_DIR=%CURRENT_DIR%Apache24
echo Starting Apache from %APACHE_DIR%
cd /d ""%APACHE_DIR%\bin""
httpd.exe -k install -n ""ApachePortable""
httpd.exe -k start -n ""ApachePortable""
" + $@"echo Apache started. Access http://localhost:{port}
start http://localhost:{port}
echo Press any key to exit...
pause > nul";
            File.WriteAllText(Path.Combine(baseDir, "start_apache.bat"), startBatchContent);

            // Create stop_apache.bat
            string stopBatchContent = @"@echo off
set CURRENT_DIR=%~dp0
set APACHE_DIR=%CURRENT_DIR%Apache24
echo Stopping Apache...
cd /d ""%APACHE_DIR%\bin""
httpd.exe -k stop -n ""ApachePortable""
httpd.exe -k uninstall -n ""ApachePortable""
echo Apache stopped.
echo Press any key to exit...
pause > nul";
            File.WriteAllText(Path.Combine(baseDir, "stop_apache.bat"), stopBatchContent);
        }

        private void CreateShortcuts(string baseDir)
        {
            // In a real application, you would use IWshRuntimeLibrary or similar to create shortcuts
            // For simplicity, we're just creating .lnk-like batch files that open the actual batch files

            string startLinkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Start Apache Portable.bat");
            string startLinkContent = $@"@echo off
start """" ""{Path.Combine(baseDir, "start_apache.bat")}""";
            File.WriteAllText(startLinkPath, startLinkContent);

            string stopLinkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Stop Apache Portable.bat");
            string stopLinkContent = $@"@echo off
start """" ""{Path.Combine(baseDir, "stop_apache.bat")}""";
            File.WriteAllText(stopLinkPath, stopLinkContent);
        }

        // Visual Studio Designer auto-generated fields
        private TextBox txtInstallLocation;
        private Button btnBrowse;
        private Label lblInstallLocation;
        private TextBox txtPort;
        private Label lblPort;
        private ProgressBar progressBar;
        private Label lblStatus;
        private Button btnInstall;
        private CheckBox chkCreateShortcuts;
        private Label lblTitle;
        private Label lblVersion;
    }
}
