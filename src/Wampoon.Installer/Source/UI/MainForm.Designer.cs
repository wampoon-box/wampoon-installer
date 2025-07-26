using System;
using System.Drawing;
using System.Windows.Forms;

namespace Wampoon.Installer.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // UI Controls.
        private CheckBox _apacheCheckBox;
        private CheckBox _mariadbCheckBox;
        private CheckBox _phpCheckBox;
        private CheckBox _phpmyadminCheckBox;
        private CheckBox _xdebugCheckBox;
        private TextBox _installPathTextBox;
        private Button _browseButton;
        private Button _openFolderButton;
        private Button _installButton;
        private Button _cancelButton;
        private Button _exportLogButton;
        private Button _quitButton;
        private Button _aboutButton;
        private ProgressBar _progressBar;
        private Label _progressLabel;
        private RichTextBox _logTextBox;
        private GroupBox _componentsGroup;
        private GroupBox _pathGroup;
        private GroupBox _logGroup;
        private Panel _bannerPanel;
        private Label _bannerTitle;
        private Label _bannerSubtitle;
        private PictureBox _bannerIcon;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._componentsGroup = new System.Windows.Forms.GroupBox();
            this._apacheCheckBox = new System.Windows.Forms.CheckBox();
            this._mariadbCheckBox = new System.Windows.Forms.CheckBox();
            this._phpCheckBox = new System.Windows.Forms.CheckBox();
            this._phpmyadminCheckBox = new System.Windows.Forms.CheckBox();
            this._xdebugCheckBox = new System.Windows.Forms.CheckBox();
            this._pathGroup = new System.Windows.Forms.GroupBox();
            this._installPathTextBox = new System.Windows.Forms.TextBox();
            this._browseButton = new System.Windows.Forms.Button();
            this._openFolderButton = new System.Windows.Forms.Button();
            this._installButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._exportLogButton = new System.Windows.Forms.Button();
            this._quitButton = new System.Windows.Forms.Button();
            this._aboutButton = new System.Windows.Forms.Button();
            this._progressLabel = new System.Windows.Forms.Label();
            this._progressBar = new System.Windows.Forms.ProgressBar();
            this._logGroup = new System.Windows.Forms.GroupBox();
            this._logTextBox = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this._bannerPanel = new System.Windows.Forms.Panel();
            this._bannerIcon = new System.Windows.Forms.PictureBox();
            this._bannerTitle = new System.Windows.Forms.Label();
            this._bannerSubtitle = new System.Windows.Forms.Label();
            this._componentsGroup.SuspendLayout();
            this._pathGroup.SuspendLayout();
            this._logGroup.SuspendLayout();
            this.panel1.SuspendLayout();
            this._bannerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._bannerIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // _componentsGroup
            // 
            this._componentsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._componentsGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this._componentsGroup.Controls.Add(this._apacheCheckBox);
            this._componentsGroup.Controls.Add(this._mariadbCheckBox);
            this._componentsGroup.Controls.Add(this._phpCheckBox);
            this._componentsGroup.Controls.Add(this._phpmyadminCheckBox);
            this._componentsGroup.Controls.Add(this._xdebugCheckBox);
            this._componentsGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._componentsGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._componentsGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this._componentsGroup.Location = new System.Drawing.Point(20, 100);
            this._componentsGroup.Name = "_componentsGroup";
            this._componentsGroup.Padding = new System.Windows.Forms.Padding(20);
            this._componentsGroup.Size = new System.Drawing.Size(839, 190);
            this._componentsGroup.TabIndex = 0;
            this._componentsGroup.TabStop = false;
            this._componentsGroup.Text = "📦 Select Components to Install";
            // 
            // _apacheCheckBox
            // 
            this._apacheCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(251)))));
            this._apacheCheckBox.Checked = true;
            this._apacheCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this._apacheCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._apacheCheckBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._apacheCheckBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this._apacheCheckBox.Location = new System.Drawing.Point(17, 30);
            this._apacheCheckBox.Name = "_apacheCheckBox";
            this._apacheCheckBox.Padding = new System.Windows.Forms.Padding(10);
            this._apacheCheckBox.Size = new System.Drawing.Size(308, 43);
            this._apacheCheckBox.TabIndex = 0;
            this._apacheCheckBox.Text = "🌐 Apache HTTP Server";
            this._apacheCheckBox.UseVisualStyleBackColor = false;
            // 
            // _mariadbCheckBox
            // 
            this._mariadbCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(251)))));
            this._mariadbCheckBox.Checked = true;
            this._mariadbCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this._mariadbCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._mariadbCheckBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._mariadbCheckBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this._mariadbCheckBox.Location = new System.Drawing.Point(343, 30);
            this._mariadbCheckBox.Name = "_mariadbCheckBox";
            this._mariadbCheckBox.Padding = new System.Windows.Forms.Padding(10);
            this._mariadbCheckBox.Size = new System.Drawing.Size(346, 42);
            this._mariadbCheckBox.TabIndex = 1;
            this._mariadbCheckBox.Text = "🗄️ MariaDB Database Server";
            this._mariadbCheckBox.UseVisualStyleBackColor = false;
            // 
            // _phpCheckBox
            // 
            this._phpCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(251)))));
            this._phpCheckBox.Checked = true;
            this._phpCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this._phpCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._phpCheckBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._phpCheckBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this._phpCheckBox.Location = new System.Drawing.Point(17, 83);
            this._phpCheckBox.Name = "_phpCheckBox";
            this._phpCheckBox.Padding = new System.Windows.Forms.Padding(10);
            this._phpCheckBox.Size = new System.Drawing.Size(308, 43);
            this._phpCheckBox.TabIndex = 2;
            this._phpCheckBox.Text = "🐘 PHP Scripting Language";
            this._phpCheckBox.UseVisualStyleBackColor = false;
            // 
            // _phpmyadminCheckBox
            // 
            this._phpmyadminCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(251)))));
            this._phpmyadminCheckBox.Checked = true;
            this._phpmyadminCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this._phpmyadminCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._phpmyadminCheckBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._phpmyadminCheckBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this._phpmyadminCheckBox.Location = new System.Drawing.Point(343, 83);
            this._phpmyadminCheckBox.Name = "_phpmyadminCheckBox";
            this._phpmyadminCheckBox.Padding = new System.Windows.Forms.Padding(10);
            this._phpmyadminCheckBox.Size = new System.Drawing.Size(346, 42);
            this._phpmyadminCheckBox.TabIndex = 3;
            this._phpmyadminCheckBox.Text = "🔧 phpMyAdmin Database Manager";
            this._phpmyadminCheckBox.UseVisualStyleBackColor = false;
            // 
            // _xdebugCheckBox
            // 
            this._xdebugCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(251)))));
            this._xdebugCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._xdebugCheckBox.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._xdebugCheckBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this._xdebugCheckBox.Location = new System.Drawing.Point(17, 132);
            this._xdebugCheckBox.Name = "_xdebugCheckBox";
            this._xdebugCheckBox.Padding = new System.Windows.Forms.Padding(10);
            this._xdebugCheckBox.Size = new System.Drawing.Size(308, 43);
            this._xdebugCheckBox.TabIndex = 4;
            this._xdebugCheckBox.Text = "🐛 Xdebug PHP Extension";
            this._xdebugCheckBox.UseVisualStyleBackColor = false;
            // 
            // _pathGroup
            // 
            this._pathGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._pathGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this._pathGroup.Controls.Add(this._installPathTextBox);
            this._pathGroup.Controls.Add(this._browseButton);
            this._pathGroup.Controls.Add(this._openFolderButton);
            this._pathGroup.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this._pathGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this._pathGroup.Location = new System.Drawing.Point(20, 296);
            this._pathGroup.Name = "_pathGroup";
            this._pathGroup.Padding = new System.Windows.Forms.Padding(15);
            this._pathGroup.Size = new System.Drawing.Size(839, 60);
            this._pathGroup.TabIndex = 1;
            this._pathGroup.TabStop = false;
            this._pathGroup.Text = "📁 Installation Directory:";
            // 
            // _installPathTextBox
            // 
            this._installPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._installPathTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(250)))), ((int)(((byte)(251)))));
            this._installPathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._installPathTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._installPathTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this._installPathTextBox.Location = new System.Drawing.Point(15, 25);
            this._installPathTextBox.Name = "_installPathTextBox";
            this._installPathTextBox.Size = new System.Drawing.Size(483, 23);
            this._installPathTextBox.TabIndex = 0;
            this._installPathTextBox.Text = "C:\\Wampoon";
            // 
            // _browseButton
            // 
            this._browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._browseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this._browseButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._browseButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this._browseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._browseButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._browseButton.ForeColor = System.Drawing.Color.White;
            this._browseButton.Location = new System.Drawing.Point(510, 19);
            this._browseButton.Name = "_browseButton";
            this._browseButton.Size = new System.Drawing.Size(80, 34);
            this._browseButton.TabIndex = 1;
            this._browseButton.Text = "Browse";
            this._browseButton.UseVisualStyleBackColor = false;
            this._browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // _openFolderButton
            // 
            this._openFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._openFolderButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(50)))), ((int)(((byte)(155)))));
            this._openFolderButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._openFolderButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
            this._openFolderButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._openFolderButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._openFolderButton.ForeColor = System.Drawing.Color.White;
            this._openFolderButton.Location = new System.Drawing.Point(596, 19);
            this._openFolderButton.Name = "_openFolderButton";
            this._openFolderButton.Size = new System.Drawing.Size(145, 34);
            this._openFolderButton.TabIndex = 2;
            this._openFolderButton.Text = "Open Selected Folder";
            this._openFolderButton.UseVisualStyleBackColor = false;
            this._openFolderButton.Click += new System.EventHandler(this.OpenFolderButton_Click);
            // 
            // _installButton
            // 
            this._installButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(145)))), ((int)(((byte)(49)))));
            this._installButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._installButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this._installButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._installButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._installButton.ForeColor = System.Drawing.Color.White;
            this._installButton.Location = new System.Drawing.Point(26, 371);
            this._installButton.Name = "_installButton";
            this._installButton.Size = new System.Drawing.Size(146, 34);
            this._installButton.TabIndex = 2;
            this._installButton.Text = "▶️ Start Installation";
            this._installButton.UseVisualStyleBackColor = false;
            this._installButton.Click += new System.EventHandler(this.InstallButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this._cancelButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._cancelButton.Enabled = false;
            this._cancelButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(127)))));
            this._cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._cancelButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._cancelButton.ForeColor = System.Drawing.Color.White;
            this._cancelButton.Location = new System.Drawing.Point(182, 371);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(80, 34);
            this._cancelButton.TabIndex = 3;
            this._cancelButton.Text = "❌ Cancel";
            this._cancelButton.UseVisualStyleBackColor = false;
            this._cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // _exportLogButton
            // 
            this._exportLogButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(125)))), ((int)(((byte)(219)))));
            this._exportLogButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._exportLogButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(6)))), ((int)(((byte)(182)))), ((int)(((byte)(212)))));
            this._exportLogButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._exportLogButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._exportLogButton.ForeColor = System.Drawing.Color.White;
            this._exportLogButton.Location = new System.Drawing.Point(295, 371);
            this._exportLogButton.Name = "_exportLogButton";
            this._exportLogButton.Size = new System.Drawing.Size(107, 34);
            this._exportLogButton.TabIndex = 4;
            this._exportLogButton.Text = "💾 Export Log";
            this._exportLogButton.UseVisualStyleBackColor = false;
            this._exportLogButton.Click += new System.EventHandler(this.ExportLogButton_Click);
            // 
            // _quitButton
            // 
            this._quitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(110)))));
            this._quitButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._quitButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(38)))), ((int)(((byte)(127)))));
            this._quitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._quitButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._quitButton.ForeColor = System.Drawing.Color.White;
            this._quitButton.Location = new System.Drawing.Point(631, 371);
            this._quitButton.Name = "_quitButton";
            this._quitButton.Size = new System.Drawing.Size(80, 34);
            this._quitButton.TabIndex = 6;
            this._quitButton.Text = "❌ Quit";
            this._quitButton.UseVisualStyleBackColor = false;
            this._quitButton.Click += new System.EventHandler(this.QuitButton_Click);
            // 
            // _aboutButton
            // 
            this._aboutButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(61)))), ((int)(((byte)(90)))));
            this._aboutButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._aboutButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this._aboutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._aboutButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._aboutButton.ForeColor = System.Drawing.Color.White;
            this._aboutButton.Location = new System.Drawing.Point(531, 371);
            this._aboutButton.Name = "_aboutButton";
            this._aboutButton.Size = new System.Drawing.Size(88, 34);
            this._aboutButton.TabIndex = 5;
            this._aboutButton.Text = "ℹ️ About";
            this._aboutButton.UseVisualStyleBackColor = false;
            this._aboutButton.Click += new System.EventHandler(this.AboutButton_Click);
            // 
            // _progressLabel
            // 
            this._progressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._progressLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._progressLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this._progressLabel.Location = new System.Drawing.Point(27, 416);
            this._progressLabel.Name = "_progressLabel";
            this._progressLabel.Size = new System.Drawing.Size(912, 20);
            this._progressLabel.TabIndex = 5;
            this._progressLabel.Text = "Ready to install";
            // 
            // _progressBar
            // 
            this._progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._progressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(231)))), ((int)(((byte)(235)))));
            this._progressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(99)))), ((int)(((byte)(235)))));
            this._progressBar.Location = new System.Drawing.Point(27, 436);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(912, 25);
            this._progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this._progressBar.TabIndex = 6;
            // 
            // _logGroup
            // 
            this._logGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._logGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this._logGroup.Controls.Add(this._logTextBox);
            this._logGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._logGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this._logGroup.Location = new System.Drawing.Point(7, 3);
            this._logGroup.Name = "_logGroup";
            this._logGroup.Padding = new System.Windows.Forms.Padding(15);
            this._logGroup.Size = new System.Drawing.Size(929, 191);
            this._logGroup.TabIndex = 7;
            this._logGroup.TabStop = false;
            this._logGroup.Text = "📜 Installation Log:";
            // 
            // _logTextBox
            // 
            this._logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._logTextBox.BackColor = System.Drawing.Color.Black;
            this._logTextBox.Font = new System.Drawing.Font("Consolas", 10F);
            this._logTextBox.ForeColor = System.Drawing.Color.White;
            this._logTextBox.Location = new System.Drawing.Point(20, 25);
            this._logTextBox.Name = "_logTextBox";
            this._logTextBox.ReadOnly = true;
            this._logTextBox.Size = new System.Drawing.Size(889, 151);
            this._logTextBox.TabIndex = 0;
            this._logTextBox.Text = "";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(247)))));
            this.panel1.Controls.Add(this._logGroup);
            this.panel1.Location = new System.Drawing.Point(20, 471);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(919, 201);
            this.panel1.TabIndex = 7;
            // 
            // _bannerPanel
            // 
            this._bannerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._bannerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(78)))), ((int)(((byte)(175)))));
            this._bannerPanel.Controls.Add(this._bannerIcon);
            this._bannerPanel.Controls.Add(this._bannerTitle);
            this._bannerPanel.Controls.Add(this._bannerSubtitle);
            this._bannerPanel.Location = new System.Drawing.Point(0, 0);
            this._bannerPanel.Name = "_bannerPanel";
            this._bannerPanel.Size = new System.Drawing.Size(963, 80);
            this._bannerPanel.TabIndex = 8;
            // 
            // _bannerIcon
            // 
            this._bannerIcon.BackColor = System.Drawing.Color.Transparent;
            this._bannerIcon.Location = new System.Drawing.Point(30, 20);
            this._bannerIcon.Name = "_bannerIcon";
            this._bannerIcon.Size = new System.Drawing.Size(40, 40);
            this._bannerIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this._bannerIcon.TabIndex = 2;
            this._bannerIcon.TabStop = false;
            // 
            // _bannerTitle
            // 
            this._bannerTitle.AutoSize = true;
            this._bannerTitle.BackColor = System.Drawing.Color.Transparent;
            this._bannerTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this._bannerTitle.ForeColor = System.Drawing.Color.White;
            this._bannerTitle.Location = new System.Drawing.Point(85, 15);
            this._bannerTitle.Name = "_bannerTitle";
            this._bannerTitle.Size = new System.Drawing.Size(236, 32);
            this._bannerTitle.TabIndex = 0;
            this._bannerTitle.Text = "WAMPoon Installer";
            // 
            // _bannerSubtitle
            // 
            this._bannerSubtitle.AutoSize = true;
            this._bannerSubtitle.BackColor = System.Drawing.Color.Transparent;
            this._bannerSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this._bannerSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(234)))), ((int)(((byte)(254)))));
            this._bannerSubtitle.Location = new System.Drawing.Point(87, 47);
            this._bannerSubtitle.Name = "_bannerSubtitle";
            this._bannerSubtitle.Size = new System.Drawing.Size(436, 19);
            this._bannerSubtitle.TabIndex = 1;
            this._bannerSubtitle.Text = "Local Web Development Stack - Apache, MariaDB, PHP, phpMyAdmin";
            // 
            // MainForm
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(247)))));
            this.ClientSize = new System.Drawing.Size(963, 697);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._bannerPanel);
            this.Controls.Add(this._componentsGroup);
            this.Controls.Add(this._pathGroup);
            this.Controls.Add(this._installButton);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._exportLogButton);
            this.Controls.Add(this._quitButton);
            this.Controls.Add(this._aboutButton);
            this.Controls.Add(this._progressLabel);
            this.Controls.Add(this._progressBar);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(840, 736);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WAMPoon Installer";
            this._componentsGroup.ResumeLayout(false);
            this._pathGroup.ResumeLayout(false);
            this._pathGroup.PerformLayout();
            this._logGroup.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this._bannerPanel.ResumeLayout(false);
            this._bannerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._bannerIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
    }
}