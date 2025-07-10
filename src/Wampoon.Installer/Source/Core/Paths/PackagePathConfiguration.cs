using System.Collections.Generic;

namespace Wampoon.Installer.Core.Paths
{
    /// <summary>
    /// Base implementation of package path configuration.
    /// Provides common functionality while following the Template Method pattern.    
    /// </summary>
    public abstract class PackagePathConfiguration : IPackagePathConfiguration
    {
        protected PackagePathConfiguration()
        {
            BinaryFiles = new Dictionary<string, string>();
            ConfigFiles = new Dictionary<string, string>();
            Subdirectories = new Dictionary<string, string>();
            AlternativeBinaryPaths = new Dictionary<string, List<string>>();
            
            InitializeConfiguration();
        }

        public abstract string PackageName { get; }
        public virtual string BinaryDirectory => "bin";
        public virtual string ConfigDirectory => "";

        public Dictionary<string, string> BinaryFiles { get; protected set; }
        public Dictionary<string, string> ConfigFiles { get; protected set; }
        public Dictionary<string, string> Subdirectories { get; protected set; }
        public Dictionary<string, List<string>> AlternativeBinaryPaths { get; protected set; }

        /// <summary>
        /// Template method for initializing package-specific configuration.
        /// Derived classes override this to define their specific paths.
        /// </summary>
        protected abstract void InitializeConfiguration();

        /// <summary>
        /// Helper method to add a binary file configuration.
        /// </summary>
        /// <param name="binaryName">The name of the binary file</param>
        /// <param name="relativePath">The relative path from package root</param>
        protected void AddBinaryFile(string binaryName, string relativePath)
        {
            BinaryFiles[binaryName] = relativePath;
        }

        /// <summary>
        /// Helper method to add a configuration file.
        /// </summary>
        /// <param name="configName">The name of the configuration file</param>
        /// <param name="relativePath">The relative path from package root</param>
        protected void AddConfigFile(string configName, string relativePath)
        {
            ConfigFiles[configName] = relativePath;
        }

        /// <summary>
        /// Helper method to add a subdirectory.
        /// </summary>
        /// <param name="subdirectoryName">The name of the subdirectory</param>
        /// <param name="relativePath">The relative path from package root</param>
        protected void AddSubdirectory(string subdirectoryName, string relativePath)
        {
            Subdirectories[subdirectoryName] = relativePath;
        }

        /// <summary>
        /// Helper method to add alternative binary paths.
        /// </summary>
        /// <param name="binaryName">The name of the binary file</param>
        /// <param name="alternativePaths">List of alternative relative paths</param>
        protected void AddAlternativeBinaryPaths(string binaryName, params string[] alternativePaths)
        {
            AlternativeBinaryPaths[binaryName] = new List<string>(alternativePaths);
        }
    }
}