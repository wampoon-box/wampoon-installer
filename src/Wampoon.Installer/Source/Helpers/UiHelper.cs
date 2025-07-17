using Wampoon.Installer.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Wampoon.Installer.Helpers
{
    internal class UiHelper
    {
        internal static void ShowErrorReport(Exception exception, string additionalInfo = "", IWin32Window owner = null)
        {
            try
            {
             ///   LogExceptionInfo(exception);

                using (var errorForm = new ErrorReportForm(exception, additionalInfo))
                {
                    if (owner != null)
                    {
                        errorForm.ShowDialog(owner);
                    }
                    else
                    {
                        errorForm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogExceptionInfo(ex);
                MessageBox.Show($"An error occurred while displaying the error report: {ex.Message}",
                    "Error Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gets the current version of the installer assembly.
        /// </summary>
        /// <returns>The version string (e.g., "1.0.0.0") or "Unknown" if version cannot be determined.</returns>
        internal static string GetInstallerVersion()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                return version?.ToString() ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }

        /// <summary>
        /// Gets a formatted version string for display purposes.
        /// </summary>
        /// <returns>A formatted version string (e.g., "v1.0.0") or empty string if version cannot be determined.</returns>
        internal static string GetFormattedInstallerVersion()
        {
            var version = GetInstallerVersion();
            if (version == "Unknown")
                return "";
            
            // Remove the last .0 if it exists for cleaner display (e.g., "1.0.0.0" becomes "v1.0.0")
            if (version.EndsWith(".0"))
                version = version.Substring(0, version.LastIndexOf(".0"));
                
            return $"v{version}";
        }
    }
}
