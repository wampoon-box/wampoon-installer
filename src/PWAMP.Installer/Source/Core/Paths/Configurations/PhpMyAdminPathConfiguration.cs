using Wampoon.Installer.Core;

namespace Wampoon.Installer.Core.Paths.Configurations
{
    /// <summary>
    /// Path configuration for phpMyAdmin Database Manager package.
    /// Defines all phpMyAdmin-specific paths and file locations.    
    /// </summary>
    public class PhpMyAdminPathConfiguration : PackagePathConfiguration
    {
        public override string PackageName => PackageNames.PhpMyAdmin;
        public override string BinaryDirectory => "";
        public override string ConfigDirectory => "";

        protected override void InitializeConfiguration()
        {
            // Application files (phpMyAdmin doesn't have traditional binaries)
            AddBinaryFile(PackageNames.PhpMyAdminFiles.IndexPhp, PackageNames.PhpMyAdminFiles.IndexPhp);
            
            // Configuration files
            AddConfigFile(PackageNames.PhpMyAdminFiles.ConfigIncPhp, PackageNames.PhpMyAdminFiles.ConfigIncPhp);
            AddConfigFile(PackageNames.PhpMyAdminFiles.ConfigSampleIncPhp, PackageNames.PhpMyAdminFiles.ConfigSampleIncPhp);
            
            // Subdirectories
            AddSubdirectory("tmp", "tmp");
            AddSubdirectory("libraries", "libraries");
            AddSubdirectory("themes", "themes");
            AddSubdirectory("templates", "templates");
        }
    }
}