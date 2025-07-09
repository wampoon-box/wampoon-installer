using System;

namespace Wampoon.Installer.Core.Paths
{
    /// <summary>
    /// Provides centralized path resolution for PWAMP package installations.
    /// </summary>
    public interface IPathResolver
    {
        /// <summary>
        /// Gets the root directory where the specified package is installed.
        /// </summary>
        /// <param name="packageName">The name of the package (e.g., "apache", "mariadb")</param>
        /// <returns>The full path to the package directory</returns>
        string GetPackageDirectory(string packageName);

        /// <summary>
        /// Gets the full path to a binary file for the specified package.
        /// </summary>
        /// <param name="packageName">The name of the package</param>
        /// <param name="binaryName">The name of the binary file (e.g., "httpd.exe", "mysqld.exe")</param>
        /// <returns>The full path to the binary file</returns>
        string GetBinaryPath(string packageName, string binaryName);

        /// <summary>
        /// Gets the full path to a configuration file for the specified package.
        /// </summary>
        /// <param name="packageName">The name of the package</param>
        /// <param name="configName">The name of the configuration file (e.g., "httpd.conf", "php.ini")</param>
        /// <returns>The full path to the configuration file</returns>
        string GetConfigPath(string packageName, string configName);

        /// <summary>
        /// Gets the full path to a subdirectory within the package directory.
        /// </summary>
        /// <param name="packageName">The name of the package</param>
        /// <param name="subdirectory">The subdirectory name (e.g., "conf", "logs", "bin")</param>
        /// <returns>The full path to the subdirectory</returns>
        string GetSubdirectoryPath(string packageName, string subdirectory);

        /// <summary>
        /// Gets the full path to a specific file within a subdirectory.
        /// </summary>
        /// <param name="packageName">The name of the package</param>
        /// <param name="subdirectory">The subdirectory name</param>
        /// <param name="fileName">The file name</param>
        /// <returns>The full path to the file</returns>
        string GetSubdirectoryFilePath(string packageName, string subdirectory, string fileName);

        /// <summary>
        /// Gets the base apps directory where all packages are installed.
        /// </summary>
        /// <returns>The full path to the apps directory</returns>
        string GetAppsDirectory();

        /// <summary>
        /// Gets the htdocs directory for web content.
        /// </summary>
        /// <returns>The full path to the htdocs directory</returns>
        string GetHtdocsDirectory();
    }
}