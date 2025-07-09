using Wampoon.Installer.Core;

namespace Wampoon.Installer.Core.Paths.Configurations
{
    /// <summary>
    /// Path configuration for PHP Scripting Language package.
    /// Defines all PHP-specific paths and file locations.    
    /// </summary>
    public class PHPPathConfiguration : PackagePathConfiguration
    {
        public override string PackageName => PackageNames.PHP;
        public override string BinaryDirectory => "";
        public override string ConfigDirectory => "";

        protected override void InitializeConfiguration()
        {
            // Binary files (PHP binaries are typically in the root directory).
            AddBinaryFile(PackageNames.PHPFiles.PhpExe, PackageNames.PHPFiles.PhpExe);
            AddBinaryFile(PackageNames.PHPFiles.PhpCgiExe, PackageNames.PHPFiles.PhpCgiExe);
            
            // Configuration files.
            AddConfigFile(PackageNames.PHPFiles.PhpIni, PackageNames.PHPFiles.PhpIni);
            AddConfigFile(PackageNames.PHPFiles.PhpIniDevelopment, PackageNames.PHPFiles.PhpIniDevelopment);
            AddConfigFile(PackageNames.PHPFiles.PhpIniProduction, PackageNames.PHPFiles.PhpIniProduction);
            
            // Subdirectories.
            AddSubdirectory("ext", "ext");
            AddSubdirectory("logs", "logs");
            AddSubdirectory("tmp", "tmp");
        }
    }
}