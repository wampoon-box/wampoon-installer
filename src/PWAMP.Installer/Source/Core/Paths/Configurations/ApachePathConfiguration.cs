using Wampoon.Installer.Core;

namespace Wampoon.Installer.Core.Paths.Configurations
{
    /// <summary>
    /// Path configuration for Apache HTTP Server package.
    /// Defines all Apache-specific paths and alternative locations.    
    /// </summary>
    public class ApachePathConfiguration : PackagePathConfiguration
    {
        public override string PackageName => PackageNames.Apache;
        public override string BinaryDirectory => "bin";
        public override string ConfigDirectory => "conf";

        protected override void InitializeConfiguration()
        {
            // Binary files
            AddBinaryFile(PackageNames.ApacheFiles.HttpdExe, $"bin/{PackageNames.ApacheFiles.HttpdExe}");
            
            // Configuration files
            AddConfigFile(PackageNames.ApacheFiles.HttpdConf, $"conf/{PackageNames.ApacheFiles.HttpdConf}");
            AddConfigFile(PackageNames.ApacheFiles.PwampCustomPathConf, $"conf/{PackageNames.ApacheFiles.PwampCustomPathConf}");
            AddConfigFile(PackageNames.ApacheFiles.PwampVhostsConf, $"conf/extra/{PackageNames.ApacheFiles.PwampVhostsConf}");
            
            // Subdirectories
            AddSubdirectory("conf", "conf");
            AddSubdirectory("bin", "bin");
            AddSubdirectory("logs", "logs");
            AddSubdirectory("htdocs", "htdocs");
            AddSubdirectory("conf-extra", "conf/extra");
            
            // Alternative binary paths for Apache24 nested structure
            AddAlternativeBinaryPaths(PackageNames.ApacheFiles.HttpdExe, $"Apache24/bin/{PackageNames.ApacheFiles.HttpdExe}");
        }
    }
}