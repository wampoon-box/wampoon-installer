using Wampoon.Installer.Core;

namespace Wampoon.Installer.Core.Paths.Configurations
{
    /// <summary>
    /// Path configuration for MariaDB Database package.
    /// Defines all MariaDB-specific paths and file locations.    
    /// </summary>
    public class MariaDBPathConfiguration : PackagePathConfiguration
    {
        public override string PackageName => PackageNames.MariaDB;
        public override string BinaryDirectory => "bin";
        public override string ConfigDirectory => "";

        protected override void InitializeConfiguration()
        {
            // Binary files.
            AddBinaryFile(PackageNames.MariaDBFiles.MysqldExe, $"bin/{PackageNames.MariaDBFiles.MysqldExe}");
            AddBinaryFile(PackageNames.MariaDBFiles.MysqlExe, $"bin/{PackageNames.MariaDBFiles.MysqlExe}");
            
            // Configuration files.
            AddConfigFile(PackageNames.MariaDBFiles.MyIni, PackageNames.MariaDBFiles.MyIni);
            AddConfigFile(PackageNames.MariaDBFiles.MyCnf, PackageNames.MariaDBFiles.MyCnf);
            
            // Subdirectories.  
            AddSubdirectory("bin", "bin");
            AddSubdirectory("data", "data");
            AddSubdirectory("logs", "logs");
        }
    }
}