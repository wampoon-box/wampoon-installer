using System;

namespace PWAMP.Installer.Events
{
    public class DownloadProgressEventArgs : EventArgs
    {
        public string PackageName { get; set; }
        public long BytesReceived { get; set; }
        public long TotalBytes { get; set; }
        public double PercentComplete { get; set; }
        public TimeSpan TimeRemaining { get; set; }
        public double DownloadSpeed { get; set; }
    }

    public class InstallationProgressEventArgs : EventArgs
    {
        public string CurrentOperation { get; set; }
        public int CompletedSteps { get; set; }
        public int TotalSteps { get; set; }
        public double PercentComplete { get; set; }
        public string PackageName { get; set; }
    }

    public class ErrorEventArgs : EventArgs
    {
        public Exception Exception { get; set; }
        public string Message { get; set; }
        public string PackageName { get; set; }
        public bool IsFatal { get; set; }
    }
}