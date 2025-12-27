using System;

namespace Wampoon.Installer.Events
{
    /// <summary>
    /// Event args for reporting URL validation progress.
    /// </summary>
    public class UrlValidationProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Name of the package being validated.
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        /// The URL being validated.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Current package index (1-based).
        /// </summary>
        public int CurrentIndex { get; set; }

        /// <summary>
        /// Total number of packages to validate.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Status message: "Validating...", "Valid", or "Failed: {reason}".
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// True if validation succeeded for this URL.
        /// </summary>
        public bool IsValid { get; set; }
    }
}
