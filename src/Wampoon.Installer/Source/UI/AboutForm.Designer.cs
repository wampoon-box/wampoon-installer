using System.Drawing;
using System.Windows.Forms;

namespace Wampoon.Installer.UI
{
    partial class AboutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.mainPanel = new System.Windows.Forms.Panel();
            this.appInfoGroupBox = new System.Windows.Forms.GroupBox();
            this.appNameLabel = new System.Windows.Forms.Label();
            this.appVersionLabel = new System.Windows.Forms.Label();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.linksGroupBox = new System.Windows.Forms.GroupBox();
            this.gitHubRepoButton = new System.Windows.Forms.Button();
            this.gitHubIssuesButton = new System.Windows.Forms.Button();
            this.viewLicenseButton = new System.Windows.Forms.Button();
            this.creditsGroupBox = new System.Windows.Forms.GroupBox();
            this.licenseLabel = new System.Windows.Forms.Label();
            this.dependenciesLabel = new System.Windows.Forms.Label();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.okButton = new System.Windows.Forms.Button();
            this.mainPanel.SuspendLayout();
            this.appInfoGroupBox.SuspendLayout();
            this.linksGroupBox.SuspendLayout();
            this.creditsGroupBox.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.BackColor = System.Drawing.Color.White;
            this.mainPanel.Controls.Add(this.appInfoGroupBox);
            this.mainPanel.Controls.Add(this.linksGroupBox);
            this.mainPanel.Controls.Add(this.creditsGroupBox);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Location = new System.Drawing.Point(0, 0);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new System.Windows.Forms.Padding(20);
            this.mainPanel.Size = new System.Drawing.Size(514, 491);
            this.mainPanel.TabIndex = 0;
            // 
            // appInfoGroupBox
            // 
            this.appInfoGroupBox.Controls.Add(this.appNameLabel);
            this.appInfoGroupBox.Controls.Add(this.appVersionLabel);
            this.appInfoGroupBox.Controls.Add(this.copyrightLabel);
            this.appInfoGroupBox.Controls.Add(this.descriptionLabel);
            this.appInfoGroupBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.appInfoGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.appInfoGroupBox.Location = new System.Drawing.Point(20, 20);
            this.appInfoGroupBox.Name = "appInfoGroupBox";
            this.appInfoGroupBox.Padding = new System.Windows.Forms.Padding(15);
            this.appInfoGroupBox.Size = new System.Drawing.Size(457, 180);
            this.appInfoGroupBox.TabIndex = 0;
            this.appInfoGroupBox.TabStop = false;
            this.appInfoGroupBox.Text = "📋 Application Information";
            // 
            // appNameLabel
            // 
            this.appNameLabel.AutoSize = true;
            this.appNameLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.appNameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.appNameLabel.Location = new System.Drawing.Point(15, 30);
            this.appNameLabel.Name = "appNameLabel";
            this.appNameLabel.Size = new System.Drawing.Size(193, 30);
            this.appNameLabel.TabIndex = 0;
            this.appNameLabel.Text = "Wampoon Admin";
            // 
            // appVersionLabel
            // 
            this.appVersionLabel.AutoSize = true;
            this.appVersionLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.appVersionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.appVersionLabel.Location = new System.Drawing.Point(15, 65);
            this.appVersionLabel.Name = "appVersionLabel";
            this.appVersionLabel.Size = new System.Drawing.Size(94, 19);
            this.appVersionLabel.TabIndex = 1;
            this.appVersionLabel.Text = "Version 1.0.0";
            // 
            // copyrightLabel
            // 
            this.copyrightLabel.AutoSize = true;
            this.copyrightLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.copyrightLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.copyrightLabel.Location = new System.Drawing.Point(15, 90);
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Size = new System.Drawing.Size(161, 15);
            this.copyrightLabel.TabIndex = 2;
            this.copyrightLabel.Text = "Copyright © 2025 - frostybee";
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.descriptionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.descriptionLabel.Location = new System.Drawing.Point(15, 115);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(414, 50);
            this.descriptionLabel.TabIndex = 3;
            this.descriptionLabel.Text = "A comprehensive control panel for managing Apache and MySQL servers in the WAMPoo" +
    "n environment.";
            // 
            // linksGroupBox
            // 
            this.linksGroupBox.Controls.Add(this.gitHubRepoButton);
            this.linksGroupBox.Controls.Add(this.gitHubIssuesButton);
            this.linksGroupBox.Controls.Add(this.viewLicenseButton);
            this.linksGroupBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.linksGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.linksGroupBox.Location = new System.Drawing.Point(20, 210);
            this.linksGroupBox.Name = "linksGroupBox";
            this.linksGroupBox.Padding = new System.Windows.Forms.Padding(15);
            this.linksGroupBox.Size = new System.Drawing.Size(457, 110);
            this.linksGroupBox.TabIndex = 1;
            this.linksGroupBox.TabStop = false;
            this.linksGroupBox.Text = "🔗 Project Links";
            // 
            // gitHubRepoButton
            // 
            this.gitHubRepoButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(41)))), ((int)(((byte)(60)))));
            this.gitHubRepoButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.gitHubRepoButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(41)))), ((int)(((byte)(60)))));
            this.gitHubRepoButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gitHubRepoButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.gitHubRepoButton.ForeColor = System.Drawing.Color.White;
            this.gitHubRepoButton.Location = new System.Drawing.Point(15, 30);
            this.gitHubRepoButton.Name = "gitHubRepoButton";
            this.gitHubRepoButton.Size = new System.Drawing.Size(140, 35);
            this.gitHubRepoButton.TabIndex = 0;
            this.gitHubRepoButton.Text = "🔗 GitHub Repository";
            this.gitHubRepoButton.UseVisualStyleBackColor = false;
            this.gitHubRepoButton.Click += new System.EventHandler(this.GitHubRepoButton_Click);
            // 
            // gitHubIssuesButton
            // 
            this.gitHubIssuesButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.gitHubIssuesButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.gitHubIssuesButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(53)))), ((int)(((byte)(69)))));
            this.gitHubIssuesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gitHubIssuesButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.gitHubIssuesButton.ForeColor = System.Drawing.Color.White;
            this.gitHubIssuesButton.Location = new System.Drawing.Point(161, 30);
            this.gitHubIssuesButton.Name = "gitHubIssuesButton";
            this.gitHubIssuesButton.Size = new System.Drawing.Size(139, 35);
            this.gitHubIssuesButton.TabIndex = 1;
            this.gitHubIssuesButton.Text = "🐛 Report a Bug";
            this.gitHubIssuesButton.UseVisualStyleBackColor = false;
            this.gitHubIssuesButton.Click += new System.EventHandler(this.GitHubIssuesButton_Click);
            // 
            // viewLicenseButton
            // 
            this.viewLicenseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.viewLicenseButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.viewLicenseButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.viewLicenseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.viewLicenseButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.viewLicenseButton.ForeColor = System.Drawing.Color.White;
            this.viewLicenseButton.Location = new System.Drawing.Point(306, 30);
            this.viewLicenseButton.Name = "viewLicenseButton";
            this.viewLicenseButton.Size = new System.Drawing.Size(139, 35);
            this.viewLicenseButton.TabIndex = 2;
            this.viewLicenseButton.Text = "📄 View License";
            this.viewLicenseButton.UseVisualStyleBackColor = false;
            this.viewLicenseButton.Click += new System.EventHandler(this.ViewLicenseButton_Click);
            // 
            // creditsGroupBox
            // 
            this.creditsGroupBox.Controls.Add(this.licenseLabel);
            this.creditsGroupBox.Controls.Add(this.dependenciesLabel);
            this.creditsGroupBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.creditsGroupBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(62)))), ((int)(((byte)(80)))));
            this.creditsGroupBox.Location = new System.Drawing.Point(20, 330);
            this.creditsGroupBox.Name = "creditsGroupBox";
            this.creditsGroupBox.Padding = new System.Windows.Forms.Padding(15);
            this.creditsGroupBox.Size = new System.Drawing.Size(457, 100);
            this.creditsGroupBox.TabIndex = 2;
            this.creditsGroupBox.TabStop = false;
            this.creditsGroupBox.Text = "📄 License & Credits";
            // 
            // licenseLabel
            // 
            this.licenseLabel.AutoSize = true;
            this.licenseLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.licenseLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.licenseLabel.Location = new System.Drawing.Point(15, 30);
            this.licenseLabel.Name = "licenseLabel";
            this.licenseLabel.Size = new System.Drawing.Size(172, 15);
            this.licenseLabel.TabIndex = 0;
            this.licenseLabel.Text = "Licensed under the MIT License";
            // 
            // dependenciesLabel
            // 
            this.dependenciesLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dependenciesLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dependenciesLabel.Location = new System.Drawing.Point(15, 50);
            this.dependenciesLabel.Name = "dependenciesLabel";
            this.dependenciesLabel.Size = new System.Drawing.Size(414, 35);
            this.dependenciesLabel.TabIndex = 1;
            this.dependenciesLabel.Text = "Dependencies: Newtonsoft.Json by James Newton-King";
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.okButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 440);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(514, 51);
            this.buttonPanel.TabIndex = 1;
            // 
            // okButton
            // 
            this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.okButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.okButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(123)))), ((int)(((byte)(255)))));
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.okButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.okButton.ForeColor = System.Drawing.Color.White;
            this.okButton.Location = new System.Drawing.Point(220, 10);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 30);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = false;
            this.okButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(514, 491);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.mainPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About - Wampoon Admin";
            this.mainPanel.ResumeLayout(false);
            this.appInfoGroupBox.ResumeLayout(false);
            this.appInfoGroupBox.PerformLayout();
            this.linksGroupBox.ResumeLayout(false);
            this.creditsGroupBox.ResumeLayout(false);
            this.creditsGroupBox.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.GroupBox appInfoGroupBox;
        private System.Windows.Forms.Label appNameLabel;
        private System.Windows.Forms.Label appVersionLabel;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.GroupBox linksGroupBox;
        private System.Windows.Forms.Button gitHubRepoButton;
        private System.Windows.Forms.Button gitHubIssuesButton;
        private System.Windows.Forms.Button viewLicenseButton;
        private System.Windows.Forms.GroupBox creditsGroupBox;
        private System.Windows.Forms.Label licenseLabel;
        private System.Windows.Forms.Label dependenciesLabel;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button okButton;
    }
}