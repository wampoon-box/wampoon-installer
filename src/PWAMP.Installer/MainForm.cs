using System;
using System.Drawing;
using System.Windows.Forms;
using PWAMP.Installer.UI;

namespace PWAMP.Installer
{
    public partial class MainForm : Form
    {
        private Button _installButton;
        private Label _titleLabel;
        private Label _descriptionLabel;

        public MainForm()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "PWAMP Installer";
            this.Size = new Size(500, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            _titleLabel = new Label
            {
                Text = "PWAMP Server Installer",
                Font = new Font("Microsoft Sans Serif", 16, FontStyle.Bold),
                Location = new Point(50, 30),
                Size = new Size(400, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            _descriptionLabel = new Label
            {
                Text = "Install Apache HTTP Server, MariaDB, PHP, and phpMyAdmin\ncomponents for your local development environment.",
                Location = new Point(50, 80),
                Size = new Size(400, 60),
                TextAlign = ContentAlignment.MiddleCenter
            };

            _installButton = new Button
            {
                Text = "Start Installation Wizard",
                Location = new Point(175, 170),
                Size = new Size(150, 40),
                UseVisualStyleBackColor = true,
                Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular)
            };

            _installButton.Click += InstallButton_Click;

            this.Controls.AddRange(new Control[] { _titleLabel, _descriptionLabel, _installButton });

            var exitButton = new Button
            {
                Text = "Exit",
                Location = new Point(350, 170),
                Size = new Size(75, 40),
                UseVisualStyleBackColor = true
            };

            exitButton.Click += (s, e) => this.Close();
            this.Controls.Add(exitButton);
        }

        private void InstallButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var wizard = new InstallerWizardForm())
                {
                    var result = wizard.ShowDialog(this);
                    
                    if (result == DialogResult.OK)
                    {
                        MessageBox.Show("Installation completed successfully!\n\nYou can now use the PWAMP Control Panel to manage your servers.", 
                            "Installation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Failed to start installation wizard: {0}", ex.Message), 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
