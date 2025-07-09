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
            this._componentsGroup = new System.Windows.Forms.GroupBox();
            this._apacheCheckBox = new System.Windows.Forms.CheckBox();
            this._mariadbCheckBox = new System.Windows.Forms.CheckBox();
            this._phpCheckBox = new System.Windows.Forms.CheckBox();
            this._phpmyadminCheckBox = new System.Windows.Forms.CheckBox();
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
            this._componentsGroup.SuspendLayout();
            this._pathGroup.SuspendLayout();
            this._logGroup.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _componentsGroup.
            // 
            this._componentsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._componentsGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this._componentsGroup.Controls.Add(this._apacheCheckBox);
            this._componentsGroup.Controls.Add(this._mariadbCheckBox);
            this._componentsGroup.Controls.Add(this._phpCheckBox);
            this._componentsGroup.Controls.Add(this._phpmyadminCheckBox);
            this._componentsGroup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._componentsGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this._componentsGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this._componentsGroup.Location = new System.Drawing.Point(20, 20);
            this._componentsGroup.Name = "_componentsGroup";
            this._componentsGroup.Padding = new System.Windows.Forms.Padding(20);
            this._componentsGroup.Size = new System.Drawing.Size(839, 154);
            this._componentsGroup.TabIndex = 0;
            this._componentsGroup.TabStop = false;
            this._componentsGroup.Text = "📦 Select Components to Install";
            // 
            // _apacheCheckBox.
            // 
            this._apacheCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
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
            // _mariadbCheckBox.
            // 
            this._mariadbCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
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
            // _phpCheckBox.
            // 
            this._phpCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
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
            // _phpmyadminCheckBox.
            // 
            this._phpmyadminCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
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
            // _pathGroup.
            // 
            this._pathGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._pathGroup.Controls.Add(this._installPathTextBox);
            this._pathGroup.Controls.Add(this._browseButton);
            this._pathGroup.Controls.Add(this._openFolderButton);
            this._pathGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._pathGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this._pathGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this._pathGroup.Location = new System.Drawing.Point(20, 180);
            this._pathGroup.Name = "_pathGroup";
            this._pathGroup.Padding = new System.Windows.Forms.Padding(15);
            this._pathGroup.Size = new System.Drawing.Size(839, 60);
            this._pathGroup.TabIndex = 1;
            this._pathGroup.TabStop = false;
            this._pathGroup.Text = "📁 Installation Directory:";
            // 
            // _installPathTextBox.
            // 
            this._installPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._installPathTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this._installPathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._installPathTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._installPathTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(65)))), ((int)(((byte)(81)))));
            this._installPathTextBox.Location = new System.Drawing.Point(15, 25);
            this._installPathTextBox.Name = "_installPathTextBox";
            this._installPathTextBox.Size = new System.Drawing.Size(483, 23);
            this._installPathTextBox.TabIndex = 0;
            this._installPathTextBox.Text = "C:\\Wampoon";
            // 
            // _browseButton.
            // 
            this._browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._browseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this._browseButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._browseButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this._browseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._browseButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._browseButton.ForeColor = System.Drawing.Color.White;
            this._browseButton.Location = new System.Drawing.Point(504, 20);
            this._browseButton.Name = "_browseButton";
            this._browseButton.Size = new System.Drawing.Size(80, 30);
            this._browseButton.TabIndex = 1;
            this._browseButton.Text = "Browse";
            this._browseButton.UseVisualStyleBackColor = false;
            this._browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // _openFolderButton.
            // 
            this._openFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._openFolderButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(102)))), ((int)(((byte)(241)))));
            this._openFolderButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._openFolderButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(102)))), ((int)(((byte)(241)))));
            this._openFolderButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._openFolderButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._openFolderButton.ForeColor = System.Drawing.Color.White;
            this._openFolderButton.Location = new System.Drawing.Point(590, 20);
            this._openFolderButton.Name = "_openFolderButton";
            this._openFolderButton.Size = new System.Drawing.Size(117, 30);
            this._openFolderButton.TabIndex = 2;
            this._openFolderButton.Text = "Open Folder";
            this._openFolderButton.UseVisualStyleBackColor = false;
            this._openFolderButton.Click += new System.EventHandler(this.OpenFolderButton_Click);
            // 
            // _installButton.
            // 
            this._installButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this._installButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._installButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(197)))), ((int)(((byte)(94)))));
            this._installButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._installButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._installButton.ForeColor = System.Drawing.Color.White;
            this._installButton.Location = new System.Drawing.Point(25, 260);
            this._installButton.Name = "_installButton";
            this._installButton.Size = new System.Drawing.Size(146, 30);
            this._installButton.TabIndex = 2;
            this._installButton.Text = "▶️ Start Installation";
            this._installButton.UseVisualStyleBackColor = false;
            this._installButton.Click += new System.EventHandler(this.InstallButton_Click);
            // 
            // _cancelButton.
            // 
            this._cancelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this._cancelButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._cancelButton.Enabled = false;
            this._cancelButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this._cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._cancelButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._cancelButton.ForeColor = System.Drawing.Color.White;
            this._cancelButton.Location = new System.Drawing.Point(200, 260);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(80, 30);
            this._cancelButton.TabIndex = 3;
            this._cancelButton.Text = "❌ Cancel";
            this._cancelButton.UseVisualStyleBackColor = false;
            this._cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // _exportLogButton.
            // 
            this._exportLogButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(165)))), ((int)(((byte)(233)))));
            this._exportLogButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._exportLogButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(165)))), ((int)(((byte)(233)))));
            this._exportLogButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._exportLogButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._exportLogButton.ForeColor = System.Drawing.Color.White;
            this._exportLogButton.Location = new System.Drawing.Point(290, 260);
            this._exportLogButton.Name = "_exportLogButton";
            this._exportLogButton.Size = new System.Drawing.Size(107, 30);
            this._exportLogButton.TabIndex = 4;
            this._exportLogButton.Text = "💾 Export Log";
            this._exportLogButton.UseVisualStyleBackColor = false;
            this._exportLogButton.Click += new System.EventHandler(this.ExportLogButton_Click);
            // 
            // _quitButton.
            // 
            this._quitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this._quitButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._quitButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this._quitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._quitButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._quitButton.ForeColor = System.Drawing.Color.White;
            this._quitButton.Location = new System.Drawing.Point(538, 260);
            this._quitButton.Name = "_quitButton";
            this._quitButton.Size = new System.Drawing.Size(80, 30);
            this._quitButton.TabIndex = 6;
            this._quitButton.Text = "❌ Quit";
            this._quitButton.UseVisualStyleBackColor = false;
            this._quitButton.Click += new System.EventHandler(this.QuitButton_Click);
            // 
            // _aboutButton.
            // 
            this._aboutButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this._aboutButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this._aboutButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(114)))), ((int)(((byte)(128)))));
            this._aboutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._aboutButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._aboutButton.ForeColor = System.Drawing.Color.White;
            this._aboutButton.Location = new System.Drawing.Point(444, 260);
            this._aboutButton.Name = "_aboutButton";
            this._aboutButton.Size = new System.Drawing.Size(88, 30);
            this._aboutButton.TabIndex = 5;
            this._aboutButton.Text = "ℹ️ About";
            this._aboutButton.UseVisualStyleBackColor = false;
            this._aboutButton.Click += new System.EventHandler(this.AboutButton_Click);
            // 
            // _progressLabel.
            // 
            this._progressLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._progressLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._progressLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(85)))), ((int)(((byte)(99)))));
            this._progressLabel.Location = new System.Drawing.Point(27, 300);
            this._progressLabel.Name = "_progressLabel";
            this._progressLabel.Size = new System.Drawing.Size(912, 20);
            this._progressLabel.TabIndex = 5;
            this._progressLabel.Text = "Ready to install";
            // 
            // _progressBar.
            // 
            this._progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._progressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this._progressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(130)))), ((int)(((byte)(246)))));
            this._progressBar.Location = new System.Drawing.Point(27, 320);
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(912, 25);
            this._progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this._progressBar.TabIndex = 6;
            // 
            // _logGroup.
            // 
            this._logGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._logGroup.Controls.Add(this._logTextBox);
            this._logGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this._logGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this._logGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this._logGroup.Location = new System.Drawing.Point(7, 3);
            this._logGroup.Name = "_logGroup";
            this._logGroup.Padding = new System.Windows.Forms.Padding(15);
            this._logGroup.Size = new System.Drawing.Size(929, 311);
            this._logGroup.TabIndex = 7;
            this._logGroup.TabStop = false;
            this._logGroup.Text = "📜 Installation Log:";
            // 
            // _logTextBox.
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
            this._logTextBox.Size = new System.Drawing.Size(889, 271);
            this._logTextBox.TabIndex = 0;
            this._logTextBox.Text = "";
            // 
            // panel1.
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(251)))), ((int)(((byte)(252)))));
            this.panel1.Controls.Add(this._logGroup);
            this.panel1.Location = new System.Drawing.Point(20, 355);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(919, 281);
            this.panel1.TabIndex = 7;
            // 
            // MainForm.
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(251)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(963, 661);
            this.Controls.Add(this.panel1);
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
            this.MinimumSize = new System.Drawing.Size(840, 700);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Wampoon Installer";
            this._componentsGroup.ResumeLayout(false);
            this._pathGroup.ResumeLayout(false);
            this._pathGroup.PerformLayout();
            this._logGroup.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
    }
}