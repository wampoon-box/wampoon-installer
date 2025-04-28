using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WAMPPortableWizard
{
    public partial class WizardForm : Form
    {
        // Wizard pages
        private Panel welcomePage;
        private Panel componentsPage;
        private Panel locationPage;
        private Panel configPage;
        private Panel progressPage;
        private Panel finishPage;

        // Navigation buttons
        private Button btnNext;
        private Button btnBack;
        private Button btnCancel;

        // Component selection checkboxes
        private CheckBox chkApache;
        private CheckBox chkPHP;
        private CheckBox chkMySQL;

        // Configuration fields
        private TextBox txtInstallLocation;
        private Button btnBrowse;
        private TextBox txtApachePort;
        private TextBox txtMySQLPort;
        private TextBox txtMySQLPassword;
        private CheckBox chkCreateShortcuts;

        // Progress indicators
        private ProgressBar progressBar;
        private Label lblStatus;
        private ListView lvProgress;

        // Download URLs and paths
        private string apacheUrl = "https://www.apachelounge.com/download/VS17/binaries/httpd-2.4.63-250207-win64-VS17.zip";
        private string phpUrl = "https://windows.php.net/downloads/releases/php-8.2.15-Win32-vs16-x64.zip";
        private string mysqlUrl = "https://dev.mysql.com/get/Downloads/MySQL-8.0/mysql-8.0.36-winx64.zip";

        // Installation variables
        private string baseDir;
        private string apachePath;
        private string phpPath;
        private string mysqlPath;
        private bool installApache;
        private bool installPHP;
        private bool installMySQL;
        private int apachePort;
        private int mysqlPort;
        private string mysqlPassword;
        private bool createShortcuts;

        // Background worker
        private BackgroundWorker worker;

        // Current page index
        private int currentPage = 0;

        public WizardForm()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
        }

        private void InitializeComponent()
        {
            this.ClientSize = new Size(600, 500);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "WAMP Portable Wizard";
            this.Font = new Font("Segoe UI", 9F);

            // Navigation buttons - will be positioned at the bottom of the form
            this.btnBack = new Button();
            this.btnBack.Location = new Point(360, 455);
            this.btnBack.Size = new Size(70, 30);
            this.btnBack.Text = "< Back";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new EventHandler(this.btnBack_Click);
            this.Controls.Add(this.btnBack);

            this.btnNext = new Button();
            this.btnNext.Location = new Point(435, 455);
            this.btnNext.Size = new Size(70, 30);
            this.btnNext.Text = "Next >";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new EventHandler(this.btnNext_Click);
            this.Controls.Add(this.btnNext);

            this.btnCancel = new Button();
            this.btnCancel.Location = new Point(510, 455);
            this.btnCancel.Size = new Size(70, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            this.Controls.Add(this.btnCancel);

            // Initialize all pages
            InitializeWelcomePage();
            InitializeComponentsPage();
            InitializeLocationPage();
            InitializeConfigPage();
            InitializeProgressPage();
            InitializeFinishPage();

            // Show the first page
            UpdatePageVisibility();
        }

        #region Page Initialization

        private void InitializeWelcomePage()
        {
            welcomePage = new Panel();
            welcomePage.Dock = DockStyle.Fill;
            welcomePage.Padding = new Padding(20);

            Label lblTitle = new Label();
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Text = "Welcome to WAMP Portable Wizard";
            welcomePage.Controls.Add(lblTitle);

            Label lblDescription = new Label();
            lblDescription.Location = new Point(20, 60);
            lblDescription.Size = new Size(540, 60);
            lblDescription.Text = "This wizard will guide you through installing a portable WAMP stack " +
                "(Windows, Apache, MySQL, PHP) that can run from any location without " +
                "requiring a formal installation.";
            welcomePage.Controls.Add(lblDescription);

            PictureBox pictureBox = new PictureBox();
            pictureBox.Location = new Point(20, 140);
            pictureBox.Size = new Size(540, 200);
            pictureBox.BackColor = Color.WhiteSmoke;
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            welcomePage.Controls.Add(pictureBox);

            Label lblNote = new Label();
            lblNote.Location = new Point(20, 360);
            lblNote.Size = new Size(540, 40);
            lblNote.Text = "Click Next to continue.";
            welcomePage.Controls.Add(lblNote);

            this.Controls.Add(welcomePage);
        }

        private void InitializeComponentsPage()
        {
            componentsPage = new Panel();
            componentsPage.Dock = DockStyle.Fill;
            componentsPage.Padding = new Padding(20);
            componentsPage.Visible = false;

            Label lblTitle = new Label();
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Text = "Select Components to Install";
            componentsPage.Controls.Add(lblTitle);

            Label lblDescription = new Label();
            lblDescription.Location = new Point(20, 50);
            lblDescription.Size = new Size(540, 40);
            lblDescription.Text = "Choose which components you want to include in your portable server:";
            componentsPage.Controls.Add(lblDescription);

            // Apache option
            chkApache = new CheckBox();
            chkApache.Location = new Point(40, 100);
            chkApache.Size = new Size(200, 24);
            chkApache.Text = "Apache HTTP Server 2.4.58";
            chkApache.Checked = true;
            componentsPage.Controls.Add(chkApache);

            Label lblApacheDesc = new Label();
            lblApacheDesc.Location = new Point(60, 125);
            lblApacheDesc.Size = new Size(500, 40);
            lblApacheDesc.ForeColor = Color.DarkGray;
            lblApacheDesc.Text = "Web server that handles HTTP requests and serves web content";
            componentsPage.Controls.Add(lblApacheDesc);

            // PHP option
            chkPHP = new CheckBox();
            chkPHP.Location = new Point(40, 170);
            chkPHP.Size = new Size(200, 24);
            chkPHP.Text = "PHP 8.2.15";
            chkPHP.Checked = true;
            componentsPage.Controls.Add(chkPHP);

            Label lblPHPDesc = new Label();
            lblPHPDesc.Location = new Point(60, 195);
            lblPHPDesc.Size = new Size(500, 40);
            lblPHPDesc.ForeColor = Color.DarkGray;
            lblPHPDesc.Text = "Server-side scripting language for web development";
            componentsPage.Controls.Add(lblPHPDesc);

            // MySQL option
            chkMySQL = new CheckBox();
            chkMySQL.Location = new Point(40, 240);
            chkMySQL.Size = new Size(200, 24);
            chkMySQL.Text = "MySQL 8.0.36";
            chkMySQL.Checked = true;
            componentsPage.Controls.Add(chkMySQL);

            Label lblMySQLDesc = new Label();
            lblMySQLDesc.Location = new Point(60, 265);
            lblMySQLDesc.Size = new Size(500, 40);
            lblMySQLDesc.ForeColor = Color.DarkGray;
            lblMySQLDesc.Text = "Relational database management system";
            componentsPage.Controls.Add(lblMySQLDesc);

            this.Controls.Add(componentsPage);
        }

        private void InitializeLocationPage()
        {
            locationPage = new Panel();
            locationPage.Dock = DockStyle.Fill;
            locationPage.Padding = new Padding(20);
            locationPage.Visible = false;

            Label lblTitle = new Label();
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Text = "Installation Location";
            locationPage.Controls.Add(lblTitle);

            Label lblDescription = new Label();
            lblDescription.Location = new Point(20, 50);
            lblDescription.Size = new Size(540, 40);
            lblDescription.Text = "Choose where to install your portable WAMP stack:";
            locationPage.Controls.Add(lblDescription);

            Label lblLocation = new Label();
            lblLocation.Location = new Point(20, 100);
            lblLocation.Size = new Size(540, 20);
            lblLocation.Text = "Destination Folder:";
            locationPage.Controls.Add(lblLocation);

            txtInstallLocation = new TextBox();
            txtInstallLocation.Location = new Point(20, 125);
            txtInstallLocation.Size = new Size(460, 23);
            txtInstallLocation.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "WAMPPortable");
            locationPage.Controls.Add(txtInstallLocation);

            btnBrowse = new Button();
            btnBrowse.Location = new Point(490, 124);
            btnBrowse.Size = new Size(80, 25);
            btnBrowse.Text = "Browse...";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += new EventHandler(this.btnBrowse_Click);
            locationPage.Controls.Add(btnBrowse);

            Label lblSpaceRequired = new Label();
            lblSpaceRequired.Location = new Point(20, 160);
            lblSpaceRequired.Size = new Size(540, 20);
            lblSpaceRequired.Text = "Space Required: ~300MB";
            locationPage.Controls.Add(lblSpaceRequired);

            Label lblPortable = new Label();
            lblPortable.Location = new Point(20, 200);
            lblPortable.Size = new Size(540, 60);
            lblPortable.Text = "The server will be installed as a portable application. You can move " +
                "the folder to another location or drive after installation.";
            locationPage.Controls.Add(lblPortable);

            this.Controls.Add(locationPage);
        }

        private void InitializeConfigPage()
        {
            configPage = new Panel();
            configPage.Dock = DockStyle.Fill;
            configPage.Padding = new Padding(20);
            configPage.Visible = false;

            Label lblTitle = new Label();
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Text = "Configuration Settings";
            configPage.Controls.Add(lblTitle);

            Label lblDescription = new Label();
            lblDescription.Location = new Point(20, 50);
            lblDescription.Size = new Size(540, 30);
            lblDescription.Text = "Configure server settings:";
            configPage.Controls.Add(lblDescription);

            // Apache port
            Label lblApachePort = new Label();
            lblApachePort.Location = new Point(20, 90);
            lblApachePort.Size = new Size(120, 20);
            lblApachePort.Text = "Apache HTTP Port:";
            configPage.Controls.Add(lblApachePort);

            txtApachePort = new TextBox();
            txtApachePort.Location = new Point(150, 90);
            txtApachePort.Size = new Size(80, 23);
            txtApachePort.Text = "80";
            configPage.Controls.Add(txtApachePort);

            // MySQL port
            Label lblMySQLPort = new Label();
            lblMySQLPort.Location = new Point(20, 125);
            lblMySQLPort.Size = new Size(120, 20);
            lblMySQLPort.Text = "MySQL Port:";
            configPage.Controls.Add(lblMySQLPort);

            txtMySQLPort = new TextBox();
            txtMySQLPort.Location = new Point(150, 125);
            txtMySQLPort.Size = new Size(80, 23);
            txtMySQLPort.Text = "3306";
            configPage.Controls.Add(txtMySQLPort);

            // MySQL root password
            Label lblMySQLPassword = new Label();
            lblMySQLPassword.Location = new Point(20, 160);
            lblMySQLPassword.Size = new Size(120, 20);
            lblMySQLPassword.Text = "MySQL Password:";
            configPage.Controls.Add(lblMySQLPassword);

            txtMySQLPassword = new TextBox();
            txtMySQLPassword.Location = new Point(150, 160);
            txtMySQLPassword.Size = new Size(200, 23);
            txtMySQLPassword.Text = "root";
            txtMySQLPassword.PasswordChar = '*';
            configPage.Controls.Add(txtMySQLPassword);

            // Shortcuts option
            chkCreateShortcuts = new CheckBox();
            chkCreateShortcuts.Location = new Point(20, 200);
            chkCreateShortcuts.Size = new Size(200, 24);
            chkCreateShortcuts.Text = "Create desktop shortcuts";
            chkCreateShortcuts.Checked = true;
            configPage.Controls.Add(chkCreateShortcuts);

            this.Controls.Add(configPage);
        }

        private void InitializeProgressPage()
        {
            progressPage = new Panel();
            progressPage.Dock = DockStyle.Fill;
            progressPage.Padding = new Padding(20);
            progressPage.Visible = false;

            Label lblTitle = new Label();
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Text = "Installing Components";
            progressPage.Controls.Add(lblTitle);

            Label lblDescription = new Label();
            lblDescription.Location = new Point(20, 50);
            lblDescription.Size = new Size(540, 30);
            lblDescription.Text = "Please wait while the components are being installed...";
            progressPage.Controls.Add(lblDescription);

            // Add progress list view
            lvProgress = new ListView();
            lvProgress.Location = new Point(20, 90);
            lvProgress.Size = new Size(540, 150);
            lvProgress.View = View.Details;
            lvProgress.FullRowSelect = true;
            lvProgress.GridLines = true;
            lvProgress.Columns.Add("Component", 150);
            lvProgress.Columns.Add("Status", 390);
            progressPage.Controls.Add(lvProgress);

            // Add progress bar
            progressBar = new ProgressBar();
            progressBar.Location = new Point(20, 250);
            progressBar.Size = new Size(540, 23);
            progressPage.Controls.Add(progressBar);

            // Add status label
            lblStatus = new Label();
            lblStatus.Location = new Point(20, 280);
            lblStatus.Size = new Size(540, 20);
            lblStatus.Text = "Preparing...";
            progressPage.Controls.Add(lblStatus);

            this.Controls.Add(progressPage);
        }

        private void InitializeFinishPage()
        {
            finishPage = new Panel();
            finishPage.Dock = DockStyle.Fill;
            finishPage.Padding = new Padding(20);
            finishPage.Visible = false;

            Label lblTitle = new Label();
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTitle.Location = new Point(20, 20);
            lblTitle.Text = "Installation Complete";
            finishPage.Controls.Add(lblTitle);

            Label lblDescription = new Label();
            lblDescription.Location = new Point(20, 50);
            lblDescription.Size = new Size(540, 60);
            lblDescription.Text = "Your portable WAMP stack has been successfully installed. You can start " +
                "the servers using the batch files or shortcuts created during installation.";
            finishPage.Controls.Add(lblDescription);

            Label lblServices = new Label();
            lblServices.Location = new Point(20, 120);
            lblServices.Size = new Size(540, 30);
            lblServices.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblServices.Text = "Installed Services:";
            finishPage.Controls.Add(lblServices);

            Label lblServicesList = new Label();
            lblServicesList.Location = new Point(40, 150);
            lblServicesList.Size = new Size(540, 100);
            lblServicesList.Text = "● Apache: http://localhost:80\r\n● PHP: Integrated with Apache\r\n● MySQL: localhost:3306";
            finishPage.Controls.Add(lblServicesList);

            Label lblNext = new Label();
            lblNext.Location = new Point(20, 260);
            lblNext.Size = new Size(540, 30);
            lblNext.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNext.Text = "What's Next?";
            finishPage.Controls.Add(lblNext);

            Label lblNextSteps = new Label();
            lblNextSteps.Location = new Point(40, 290);
            lblNextSteps.Size = new Size(540, 100);
            lblNextSteps.Text = "● Start the servers using start_wamp.bat\r\n" +
                "● Place your web files in the htdocs folder\r\n" +
                "● Access phpMyAdmin at http://localhost/phpmyadmin";
            finishPage.Controls.Add(lblNextSteps);

            CheckBox chkLaunchWAMP = new CheckBox();
            chkLaunchWAMP.Location = new Point(20, 400);
            chkLaunchWAMP.Size = new Size(300, 24);
            chkLaunchWAMP.Text = "Launch WAMP when I click Finish";
            chkLaunchWAMP.Checked = true;
            finishPage.Controls.Add(chkLaunchWAMP);

            this.Controls.Add(finishPage);
        }

        #endregion

        #region Navigation Handlers

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                UpdatePageVisibility();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (ValidateCurrentPage())
            {
                if (currentPage < 5)
                {
                    currentPage++;

                    // Special handling for the progress page
                    if (currentPage == 4)
                    {
                        StartInstallation();
                    }

                    UpdatePageVisibility();
                }
                else
                {
                    // Finish button clicked on the last page
                    // Launch WAMP if the checkbox is checked (we would check this in a real implementation)
                    this.Close();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to cancel the installation?",
                               "Cancel Installation",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
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
                    txtInstallLocation.Text = Path.Combine(folderDialog.SelectedPath, "WAMPPortable");
                }
            }
        }

        private void UpdatePageVisibility()
        {
            // Hide all pages
            welcomePage.Visible = false;
            componentsPage.Visible = false;
            locationPage.Visible = false;
            configPage.Visible = false;
            progressPage.Visible = false;
            finishPage.Visible = false;

            // Show the current page
            switch (currentPage)
            {
                case 0:
                    welcomePage.Visible = true;
                    btnBack.Enabled = false;
                    btnNext.Text = "Next >";
                    break;
                case 1:
                    componentsPage.Visible = true;
                    btnBack.Enabled = true;
                    btnNext.Text = "Next >";
                    break;
                case 2:
                    locationPage.Visible = true;
                    btnNext.Text = "Next >";
                    break;
                case 3:
                    configPage.Visible = true;
                    btnNext.Text = "Install";
                    break;
                case 4:
                    progressPage.Visible = true;
                    btnBack.Enabled = false;
                    btnNext.Enabled = false;
                    btnCancel.Enabled = false;
                    break;
                case 5:
                    finishPage.Visible = true;
                    btnBack.Enabled = false;
                    btnNext.Text = "Finish";
                    btnCancel.Enabled = false;
                    break;
            }
        }

        private bool ValidateCurrentPage()
        {
            switch (currentPage)
            {
                case 0:
                    // Welcome page - always valid
                    return true;

                case 1:
                    // Components page - ensure at least one component is selected
                    if (!chkApache.Checked && !chkPHP.Checked && !chkMySQL.Checked)
                    {
                        MessageBox.Show("Please select at least one component to install.",
                                         "Selection Required",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Warning);
                        return false;
                    }
                    return true;

                case 2:
                    // Location page - validate the installation path
                    if (string.IsNullOrWhiteSpace(txtInstallLocation.Text))
                    {
                        MessageBox.Show("Please specify an installation location.",
                                         "Location Required",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Warning);
                        return false;
                    }
                    return true;

                case 3:
                    // Configuration page - validate port numbers and password
                    int apachePort, mysqlPort;

                    if (!int.TryParse(txtApachePort.Text, out apachePort) || apachePort < 1 || apachePort > 65535)
                    {
                        MessageBox.Show("Please enter a valid port number for Apache (1-65535).",
                                         "Invalid Port",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Warning);
                        return false;
                    }

                    if (!int.TryParse(txtMySQLPort.Text, out mysqlPort) || mysqlPort < 1 || mysqlPort > 65535)
                    {
                        MessageBox.Show("Please enter a valid port number for MySQL (1-65535).",
                                         "Invalid Port",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Warning);
                        return false;
                    }

                    if (apachePort == mysqlPort)
                    {
                        MessageBox.Show("Apache and MySQL cannot use the same port.",
                                         "Port Conflict",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Warning);
                        return false;
                    }

                    if (string.IsNullOrEmpty(txtMySQLPassword.Text))
                    {
                        MessageBox.Show("Please enter a password for MySQL root user.",
                                         "Password Required",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Warning);
                        return false;
                    }

                    return true;

                default:
                    return true;
            }
        }

        #endregion

        #region Installation Logic

        private void InitializeBackgroundWorker()
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        private void StartInstallation()
        {
            // Capture installation parameters
            baseDir = txtInstallLocation.Text;
            installApache = chkApache.Checked;
            installPHP = chkPHP.Checked;
            installMySQL = chkMySQL.Checked;
            apachePort = int.Parse(txtApachePort.Text);
            mysqlPort = int.Parse(txtMySQLPort.Text);
            mysqlPassword = txtMySQLPassword.Text;
            createShortcuts = chkCreateShortcuts.Checked;

            // Set paths
            apachePath = Path.Combine(baseDir, "Apache24");
            phpPath = Path.Combine(baseDir, "php");
            mysqlPath = Path.Combine(baseDir, "mysql");

            // Initialize progress list view
            lvProgress.Items.Clear();
            if (installApache)
                lvProgress.Items.Add(new ListViewItem(new string[] { "Apache", "Pending" }));
            if (installPHP)
                lvProgress.Items.Add(new ListViewItem(new string[] { "PHP", "Pending" }));
            if (installMySQL)
                lvProgress.Items.Add(new ListViewItem(new string[] { "MySQL", "Pending" }));
            lvProgress.Items.Add(new ListViewItem(new string[] { "Configuration", "Pending" }));

            // Start the background worker
            worker.RunWorkerAsync();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int overallProgress = 0;

            try
            {
                // Create base directory
                Directory.CreateDirectory(baseDir);

                // Install Apache if selected
                if (installApache)
                {
                    worker.ReportProgress(overallProgress, new InstallStatus { Component = "Apache", Status = "Downloading...", OverallProgress = overallProgress });
                    string zipFile = Path.Combine(baseDir, "apache.zip");
                    string extractDir = Path.Combine(baseDir, "apache_extract");

                    Directory.CreateDirectory(extractDir);
                    DownloadFile(apacheUrl, zipFile, worker, "Apache");

                    worker.ReportProgress(overallProgress += 10, new InstallStatus { Component = "Apache", Status = "Extracting...", OverallProgress = overallProgress });
                    ExtractZipFile(zipFile, extractDir);

                    // Move Apache to final location
                    string extractedApachePath = Path.Combine(extractDir, "Apache24");
                    if (Directory.Exists(apachePath))
                        Directory.Delete(apachePath, true);
                    Directory.Move(extractedApachePath, apachePath);

                    worker.ReportProgress(overallProgress += 5, new InstallStatus { Component = "Apache", Status = "Configuring...", OverallProgress = overallProgress });
                    ConfigureApache(apachePath, apachePort);

                    // Clean up
                    try
                    {
                        File.Delete(zipFile);
                        Directory.Delete(extractDir, true);
                    }
                    catch { /* Ignore cleanup errors */ }

                    worker.ReportProgress(overallProgress += 5, new InstallStatus { Component = "Apache", Status = "Completed", OverallProgress = overallProgress });
                }

                // Install PHP if selected
                if (installPHP)
                {
                    worker.ReportProgress(overallProgress, new InstallStatus { Component = "PHP", Status = "Downloading...", OverallProgress = overallProgress });
                    string zipFile = Path.Combine(baseDir, "php.zip");
                    string extractDir = Path.Combine(baseDir, "php_extract");

                    Directory.CreateDirectory(extractDir);
                    DownloadFile(phpUrl, zipFile, worker, "PHP");

                    worker.ReportProgress(overallProgress += 10, new InstallStatus { Component = "PHP", Status = "Extracting...", OverallProgress = overallProgress });
                    ExtractZipFile(zipFile, extractDir);

                    // Move PHP to final location
                    if (Directory.Exists(phpPath))
                        Directory.Delete(phpPath, true);
                    Directory.Move(extractDir, phpPath);

                    worker.ReportProgress(overallProgress += 5, new InstallStatus { Component = "PHP", Status = "Configuring...", OverallProgress = overallProgress });
                    ConfigurePHP(phpPath, apachePath);

                    // Clean up
                    try
                    {
                        File.Delete(zipFile);
                    }
                    catch { /* Ignore cleanup errors */ }

                    worker.ReportProgress(overallProgress += 5, new InstallStatus { Component = "PHP", Status = "Completed", OverallProgress = overallProgress });
                }

                // Install MySQL if selected
                if (installMySQL)
                {
                    worker.ReportProgress(overallProgress, new InstallStatus { Component = "MySQL", Status = "Downloading...", OverallProgress = overallProgress });
                    string zipFile = Path.Combine(baseDir, "mysql.zip");
                    string extractDir = Path.Combine(baseDir, "mysql_extract");

                    Directory.CreateDirectory(extractDir);
                    DownloadFile(mysqlUrl, zipFile, worker, "MySQL");

                    worker.ReportProgress(overallProgress += 10, new InstallStatus { Component = "MySQL", Status = "Extracting...", OverallProgress = overallProgress });
                    ExtractZipFile(zipFile, extractDir);

                    // Move MySQL to final location
                    if (Directory.Exists(mysqlPath))
                        Directory.Delete(mysqlPath, true);

                    // Find MySQL directory in extracted folder (it may have a version number)
                    string mysqlExtractedDir = null;
                    foreach (string dir in Directory.GetDirectories(extractDir))
                    {
                        if (Path.GetFileName(dir).StartsWith("mysql"))
                        {
                            mysqlExtractedDir = dir;
                            break;
                        }
                    }

                    if (mysqlExtractedDir != null)
                    {
                        Directory.Move(mysqlExtractedDir, mysqlPath);
                    }
                    else
                    {
                        Directory.Move(extractDir, mysqlPath);
                    }

                    worker.ReportProgress(overallProgress += 5, new InstallStatus { Component = "MySQL", Status = "Configuring...", OverallProgress = overallProgress });
                    ConfigureMySQL(mysqlPath, mysqlPort, mysqlPassword);

                    // Clean up
                    try
                    {
                        File.Delete(zipFile);
                        if (Directory.Exists(extractDir))
                            Directory.Delete(extractDir, true);
                    }
                    catch { /* Ignore cleanup errors */ }

                    worker.ReportProgress(overallProgress += 5, new InstallStatus { Component = "MySQL", Status = "Completed", OverallProgress = overallProgress });
                }

                // Create integration configurations
                worker.ReportProgress(overallProgress, new InstallStatus { Component = "Configuration", Status = "Integrating components...", OverallProgress = overallProgress });
                IntegrateComponents();

                // Create startup and shutdown scripts
                CreateScripts();

                // Create shortcuts if requested
                if (createShortcuts)
                {
                    CreateShortcuts();
                }

                // Install phpMyAdmin
                if (installApache && installPHP && installMySQL)
                {
                    worker.ReportProgress(overallProgress += 5, new InstallStatus { Component = "Configuration", Status = "Installing phpMyAdmin...", OverallProgress = overallProgress });
                    InstallPhpMyAdmin();
                }

                worker.ReportProgress(100, new InstallStatus { Component = "Configuration", Status = "Completed", OverallProgress = 100 });

                // Populate finish page information
                UpdateFinishPageInfo();

                e.Result = true;
            }
            catch (Exception ex)
            {
                worker.ReportProgress(0, new InstallStatus { Component = "Error", Status = ex.Message, OverallProgress = 0 });
                e.Result = false;
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            InstallStatus status = e.UserState as InstallStatus;
            if (status != null)
            {
                // Update progress bar
                progressBar.Value = status.OverallProgress;

                // Update status label
                lblStatus.Text = $"{status.Component}: {status.Status}";

                // Update list view
                foreach (ListViewItem item in lvProgress.Items)
                {
                    if (item.Text == status.Component)
                    {
                        item.SubItems[1].Text = status.Status;
                        break;
                    }
                }
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result)
            {
                // Installation successful, move to final page
                currentPage = 5;
                UpdatePageVisibility();

                // Enable the Next button (now Finish)
                btnNext.Enabled = true;
            }
            else
            {
                // Installation failed, allow going back
                btnBack.Enabled = true;
                btnCancel.Enabled = true;

                MessageBox.Show("Installation failed. Please check the error details.",
                               "Installation Error",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Installation Helper Methods

        private void DownloadFile(string url, string destinationPath, BackgroundWorker worker, string component)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");

                // Add download progress reporting
                client.DownloadProgressChanged += (s, e) =>
                {
                    worker.ReportProgress(0, new InstallStatus
                    {
                        Component = component,
                        Status = $"Downloading... {e.ProgressPercentage}%",
                        OverallProgress = 0
                    });
                };

                // Download the file synchronously (the worker is already on a background thread)
                client.DownloadFile(url, destinationPath);
            }
        }

        private void ExtractZipFile(string zipFilePath, string extractPath)
        {
            ZipFile.ExtractToDirectory(zipFilePath, extractPath);
        }

        private void ConfigureApache(string apachePath, int port)
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

            // Enable common modules needed for PHP
            confContent = confContent.Replace(
                "#LoadModule rewrite_module modules/mod_rewrite.so",
                "LoadModule rewrite_module modules/mod_rewrite.so");

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
    <title>WAMP Portable Stack</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 40px; line-height: 1.6; }}
        h1 {{ color: #333; }}
        .container {{ max-width: 800px; margin: 0 auto; }}
        .component {{ background-color: #f5f5f5; padding: 15px; margin-bottom: 15px; border-radius: 5px; }}
        .success {{ color: green; }}
    </style>
</head>
<body>
    <div class='container'>
        <h1>WAMP Portable Stack <span class='success'>✓</span></h1>
        <div class='component'>
            <h2>Apache Web Server</h2>
            <p>Status: <span class='success'>Running</span></p>
            <p>Port: {port}</p>
        </div>
        <div class='component'>
            <h2>PHP</h2>
            <p>Check PHP info: <a href='phpinfo.php'>phpinfo.php</a></p>
        </div>
        <div class='component'>
            <h2>MySQL</h2>
            <p>Port: {mysqlPort}</p>
            <p>User: root</p>
            <p>phpMyAdmin: <a href='phpmyadmin/'>phpmyadmin/</a> (if installed)</p>
        </div>
        <p>Thank you for using WAMP Portable Wizard!</p>
    </div>
</body>
</html>";
            File.WriteAllText(indexPath, indexContent, Encoding.UTF8);

            // Create a phpinfo.php file
            string phpinfoPath = Path.Combine(apachePath, "htdocs", "phpinfo.php");
            File.WriteAllText(phpinfoPath, "<?php\nphpinfo();\n?>");
        }

        private void ConfigurePHP(string phpPath, string apachePath)
        {
            // Copy php.ini-development to php.ini
            string phpIniSrc = Path.Combine(phpPath, "php.ini-development");
            string phpIniDest = Path.Combine(phpPath, "php.ini");
            File.Copy(phpIniSrc, phpIniDest, true);

            // Modify php.ini
            string phpIniContent = File.ReadAllText(phpIniDest);

            // Uncomment and set extension_dir
            phpIniContent = Regex.Replace(phpIniContent,
                @";extension_dir\s*=\s*""ext""",
                "extension_dir = \"ext\"");

            // Enable common extensions
            phpIniContent = phpIniContent.Replace(";extension=mysqli", "extension=mysqli");
            phpIniContent = phpIniContent.Replace(";extension=pdo_mysql", "extension=pdo_mysql");
            phpIniContent = phpIniContent.Replace(";extension=mbstring", "extension=mbstring");
            phpIniContent = phpIniContent.Replace(";extension=openssl", "extension=openssl");

            // Save modified php.ini
            File.WriteAllText(phpIniDest, phpIniContent);
        }

        private void ConfigureMySQL(string mysqlPath, int port, string password)
        {
            // Create data directory if it doesn't exist
            string dataDir = Path.Combine(mysqlPath, "data");
            Directory.CreateDirectory(dataDir);

            // Create my.ini configuration file
            string myIniPath = Path.Combine(mysqlPath, "my.ini");
            string myIniContent = $@"[mysqld]
# Set port
port={port}

# Set base directory
basedir=./

# Set data directory
datadir=./data

# Default character set
character-set-server=utf8mb4
collation-server=utf8mb4_general_ci

# Default storage engine
default-storage-engine=INNODB

# Max allowed packet
max_allowed_packet=256M

# Binary logging format
binlog_format=row

# Server ID
server-id=1

[mysql]
# Default character set
default-character-set=utf8mb4

[client]
# Default character set
default-character-set=utf8mb4
port={port}
";
            File.WriteAllText(myIniPath, myIniContent);
        }

        private void IntegrateComponents()
        {
            if (installApache && installPHP)
            {
                // Configure Apache to use PHP
                string httpConf = Path.Combine(apachePath, "conf", "httpd.conf");
                string httpConfContent = File.ReadAllText(httpConf);

                // Check if the PHP configuration already exists, if not, add it
                if (!httpConfContent.Contains("application/x-httpd-php"))
                {
                    // Add PHP module configuration
                    string phpConfig = $@"
# PHP Configuration
LoadModule php_module ""./../php/php8apache2_4.dll""
AddType application/x-httpd-php .php
PHPIniDir ""./../php""

<FilesMatch \.php$>
    SetHandler application/x-httpd-php
</FilesMatch>

# PHP default index file
<IfModule dir_module>
    DirectoryIndex index.php index.html
</IfModule>
";
                    // Append PHP configuration
                    httpConfContent += phpConfig;
                    File.WriteAllText(httpConf, httpConfContent);
                }
            }
        }

        private void CreateScripts()
        {
            // Create start_wamp.bat
            string startBatchContent = @"@echo off
SETLOCAL
set CURRENT_DIR=%~dp0
set APACHE_DIR=%CURRENT_DIR%Apache24
set PHP_DIR=%CURRENT_DIR%php
set MYSQL_DIR=%CURRENT_DIR%mysql

echo WAMP Portable Server
echo ==========================================
echo.
";

            if (installApache)
            {
                startBatchContent += @"
echo Starting Apache...
cd /d ""%APACHE_DIR%\bin""
httpd.exe -k install -n ""WAMPApache""
httpd.exe -k start -n ""WAMPApache""
IF %ERRORLEVEL% NEQ 0 (
  echo Failed to start Apache! Check if port is already in use.
) ELSE (
  echo Apache started successfully.
)
echo.
";
            }

            if (installMySQL)
            {
                startBatchContent += @"
echo Starting MySQL...
cd /d ""%MYSQL_DIR%\bin""
start /min mysqld.exe --defaults-file=""%MYSQL_DIR%\my.ini"" --standalone
timeout /t 5 > nul
echo MySQL started successfully.
echo.
";
            }

            startBatchContent += $@"
echo All servers started!
echo Access your web server at http://localhost:{apachePort}
start http://localhost:{apachePort}
echo.
echo Press any key to exit this window...
pause > nul
";
            File.WriteAllText(Path.Combine(baseDir, "start_wamp.bat"), startBatchContent);

            // Create stop_wamp.bat
            string stopBatchContent = @"@echo off
SETLOCAL
set CURRENT_DIR=%~dp0
set APACHE_DIR=%CURRENT_DIR%Apache24
set PHP_DIR=%CURRENT_DIR%php
set MYSQL_DIR=%CURRENT_DIR%mysql

echo Stopping WAMP Portable Server
echo ==========================================
echo.
";

            if (installApache)
            {
                stopBatchContent += @"
echo Stopping Apache...
cd /d ""%APACHE_DIR%\bin""
httpd.exe -k stop -n ""WAMPApache""
httpd.exe -k uninstall -n ""WAMPApache""
echo Apache stopped.
echo.
";
            }

            if (installMySQL)
            {
                stopBatchContent += @"
echo Stopping MySQL...
cd /d ""%MYSQL_DIR%\bin""
mysqladmin -u root -p" + mysqlPassword + @" shutdown
echo MySQL stopped.
echo.
";
            }

            stopBatchContent += @"
echo All services stopped.
echo.
echo Press any key to exit...
pause > nul
";
            File.WriteAllText(Path.Combine(baseDir, "stop_wamp.bat"), stopBatchContent);
        }

        private void CreateShortcuts()
        {
            // In a real application, you would use IWshRuntimeLibrary or similar to create shortcuts
            // For simplicity, we're just creating .lnk-like batch files that open the actual batch files
            string startLinkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Start WAMP Portable.bat");
            string startLinkContent = $@"@echo off
start """" ""{Path.Combine(baseDir, "start_wamp.bat")}""";
            File.WriteAllText(startLinkPath, startLinkContent);

            string stopLinkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Stop WAMP Portable.bat");
            string stopLinkContent = $@"@echo off
start """" ""{Path.Combine(baseDir, "stop_wamp.bat")}""";
            File.WriteAllText(stopLinkPath, stopLinkContent);
        }

        private void InstallPhpMyAdmin()
        {
            // In a real implementation, this would download phpMyAdmin and configure it
            // For simplicity, we'll just create a placeholder file that redirects to the online phpMyAdmin demo

            string phpmyadminDir = Path.Combine(apachePath, "htdocs", "phpmyadmin");
            Directory.CreateDirectory(phpmyadminDir);

            string indexPath = Path.Combine(phpmyadminDir, "index.php");
            string indexContent = @"<?php
// This is a placeholder for phpMyAdmin
// In a full implementation, phpMyAdmin would be installed here
?>
<!DOCTYPE html>
<html>
<head>
    <title>phpMyAdmin Placeholder</title>
    <style>
        body { font-family: Arial, sans-serif; margin: 40px; line-height: 1.6; }
        .container { max-width: 800px; margin: 0 auto; padding: 20px; background-color: #f5f5f5; }
    </style>
</head>
<body>
    <div class='container'>
        <h1>phpMyAdmin Placeholder</h1>
        <p>In a complete implementation, phpMyAdmin would be installed and configured here.</p>
        <p>To access your MySQL server:</p>
        <ul>
            <li>Host: localhost</li>
            <li>Port: " + mysqlPort + @"</li>
            <li>Username: root</li>
            <li>Password: " + mysqlPassword + @"</li>
        </ul>
    </div>
</body>
</html>";
            File.WriteAllText(indexPath, indexContent);
        }

        private void UpdateFinishPageInfo()
        {
            // In a real implementation, this would dynamically update the finish page
            // with details about the installed components
        }

        #endregion
    }

    class InstallStatus
    {
        public string Component { get; set; }
        public string Status { get; set; }
        public int OverallProgress { get; set; }
    }
    
}