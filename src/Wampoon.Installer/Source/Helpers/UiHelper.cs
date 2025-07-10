using Wampoon.Installer.UI;
using System;
using System.Collections.Generic;
using System.Linq;
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
                //LogExceptionInfo(ex);
                MessageBox.Show($"An error occurred while displaying the error report: {ex.Message}",
                    "Error Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
