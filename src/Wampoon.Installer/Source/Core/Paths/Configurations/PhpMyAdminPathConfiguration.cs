using Wampoon.Installer.Core;

namespace Wampoon.Installer.Core.Paths.Configurations
{
    /// <summary>
    /// Path configuration for phpMyAdmin Database Manager package.
    /// Defines all phpMyAdmin-specific paths and file locations.    
    /// </summary>
    public class PhpMyAdminPathConfiguration : PackagePathConfiguration
    {
        public override string PackageName => AppSettings.PackageNames.PhpMyAdmin;
        public override string BinaryDirectory => "";
        public override string ConfigDirectory => "";

        protected override void InitializeConfiguration()
        {
            // Application files (phpMyAdmin doesn't have traditional binaries).
            AddBinaryFile(AppSettings.PhpMyAdminFiles.IndexPhp, AppSettings.PhpMyAdminFiles.IndexPhp);
            
            // Configuration files.
            AddConfigFile(AppSettings.PhpMyAdminFiles.ConfigIncPhp, AppSettings.PhpMyAdminFiles.ConfigIncPhp);
            AddConfigFile(AppSettings.PhpMyAdminFiles.ConfigSampleIncPhp, AppSettings.PhpMyAdminFiles.ConfigSampleIncPhp);
            
            // Subdirectories.  
            AddSubdirectory("tmp", "tmp");
            AddSubdirectory("libraries", "libraries");
            AddSubdirectory("themes", "themes");
            AddSubdirectory("templates", "templates");
        }
    }
}