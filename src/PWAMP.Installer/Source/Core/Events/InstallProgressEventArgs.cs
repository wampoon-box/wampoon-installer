using System;

namespace PWAMP.Installer.Neo.Core.Events
{
    public class InstallProgressEventArgs : EventArgs
    {
        public string Message { get; }
        public int PercentComplete { get; }
        public string CurrentStep { get; }

        public InstallProgressEventArgs(string message, int percentComplete = 0, string currentStep = "")
        {
            Message = message;
            PercentComplete = percentComplete;
            CurrentStep = currentStep;
        }
    }
}