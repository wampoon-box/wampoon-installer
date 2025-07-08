using System;
using System.IO;
using System.Threading.Tasks;
using PWAMP.Installer.Neo.Helpers.Common;
using PWAMP.Installer.Neo.Core;
using PWAMP.Installer.Neo.Core.Paths;

namespace PWAMP.Installer.Neo.Helpers
{
    public static class PHPConfigHelper
    {
        public static async Task ConfigurePHPAsync(string installPath, IProgress<string> logger)
        {
            var pathResolver = PathResolverFactory.CreatePathResolver(installPath);
            await ConfigurePHPAsync(pathResolver, logger);
        }

        public static async Task ConfigurePHPAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            try
            {
                logger?.Report("Configuring PHP Scripting Language...");
                
                // Validate prerequisites
                if (!await ValidatePrerequisitesAsync(pathResolver, logger))
                {
                    throw new Exception("PHP prerequisites validation failed");
                }
                
                // Create necessary directories
                await CreatePHPDirectoriesAsync(pathResolver, logger);
                
                // Configure PHP using template
                await ConfigurePHPFromTemplateAsync(pathResolver, logger);
                
                // Validate configuration was created
                if (!await ValidateConfigurationAsync(pathResolver, logger))
                {
                    throw new Exception("PHP configuration validation failed");
                }
                
                logger?.Report("✓ PHP Scripting Language configured successfully");
            }
            catch (Exception ex)
            {
                logger?.Report($"✗ PHP configuration failed: {ex.Message}");
                throw;
            }
        }

        private static async Task<bool> ValidatePrerequisitesAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            logger?.Report("Validating PHP prerequisites...");
            
            // Check if PHP binaries exist
            var phpPath = pathResolver.GetBinaryPath(PackageNames.PHP, PackageNames.PHPFiles.PhpExe);
            var phpExists = FileHelper.ValidateFileExists(phpPath, "PHP binary");
            
            // Check if required template exists
            var templatePath = TemplateHelper.GetTemplatePath(PackageNames.PHPFiles.Templates.PhpIni);
            var templateExists = FileHelper.ValidateFileExists(templatePath, "PHP template");
            
            if (!phpExists)
            {
                logger?.Report($"✗ PHP binary not found at: {phpPath}");
            }
            
            if (!templateExists)
            {
                logger?.Report($"✗ PHP template not found at: {templatePath}");
            }
            
            return phpExists && templateExists;
        }

        private static async Task CreatePHPDirectoriesAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            logger?.Report("Creating PHP directories...");
            
            var phpDir = pathResolver.GetPackageDirectory(PackageNames.PHP);
            
            await FileHelper.CreateDirectoryIfNotExistsAsync(phpDir);
            
            // Create required PHP folders
            await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(phpDir, "logs"));
            await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(phpDir, "sessions"));
            await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(phpDir, "temp"));
        }

        private static async Task ConfigurePHPFromTemplateAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            logger?.Report("Copying PHP configuration from template...");
            
            var templatePath = TemplateHelper.GetTemplatePath(PackageNames.PHPFiles.Templates.PhpIni);
            var targetPath = pathResolver.GetConfigPath(PackageNames.PHP, PackageNames.PHPFiles.PhpIni);
            
            await TemplateHelper.CopyTemplateWithVersionAsync(templatePath, targetPath);
            
            logger?.Report("PHP configuration copied from template");
        }

        private static async Task<bool> ValidateConfigurationAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            logger?.Report("Validating PHP configuration...");
            
            var phpIni = pathResolver.GetConfigPath(PackageNames.PHP, PackageNames.PHPFiles.PhpIni);
            var isValid = FileHelper.ValidateFileExists(phpIni, "PHP configuration");
            
            if (!isValid)
            {
                logger?.Report($"✗ PHP configuration not found at: {phpIni}");
            }
            
            return isValid;
        }
    }
}