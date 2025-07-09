using System;

namespace PWAMP.Installer.Neo.Helpers.Logging
{
    public class LogEventArgs : EventArgs
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public string PackageName { get; set; }
        public Exception Exception { get; set; }
        public DateTime Timestamp { get; set; }

        public LogEventArgs(LogLevel level, string message, string packageName = null, Exception exception = null)
        {
            Level = level;
            Message = message;
            PackageName = packageName;
            Exception = exception;
            Timestamp = DateTime.Now;
        }
    }
}