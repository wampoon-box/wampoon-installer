using System;
using System.IO;
using System.Threading.Tasks;
using PWAMP.Installer.Neo.Helpers.Common;
using PWAMP.Installer.Neo.Core;
using PWAMP.Installer.Neo.Core.Paths;

namespace PWAMP.Installer.Neo.Helpers
{
    public static class MariaDBConfigHelper
    {
        public static async Task ConfigureMariaDBAsync(string installPath, IProgress<string> logger)
        {
            var pathResolver = PathResolverFactory.CreatePathResolver(installPath);
            await ConfigureMariaDBAsync(pathResolver, logger);
        }

        public static async Task ConfigureMariaDBAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            try
            {
                logger?.Report("Configuring MariaDB Database...");
                
                // Validate prerequisites
                if (!await ValidatePrerequisitesAsync(pathResolver, logger))
                {
                    throw new Exception("MariaDB prerequisites validation failed");
                }
                
                // Create necessary directories
                await CreateMariaDBDirectoriesAsync(pathResolver, logger);
                
                // Configure MariaDB using template
                await ConfigureMariaDBFromTemplateAsync(pathResolver, logger);
                
                // Validate configuration was created
                if (!await ValidateConfigurationAsync(pathResolver, logger))
                {
                    throw new Exception("MariaDB configuration validation failed");
                }
                
                logger?.Report("✓ MariaDB Database configured successfully");
            }
            catch (Exception ex)
            {
                logger?.Report($"✗ MariaDB configuration failed: {ex.Message}");
                throw;
            }
        }

        private static async Task<bool> ValidatePrerequisitesAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            logger?.Report("Validating MariaDB prerequisites...");
            
            // Check if MariaDB binaries exist
            var mariadbPath = pathResolver.GetBinaryPath(PackageNames.MariaDB, PackageNames.MariaDBFiles.MysqldExe);
            var mariadbExists = FileHelper.ValidateFileExists(mariadbPath, "MariaDB binary");
            
            // Check if required template exists
            var templatePath = TemplateHelper.GetTemplatePath(PackageNames.MariaDBFiles.Templates.MyIni);
            var templateExists = FileHelper.ValidateFileExists(templatePath, "MariaDB template");
            
            if (!mariadbExists)
            {
                logger?.Report($"✗ MariaDB binary not found at: {mariadbPath}");
            }
            
            if (!templateExists)
            {
                logger?.Report($"✗ MariaDB template not found at: {templatePath}");
            }
            
            return mariadbExists && templateExists;
        }

        private static async Task CreateMariaDBDirectoriesAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            logger?.Report("Creating MariaDB directories...");
            
            var mariadbDir = pathResolver.GetPackageDirectory(PackageNames.MariaDB);
            
            await FileHelper.CreateDirectoryIfNotExistsAsync(mariadbDir);
            
            // Create required MariaDB folders
            await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(mariadbDir, "data"));
            await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(mariadbDir, "logs"));
            await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(mariadbDir, "tmp"));
            await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(mariadbDir, "secure-files"));
        }

        private static async Task ConfigureMariaDBFromTemplateAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            logger?.Report("Copying MariaDB configuration from template...");
            
            var templatePath = TemplateHelper.GetTemplatePath(PackageNames.MariaDBFiles.Templates.MyIni);
            var targetPath = pathResolver.GetConfigPath(PackageNames.MariaDB, PackageNames.MariaDBFiles.MyIni);
            
            await TemplateHelper.CopyTemplateWithVersionAsync(templatePath, targetPath);
            
            logger?.Report("MariaDB configuration copied from template");
        }

        private static async Task<bool> ValidateConfigurationAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            logger?.Report("Validating MariaDB configuration...");
            
            var myCnf = pathResolver.GetConfigPath(PackageNames.MariaDB, PackageNames.MariaDBFiles.MyIni);
            var isValid = FileHelper.ValidateFileExists(myCnf, "MariaDB configuration");
            
            if (!isValid)
            {
                logger?.Report($"✗ MariaDB configuration not found at: {myCnf}");
            }
            
            return isValid;
        }
    }
}