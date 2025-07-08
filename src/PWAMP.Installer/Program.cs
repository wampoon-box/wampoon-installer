using System;
using System.Windows.Forms;
using PWAMP.Installer.Neo.UI;

namespace PWAMP.Installer.Neo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the Neo PWAMP installer application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", 
                    "PWAMP Installer Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }
    }
}