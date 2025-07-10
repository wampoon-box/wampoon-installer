using Wampoon.Installer.Core;

namespace Wampoon.Installer.Core.Paths.Configurations
{
    /// <summary>
    /// Path configuration for Apache HTTP Server package.
    /// Defines all Apache-specific paths and alternative locations.    
    /// </summary>
    public class ApachePathConfiguration : PackagePathConfiguration
    {
        public override string PackageName => AppSettings.PackageNames.Apache;
        public override string BinaryDirectory => "bin";
        public override string ConfigDirectory => "conf";

        protected override void InitializeConfiguration()
        {
            // Binary files.
            AddBinaryFile(AppSettings.ApacheFiles.HttpdExe, $"bin/{AppSettings.ApacheFiles.HttpdExe}");
            
            // Configuration files.
            AddConfigFile(AppSettings.ApacheFiles.HttpdConf, $"conf/{AppSettings.ApacheFiles.HttpdConf}");
            AddConfigFile(AppSettings.ApacheFiles.WampoonCustomPathConf, $"conf/{AppSettings.ApacheFiles.WampoonCustomPathConf}");
            AddConfigFile(AppSettings.ApacheFiles.WampoonVhostsConf, $"conf/extra/{AppSettings.ApacheFiles.WampoonVhostsConf}");
            
            // Subdirectories.
            AddSubdirectory("conf", "conf");
            AddSubdirectory("bin", "bin");
            AddSubdirectory("logs", "logs");
            AddSubdirectory("htdocs", "htdocs");
            AddSubdirectory("conf-extra", "conf/extra");
            
            // Alternative binary paths for Apache24 nested structure.
            AddAlternativeBinaryPaths(AppSettings.ApacheFiles.HttpdExe, $"Apache24/bin/{AppSettings.ApacheFiles.HttpdExe}");
        }
    }
}