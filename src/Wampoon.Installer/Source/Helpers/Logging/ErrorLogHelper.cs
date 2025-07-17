using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wampoon.Installer.UI;

namespace Wampoon.Installer.Helpers
{

    internal static class ErrorLogHelper
    {
        private static readonly object _lock = new object();
        private static readonly string _logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppConstants.APP_LOG_FOLDER);
        private static readonly string _logFilePath = Path.Combine(_logDirectory, AppConstants.APP_LOG_FILE);

        static ErrorLogHelper()
        {
            try
            {
                if (!Directory.Exists(_logDirectory))
                {
                    Directory.CreateDirectory(_logDirectory);
                }
            }
            catch
            {
                // Silently fail if we can't create the log directory.
            }
        }

        internal static void LogExceptionInfo(Exception ex)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " +
                                $"Error: {ex.Message}{Environment.NewLine}" +
                                $"Exception: {ex.GetType().Name}{Environment.NewLine}" +                                
                                $"Stack Trace:\n{ex.StackTrace}{Environment.NewLine}" +
                                new string('-', 80) + $"{Environment.NewLine}";
            WriteToFile(logEntry);
        }

        internal static void LogExceptionInfo(string message)
        {
            var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR: {message}{Environment.NewLine}";
            WriteToFile(logEntry);
        }

        internal static void ShowErrorReport(Exception exception, string additionalInfo = "", IWin32Window owner = null)
        {
            try
            {
                LogExceptionInfo(exception);
                
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
                LogExceptionInfo(ex);
                MessageBox.Show($"An error occurred while displaying the error report: {ex.Message}", 
                    "Error Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void WriteToFile(string message)
        {
            try
            {
                lock (_lock)
                {
                    if (!Directory.Exists(_logDirectory))
                    {
                        Directory.CreateDirectory(_logDirectory);
                    }

                    // Check if rotation is needed.
                    if (IsRotateRequired())
                    {
                        RotateLogFile();
                    }

                    using (var stream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(message);
                    }
                }
            }
            catch
            {
                // Silently fail to prevent logger from crashing the application.
            }
        }

        private static bool IsRotateRequired()
        {
            try
            {
                if (!File.Exists(_logFilePath))
                    return false;

                var fileInfo = new FileInfo(_logFilePath);
                var fileAge = DateTime.Now - fileInfo.CreationTime;
                // 2 months (approximately).
                return fileAge.TotalDays >= 60; 
            }
            catch
            {
                return false;
            }
        }

        private static void RotateLogFile()
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyyMM");
                var rotatedFileName = $"error_{timestamp}.log";
                var rotatedFilePath = Path.Combine(_logDirectory, rotatedFileName);

                File.Move(_logFilePath, rotatedFilePath);
            }
            catch
            {
                // Silently fail. This is to ensure that the logger does not crash the application if rotation fails.
            }
        }
    }
}
