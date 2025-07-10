using System;

namespace Wampoon.Installer.Events
{
    public class DownloadProgressEventArgs : EventArgs
    {
        public string PackageName { get; set; }
        public long BytesReceived { get; set; }
        public long TotalBytes { get; set; }
        public double PercentComplete { get; set; }
        public TimeSpan TimeRemaining { get; set; }
        public double DownloadSpeed { get; set; }
        public string Status { get; set; }
    }

    public class InstallationProgressEventArgs : EventArgs
    {
        public string CurrentOperation { get; set; }
        public int CompletedSteps { get; set; }
        public int TotalSteps { get; set; }
        public double PercentComplete { get; set; }
        public string PackageName { get; set; }
        public InstallationStage Stage { get; set; }
    }

    public class InstallerErrorEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
        public string Message { get; set; }
        public string PackageName { get; set; }
        public bool IsFatal { get; set; }
        public ErrorType ErrorType { get; set; }
    }

    public class InstallationCompletedEventArgs : EventArgs
    {
        public bool Success { get; set; }
        public string PackageName { get; set; }
        public string InstallPath { get; set; }
        public TimeSpan Duration { get; set; }
        public string Message { get; set; }
    }

    public enum InstallationStage
    {
        Initializing,
        Downloading,
        Extracting,
        Configuring,
        Validating,
        Completed,
        Failed
    }

    public enum ErrorType
    {
        Download,
        Extraction,
        Configuration,
        Validation,
        Permission,
        Network,
        FileSystem,
        Unknown
    }
}