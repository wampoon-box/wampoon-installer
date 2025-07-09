using System;
using PWAMP.Installer.Neo.Events;

namespace PWAMP.Installer.Neo.Helpers.Logging
{
    public interface ILogger
    {
        void LogInfo(string message);
        void LogInfo(string message, string packageName);
        void LogWarning(string message);
        void LogWarning(string message, string packageName);
        void LogError(string message);
        void LogError(string message, string packageName);
        void LogError(string message, Exception exception);
        void LogError(string message, Exception exception, string packageName);
        void LogProgress(string operation, int completedSteps, int totalSteps, string packageName = null);
        void LogDownloadProgress(string packageName, long bytesReceived, long totalBytes, double speed = 0);
        void LogInstallationCompleted(bool success, string packageName, string installPath, TimeSpan duration, string message = null);
    }
}