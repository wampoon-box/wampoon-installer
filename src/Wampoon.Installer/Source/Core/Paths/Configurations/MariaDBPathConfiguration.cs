using Wampoon.Installer.Core;

namespace Wampoon.Installer.Core.Paths.Configurations
{
    /// <summary>
    /// Path configuration for MariaDB Database package.
    /// Defines all MariaDB-specific paths and file locations.    
    /// </summary>
    public class MariaDBPathConfiguration : PackagePathConfiguration
    {
        public override string PackageName => AppSettings.PackageNames.MariaDB;
        public override string BinaryDirectory => "bin";
        public override string ConfigDirectory => "";

        protected override void InitializeConfiguration()
        {
            // Binary files.
            AddBinaryFile(AppSettings.MariaDBFiles.MysqldExe, $"bin/{AppSettings.MariaDBFiles.MysqldExe}");
            AddBinaryFile(AppSettings.MariaDBFiles.MysqlExe, $"bin/{AppSettings.MariaDBFiles.MysqlExe}");
            
            // Configuration files.
            AddConfigFile(AppSettings.MariaDBFiles.MyIni, AppSettings.MariaDBFiles.MyIni);
            AddConfigFile(AppSettings.MariaDBFiles.MyCnf, AppSettings.MariaDBFiles.MyCnf);
            
            // Subdirectories.  
            AddSubdirectory("bin", "bin");
            AddSubdirectory("data", "data");
            AddSubdirectory("logs", "logs");
        }
    }
}