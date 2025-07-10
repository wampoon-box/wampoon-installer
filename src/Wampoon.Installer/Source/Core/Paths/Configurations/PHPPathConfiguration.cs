using Wampoon.Installer.Core;

namespace Wampoon.Installer.Core.Paths.Configurations
{
    /// <summary>
    /// Path configuration for PHP Scripting Language package.
    /// Defines all PHP-specific paths and file locations.    
    /// </summary>
    public class PHPPathConfiguration : PackagePathConfiguration
    {
        public override string PackageName => AppSettings.PackageNames.PHP;
        public override string BinaryDirectory => "";
        public override string ConfigDirectory => "";

        protected override void InitializeConfiguration()
        {
            // Binary files (PHP binaries are typically in the root directory).
            AddBinaryFile(AppSettings.PHPFiles.PhpExe, AppSettings.PHPFiles.PhpExe);
            AddBinaryFile(AppSettings.PHPFiles.PhpCgiExe, AppSettings.PHPFiles.PhpCgiExe);
            
            // Configuration files.
            AddConfigFile(AppSettings.PHPFiles.PhpIni, AppSettings.PHPFiles.PhpIni);
            AddConfigFile(AppSettings.PHPFiles.PhpIniDevelopment, AppSettings.PHPFiles.PhpIniDevelopment);
            AddConfigFile(AppSettings.PHPFiles.PhpIniProduction, AppSettings.PHPFiles.PhpIniProduction);
            
            // Subdirectories.
            AddSubdirectory("ext", "ext");
            AddSubdirectory("logs", "logs");
            AddSubdirectory("tmp", "tmp");
        }
    }
}