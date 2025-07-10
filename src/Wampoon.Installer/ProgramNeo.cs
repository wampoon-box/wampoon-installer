using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Pwamp.Neo.Installer.UI;
using Pwamp.Neo.Installer.UI.Steps;

namespace PWAMP.Installer
{
    /// <summary>
    /// New entry point using the Neo wizard framework
    /// This replaces the original monolithic MainForm with a clean step-based wizard
    /// </summary>
    internal static class ProgramNeo
    {
        [STAThread]
        static async Task Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Create the main wizard form
                var wizardForm = new MainForm();
                
                // Add all wizard steps in order
                wizardForm.AddStep(new WelcomeStep());
                wizardForm.AddStep(new ConfigurationStep());
                wizardForm.AddStep(new ComponentSelectionStep());
                wizardForm.AddStep(new InstallationStep());
                wizardForm.AddStep(new CompletionStep());

                // Start the wizard
                await wizardForm.StartWizardAsync();
                
                // Run the application
                Application.Run(wizardForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start installer: {ex.Message}", 
                    "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}