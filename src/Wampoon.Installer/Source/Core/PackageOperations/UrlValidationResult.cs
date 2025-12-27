using System.Collections.Generic;

namespace Wampoon.Installer.Core.PackageOperations
{
    /// <summary>
    /// Result of validating package download URLs before installation.
    /// </summary>
    public class UrlValidationResult
    {
        /// <summary>
        /// True if all package URLs are accessible, false if any failed.
        /// </summary>
        public bool AllUrlsValid { get; set; }

        /// <summary>
        /// List of validation failures. Empty if all URLs are valid.
        /// </summary>
        public List<UrlValidationFailure> Failures { get; set; }

        public UrlValidationResult()
        {
            Failures = new List<UrlValidationFailure>();
        }
    }

    /// <summary>
    /// Details about a failed URL validation.
    /// </summary>
    public class UrlValidationFailure
    {
        /// <summary>
        /// Name of the package with the invalid URL.
        /// </summary>
        public string PackageName { get; set; }

        /// <summary>
        /// The URL that failed validation.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Human-readable reason for the failure (e.g., "HTTP 404 Not Found", "Request timed out").
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// HTTP status code if applicable, null for network/timeout errors.
        /// </summary>
        public int? HttpStatusCode { get; set; }

        /// <summary>
        /// Official website where users can find valid download URLs for this package.
        /// </summary>
        public string PackageWebsite { get; set; }
    }
}
