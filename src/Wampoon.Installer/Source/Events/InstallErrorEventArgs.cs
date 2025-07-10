using System;

namespace Wampoon.Installer.Core.Events
{
    public class InstallErrorEventArgs : EventArgs
    {
        public string ErrorMessage { get; }
        public Exception Exception { get; }
        public string Component { get; }

        public InstallErrorEventArgs(string errorMessage, Exception exception = null, string component = "")
        {
            ErrorMessage = errorMessage;
            Exception = exception;
            Component = component;
        }
    }
}