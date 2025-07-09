using System;
using Wampoon.Installer.Events;

namespace Wampoon.Installer.Helpers.Logging
{
    public class EventBasedLogger : ILogger
    {
        public event EventHandler<LogEventArgs> LogMessage;
        public event EventHandler<InstallationProgressEventArgs> ProgressReported;
        public event EventHandler<DownloadProgressEventArgs> DownloadProgressReported;
        public event EventHandler<InstallationCompletedEventArgs> InstallationCompleted;
        public event EventHandler<InstallerErrorEventArgs> ErrorOccurred;

        public void LogInfo(string message)
        {
            LogInfo(message, null);
        }

        public void LogInfo(string message, string packageName)
        {
            OnLogMessage(new LogEventArgs(LogLevel.Info, message, packageName));
        }

        public void LogWarning(string message)
        {
            LogWarning(message, null);
        }

        public void LogWarning(string message, string packageName)
        {
            OnLogMessage(new LogEventArgs(LogLevel.Warning, message, packageName));
        }

        public void LogError(string message)
        {
            LogError(message, null, null);
        }

        public void LogError(string message, string packageName)
        {
            LogError(message, null, packageName);
        }

        public void LogError(string message, Exception exception)
        {
            LogError(message, exception, null);
        }

        public void LogError(string message, Exception exception, string packageName)
        {
            OnLogMessage(new LogEventArgs(LogLevel.Error, message, packageName, exception));
            
            // Also raise the error event for existing error handling.
            OnErrorOccurred(new InstallerErrorEventArgs
            {
                Message = message,
                Exception = exception,
                PackageName = packageName,
                IsFatal = false,
                ErrorType = ErrorType.Unknown
            });
        }

        public void LogProgress(string operation, int completedSteps, int totalSteps, string packageName = null)
        {
            var percentComplete = totalSteps > 0 ? (double)completedSteps / totalSteps * 100 : 0;
            
            OnLogMessage(new LogEventArgs(LogLevel.Progress, operation, packageName));
            
            // Also raise the progress event for existing progress handling.
            OnProgressReported(new InstallationProgressEventArgs
            {
                CurrentOperation = operation,
                CompletedSteps = completedSteps,
                TotalSteps = totalSteps,
                PercentComplete = percentComplete,
                PackageName = packageName,
                Stage = InstallationStage.Configuring
            });
        }

        public void LogDownloadProgress(string packageName, long bytesReceived, long totalBytes, double speed = 0)
        {
            var percentComplete = totalBytes > 0 ? (double)bytesReceived / totalBytes * 100 : 0;
            var message = $"Downloaded {FormatBytes(bytesReceived)} of {FormatBytes(totalBytes)} ({percentComplete:F1}%)";
            
            OnLogMessage(new LogEventArgs(LogLevel.DownloadProgress, message, packageName));
            
            // Also raise the download progress event for existing download progress handling.
            OnDownloadProgressReported(new DownloadProgressEventArgs
            {
                PackageName = packageName,
                BytesReceived = bytesReceived,
                TotalBytes = totalBytes,
                PercentComplete = percentComplete,
                DownloadSpeed = speed,
                Status = "Downloading"
            });
        }

        public void LogInstallationCompleted(bool success, string packageName, string installPath, TimeSpan duration, string message = null)
        {
            var logMessage = message ?? (success ? "Installation completed successfully" : "Installation failed");
            
            OnLogMessage(new LogEventArgs(LogLevel.InstallationCompleted, logMessage, packageName));
            
            // Also raise the installation completed event for existing completion handling.
            OnInstallationCompleted(new InstallationCompletedEventArgs
            {
                Success = success,
                PackageName = packageName,
                InstallPath = installPath,
                Duration = duration,
                Message = logMessage
            });
        }

        protected virtual void OnLogMessage(LogEventArgs e)
        {
            LogMessage?.Invoke(this, e);
        }

        protected virtual void OnProgressReported(InstallationProgressEventArgs e)
        {
            ProgressReported?.Invoke(this, e);
        }

        protected virtual void OnDownloadProgressReported(DownloadProgressEventArgs e)
        {
            DownloadProgressReported?.Invoke(this, e);
        }

        protected virtual void OnInstallationCompleted(InstallationCompletedEventArgs e)
        {
            InstallationCompleted?.Invoke(this, e);
        }

        protected virtual void OnErrorOccurred(InstallerErrorEventArgs e)
        {
            ErrorOccurred?.Invoke(this, e);
        }

        private string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = { "GB", "MB", "KB", "Bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);
                max /= scale;
            }
            return "0 Bytes";
        }
    }
}