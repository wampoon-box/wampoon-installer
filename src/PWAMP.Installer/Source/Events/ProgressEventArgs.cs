using System;

namespace PWAMP.Installer.Events
{
    /// <summary>
    /// Event arguments for progress events.
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        /// <summary>
        /// The exception that occurred, if any.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// The message associated with the progress event.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Whether the progress event is fatal.
        /// </summary>
        public bool IsFatal { get; set; }

        /// <summary>
        /// The stage of the installation.
        /// </summary>
        public InstallationStage Stage { get; set; }

        /// <summary>
        /// The package name.
        /// </summary>
        public string PackageName { get; set; }
    }
}