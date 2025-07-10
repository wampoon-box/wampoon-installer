using System.Collections.Generic;

namespace Wampoon.Installer.Core.Paths
{
    /// <summary>
    /// Defines the path configuration contract for a Wampoon package.    
    /// </summary>
    public interface IPackagePathConfiguration
    {
        /// <summary>
        /// Gets the unique identifier for this package.
        /// </summary>
        string PackageName { get; }

        /// <summary>
        /// Gets the default directory where binaries are located relative to the package root.
        /// </summary>
        string BinaryDirectory { get; }

        /// <summary>
        /// Gets the default directory where configuration files are located relative to the package root.
        /// </summary>
        string ConfigDirectory { get; }

        /// <summary>
        /// Gets a dictionary mapping binary names to their relative paths within the package.
        /// Key: Binary name (e.g., "httpd.exe")
        /// Value: Relative path from package root (e.g., "bin/httpd.exe")
        /// </summary>
        Dictionary<string, string> BinaryFiles { get; }

        /// <summary>
        /// Gets a dictionary mapping configuration file names to their relative paths within the package.
        /// Key: Configuration name (e.g., "httpd.conf")
        /// Value: Relative path from package root (e.g., "conf/httpd.conf")
        /// </summary>
        Dictionary<string, string> ConfigFiles { get; }

        /// <summary>
        /// Gets a dictionary mapping subdirectory names to their relative paths within the package.
        /// Key: Subdirectory name (e.g., "conf", "logs")
        /// Value: Relative path from package root (e.g., "conf", "logs")
        /// </summary>
        Dictionary<string, string> Subdirectories { get; }

        /// <summary>
        /// Gets alternative paths for binaries (e.g., for Apache24 nested structure).
        /// Key: Binary name
        /// Value: List of alternative relative paths to try
        /// </summary>
        Dictionary<string, List<string>> AlternativeBinaryPaths { get; }
    }
}