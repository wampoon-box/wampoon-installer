using Wampoon.Installer.Helpers;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Frostybee.Pwamp.UI
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            CenterToScreen();
            LoadApplicationInfo();
        }

        private void LoadApplicationInfo()
        {
            Text = $"About {AppConstants.APP_NAME}";
            appNameLabel.Text = AppConstants.APP_NAME;
            //appVersionLabel.Text = $"Version {AppConstants.APP_VERSION}";
            copyrightLabel.Text = "Copyright © 2025 - frostybee";
            descriptionLabel.Text = "An installer for setting up WAMPoon, a local development environment with Apache HTTP Server, MariaDB, PHP, and phpMyAdmin.";
        }

        private void GitHubRepoButton_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(AppConstants.GITHUB_REPO_URI);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open GitHub repository: {ex.Message}\n\nYou can visit the repository manually at: {AppConstants.GITHUB_REPO_URI}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void GitHubIssuesButton_Click(object sender, EventArgs e)
        {
            try
            {
                var issuesUrl = $"{AppConstants.GITHUB_REPO_URI}/issues";
                Process.Start(issuesUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open GitHub Issues: {ex.Message}\n\nYou can visit Issues manually at: {AppConstants.GITHUB_REPO_URI}/issues", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}