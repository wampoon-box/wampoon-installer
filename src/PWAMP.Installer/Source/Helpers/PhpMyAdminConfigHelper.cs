using System;
using System.IO;
using System.Threading.Tasks;
using PWAMP.Installer.Neo.Helpers.Common;
using PWAMP.Installer.Neo.Core;
using PWAMP.Installer.Neo.Core.Paths;

namespace PWAMP.Installer.Neo.Helpers
{
    public static class PhpMyAdminConfigHelper
    {
        public static async Task ConfigurePhpMyAdminAsync(string installPath, IProgress<string> logger)
        {
            var pathResolver = PathResolverFactory.CreatePathResolver(installPath);
            await ConfigurePhpMyAdminAsync(pathResolver, logger);
        }

        public static async Task ConfigurePhpMyAdminAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            try
            {
                logger?.Report("Configuring phpMyAdmin Database Manager...");
                
                // Validate prerequisites
                if (!await ValidatePrerequisitesAsync(pathResolver, logger))
                {
                    throw new Exception("phpMyAdmin prerequisites validation failed");
                }
                
                // Configure phpMyAdmin using template
                await ConfigurePhpMyAdminFromTemplateAsync(pathResolver, logger);
                
                // Validate configuration was created
                if (!await ValidateConfigurationAsync(pathResolver, logger))
                {
                    throw new Exception("phpMyAdmin configuration validation failed");
                }
                
                logger?.Report("✓ phpMyAdmin Database Manager configured successfully");
            }
            catch (Exception ex)
            {
                logger?.Report($"✗ phpMyAdmin configuration failed: {ex.Message}");
                throw;
            }
        }

        private static async Task<bool> ValidatePrerequisitesAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            logger?.Report("Validating phpMyAdmin prerequisites...");
            
            // Check if phpMyAdmin files exist
            var phpMyAdminPath = pathResolver.GetBinaryPath(PackageNames.PhpMyAdmin, PackageNames.PhpMyAdminFiles.IndexPhp);
            var phpMyAdminExists = FileHelper.ValidateFileExists(phpMyAdminPath, "phpMyAdmin");
            
            // Check if required template exists
            var templatePath = TemplateHelper.GetTemplatePath(PackageNames.PhpMyAdminFiles.Templates.ConfigIncPhp);
            var templateExists = FileHelper.ValidateFileExists(templatePath, "phpMyAdmin template");
            
            if (!phpMyAdminExists)
            {
                logger?.Report($"✗ phpMyAdmin not found at: {phpMyAdminPath}");
            }
            
            if (!templateExists)
            {
                logger?.Report($"✗ phpMyAdmin template not found at: {templatePath}");
            }
            
            return phpMyAdminExists && templateExists;
        }

        private static async Task ConfigurePhpMyAdminFromTemplateAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            logger?.Report("Copying phpMyAdmin configuration from template...");
            
            var templatePath = TemplateHelper.GetTemplatePath(PackageNames.PhpMyAdminFiles.Templates.ConfigIncPhp);
            var targetPath = pathResolver.GetConfigPath(PackageNames.PhpMyAdmin, PackageNames.PhpMyAdminFiles.ConfigIncPhp);
            
            await TemplateHelper.CopyTemplateWithVersionAsync(templatePath, targetPath);
            
            logger?.Report("phpMyAdmin configuration copied from template");
        }

        private static async Task<bool> ValidateConfigurationAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            logger?.Report("Validating phpMyAdmin configuration...");
            
            var configFile = pathResolver.GetConfigPath(PackageNames.PhpMyAdmin, PackageNames.PhpMyAdminFiles.ConfigIncPhp);
            var isValid = FileHelper.ValidateFileExists(configFile, "phpMyAdmin configuration");
            
            if (!isValid)
            {
                logger?.Report($"✗ phpMyAdmin configuration not found at: {configFile}");
            }
            
            return isValid;
        }
    }
}