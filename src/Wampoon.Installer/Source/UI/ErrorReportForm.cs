
using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Wampoon.Installer.Helpers;

namespace Wampoon.Installer.UI
{
    public partial class ErrorReportForm : Form
    {
        private readonly Exception _exception;
        private readonly string _additionalInfo;
        
        public ErrorReportForm(Exception exception, string additionalInfo = "")
        {
            _exception = exception;
            _additionalInfo = additionalInfo;
            
            InitializeComponent();
            CenterToScreen();
            LoadExceptionData();
        }

        private void LoadExceptionData()
        {
            Text = $"{AppConstants.APP_FULL_NAME} -  Error Report";
            
            errorMessageTextBox.Text = _exception.Message;
            exceptionTypeLabel.Text = $"Exception Type: {_exception.GetType().Name}";
            
            var stackTrace = _exception.StackTrace ?? "No stack trace available";
            stackTraceTextBox.Text = stackTrace;
            
            if (!string.IsNullOrWhiteSpace(_additionalInfo))
            {
                additionalInfoTextBox.Text = _additionalInfo;
            }
            
            timestampLabel.Text = $"Occurred: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
        }

        private void CopyToClipboardButton_Click(object sender, EventArgs e)
        {
            try
            {
                var errorReport = GenerateErrorReport();
                Clipboard.SetText(errorReport);
                MessageBox.Show("Error details copied to clipboard.", "Copied", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogExceptionInfo(ex);
                MessageBox.Show($"Failed to copy to clipboard: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReportIssueButton_Click(object sender, EventArgs e)
        {
            try
            {
                var issueUrl = GenerateGitHubIssueUrl();
                Process.Start(issueUrl);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogExceptionInfo(ex);
                MessageBox.Show($"Failed to open GitHub: {ex.Message}\n\nYou can report this issue manually at: {AppConstants.GITHUB_REPO_URI}/issues", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private string GenerateErrorReport()
        {
            var report = new StringBuilder();
            report.AppendLine($"{AppConstants.APP_FULL_NAME} Error Report");
            report.AppendLine("====================================");
            report.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            report.AppendLine($"Exception Type: {_exception.GetType().Name}");
            report.AppendLine($"Message: {_exception.Message}");
            report.AppendLine();
            
            if (!string.IsNullOrWhiteSpace(_additionalInfo))
            {
                report.AppendLine("Additional Information:");
                report.AppendLine(_additionalInfo);
                report.AppendLine();
            }
            
            report.AppendLine("Stack Trace:");
            report.AppendLine(_exception.StackTrace ?? "No stack trace available");
            
            if (_exception.InnerException != null)
            {
                report.AppendLine();
                report.AppendLine("Inner Exception:");
                report.AppendLine($"Type: {_exception.InnerException.GetType().Name}");
                report.AppendLine($"Message: {_exception.InnerException.Message}");
                if (!string.IsNullOrWhiteSpace(_exception.InnerException.StackTrace))
                {
                    report.AppendLine("Stack Trace:");
                    report.AppendLine(_exception.InnerException.StackTrace);
                }
            }
            
            report.AppendLine();
            report.AppendLine("System Information:");
            report.AppendLine($"OS: {Environment.OSVersion}");
            report.AppendLine($"Runtime: {Environment.Version}");
            report.AppendLine($"Working Directory: {Environment.CurrentDirectory}");
            
            return report.ToString();
        }

        private string GenerateGitHubIssueUrl()
        {
            var title = $"Runtime Error: {_exception.GetType().Name}";
            var body = new StringBuilder();
            
            body.AppendLine("## Error Description");
            body.AppendLine($"**Exception Type:** {_exception.GetType().Name}");
            body.AppendLine($"**Message:** {_exception.Message}");
            body.AppendLine($"**Timestamp:** {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            body.AppendLine();
            
            if (!string.IsNullOrWhiteSpace(_additionalInfo))
            {
                body.AppendLine("## Additional Information");
                body.AppendLine(_additionalInfo);
                body.AppendLine();
            }
            
            body.AppendLine("## Stack Trace");
            body.AppendLine("```");
            body.AppendLine(_exception.StackTrace ?? "No stack trace available");
            body.AppendLine("```");
            
            if (_exception.InnerException != null)
            {
                body.AppendLine();
                body.AppendLine("## Inner Exception");
                body.AppendLine($"**Type:** {_exception.InnerException.GetType().Name}");
                body.AppendLine($"**Message:** {_exception.InnerException.Message}");
                if (!string.IsNullOrWhiteSpace(_exception.InnerException.StackTrace))
                {
                    body.AppendLine("```");
                    body.AppendLine(_exception.InnerException.StackTrace);
                    body.AppendLine("```");
                }
            }
            
            body.AppendLine();
            body.AppendLine("## System Information");
            body.AppendLine($"- **OS:** {Environment.OSVersion}");
            body.AppendLine($"- **Runtime:** {Environment.Version}");
            body.AppendLine();
            body.AppendLine("## Steps to Reproduce");
            body.AppendLine("1. ");
            body.AppendLine("2. ");
            body.AppendLine("3. ");
            
            var encodedTitle = WebUtility.UrlEncode(title);
            var encodedBody = WebUtility.UrlEncode(body.ToString());
            
            return $"{AppConstants.GITHUB_REPO_URI}/issues/new?title={encodedTitle}&body={encodedBody}&labels=bug";
        }
    }
}