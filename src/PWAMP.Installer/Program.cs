using Wampoon.Installer.Helpers;
using Wampoon.Installer.UI;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wampoon.Installer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the Wampoon installer application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Set up global exception handlers before running the application.
            SetupGlobalExceptionHandlers();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            try
            {
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", 
                    $"{AppConstants.APP_FULL_NAME} Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
            }
        }
        private static void SetupGlobalExceptionHandlers()
        {
            // Handle UI thread exceptions.
            Application.ThreadException += Application_ThreadException;

            // Handle non-UI thread exceptions.
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Handle Task exceptions (for async/await scenarios).
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            // Set the unhandled exception mode.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        }
        // Handles exceptions on the UI thread.
        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            try
            {
                UiHelper.ShowErrorReport(e.Exception, "UI Thread Exception");

                DialogResult result = MessageBox.Show(
                    "An unexpected error occurred. Do you want to continue running the application?",
                    "Application Error",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    Application.Exit();
                }
            }
            catch
            {
                // Fallback if error reporting fails.
                MessageBox.Show($"A critical error occurred: {e.Exception.Message}", "Fatal Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        // Handles exceptions on non-UI threads.
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;

            try
            {
                UiHelper.ShowErrorReport(ex, "Non-UI Thread Exception - Application will close");
            }
            catch
            {
                // Fallback if error reporting fails.
                MessageBox.Show($"A fatal error occurred: {ex.Message}\n\nThe application will now close.",
                    "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

            // Application is terminating, perform cleanup if needed.
            Environment.Exit(1);
        }

        // Handles unobserved Task exceptions.
        private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            // Mark as observed to prevent application termination.
            e.SetObserved();

            try
            {
                UiHelper.ShowErrorReport(e.Exception.GetBaseException(), "Unobserved Task Exception");
            }
            catch
            {
                // Fallback if error reporting fails.
                MessageBox.Show($"An error occurred in a background task: {e.Exception.GetBaseException().Message}",
                    "Background Task Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}