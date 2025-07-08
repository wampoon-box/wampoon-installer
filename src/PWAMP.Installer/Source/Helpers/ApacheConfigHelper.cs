using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PWAMP.Installer.Neo.Helpers.Common;
using PWAMP.Installer.Neo.Core;
using PWAMP.Installer.Neo.Core.Paths;

namespace PWAMP.Installer.Neo.Helpers
{
    public static class ApacheConfigHelper
    {
        public static async Task ConfigureApacheAsync(string installPath, IProgress<string> logger)
        {
            var pathResolver = PathResolverFactory.CreatePathResolver(installPath);
            await ConfigureApacheAsync(pathResolver, logger);
        }

        public static async Task ConfigureApacheAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            try
            {
                logger?.Report("Configuring Apache HTTP Server...");

                // Validate prerequisites
                if (!await ValidatePrerequisitesAsync(pathResolver, logger))
                {
                    throw new Exception("Apache prerequisites validation failed. Check the detailed error messages above for specific issues with Apache binary or template files.");
                }

                // Create necessary directories
                await CreateApacheDirectoriesAsync(pathResolver, logger);

                // Now we need to copy the Apache's config templates.
                await ConfigureApacheFromTemplateAsync(pathResolver, logger);

                // Validate configuration was created.
                if (!await ValidateConfigurationAsync(pathResolver, logger))
                {
                    throw new Exception("Apache configuration validation failed");
                }

                logger?.Report("✓ Apache HTTP Server configured successfully");
            }
            catch (Exception ex)
            {
                logger?.Report($"✗ Apache configuration failed: {ex.Message}");
                throw;
            }
        }

        private static async Task<bool> ValidatePrerequisitesAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            logger?.Report("Validating Apache prerequisites...");
            var validationErrors = new List<string>();

            // Check if Apache binaries exist using path resolver
            var apachePath = pathResolver.GetBinaryPath(PackageNames.Apache, PackageNames.ApacheFiles.HttpdExe);
            logger?.Report($"Checking Apache binary at: {apachePath}");
            
            var apacheExists = FileHelper.ValidateFileExists(apachePath, "Apache binary");

            if (apacheExists)
            {
                logger?.Report($"✓ Found Apache binary at: {apachePath}");
            }
            else
            {
                var errorMsg = $"Apache binary not found at: {apachePath}";
                logger?.Report($"✗ {errorMsg}");
                validationErrors.Add(errorMsg);
                
                // Check if directory exists
                var apacheDir = System.IO.Path.GetDirectoryName(apachePath);
                if (!System.IO.Directory.Exists(apacheDir))
                {
                    logger?.Report($"✗ Apache directory does not exist: {apacheDir}");
                    validationErrors.Add($"Apache directory missing: {apacheDir}");
                }
                else
                {
                    logger?.Report($"✓ Apache directory exists: {apacheDir}");
                    // List files in the directory to help debug
                    try
                    {
                        var files = System.IO.Directory.GetFiles(apacheDir, "*.exe");
                        if (files.Length > 0)
                        {
                            logger?.Report($"Found {files.Length} .exe files in Apache directory: {string.Join(", ", files.Select(System.IO.Path.GetFileName))}");
                        }
                        else
                        {
                            logger?.Report("No .exe files found in Apache directory");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger?.Report($"Error listing Apache directory contents: {ex.Message}");
                    }
                }
            }

            // Check if required template exists
            var templatePath = TemplateHelper.GetTemplatePath(PackageNames.ApacheFiles.Templates.HttpdConf);
            logger?.Report($"Checking Apache template at: {templatePath}");
            
            var templateExists = FileHelper.ValidateFileExists(templatePath, "Apache template");

            if (templateExists)
            {
                logger?.Report($"✓ Found Apache template at: {templatePath}");
            }
            else
            {
                var errorMsg = $"Apache template not found at: {templatePath}";
                logger?.Report($"✗ {errorMsg}");
                validationErrors.Add(errorMsg);
                
                // Check if template directory exists
                var templateDir = System.IO.Path.GetDirectoryName(templatePath);
                if (!System.IO.Directory.Exists(templateDir))
                {
                    logger?.Report($"✗ Template directory does not exist: {templateDir}");
                    validationErrors.Add($"Template directory missing: {templateDir}");
                }
                else
                {
                    logger?.Report($"✓ Template directory exists: {templateDir}");
                    // List template files to help debug
                    try
                    {
                        var templateFiles = System.IO.Directory.GetFiles(templateDir, "*.conf");
                        if (templateFiles.Length > 0)
                        {
                            logger?.Report($"Found {templateFiles.Length} .conf template files: {string.Join(", ", templateFiles.Select(System.IO.Path.GetFileName))}");
                        }
                        else
                        {
                            logger?.Report("No .conf template files found in template directory");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger?.Report($"Error listing template directory contents: {ex.Message}");
                    }
                }
            }

            var isValid = apacheExists && templateExists;
            
            if (!isValid)
            {
                logger?.Report($"✗ Apache prerequisites validation failed with {validationErrors.Count} error(s):");
                foreach (var error in validationErrors)
                {
                    logger?.Report($"  - {error}");
                }
            }
            else
            {
                logger?.Report("✓ All Apache prerequisites validated successfully");
            }

            return isValid;
        }

        private static async Task CreateApacheDirectoriesAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            logger?.Report("Creating Apache directories...");
            //TODO: Create Apache's log and tmp folders.
            var apacheDir = pathResolver.GetPackageDirectory(PackageNames.Apache);
            var confDir = pathResolver.GetSubdirectoryPath(PackageNames.Apache, "conf");

            await FileHelper.CreateDirectoryIfNotExistsAsync(apacheDir);
            await FileHelper.CreateDirectoryIfNotExistsAsync(confDir);
        }

        private static async Task ConfigureApacheFromTemplateAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            logger?.Report("Copying Apache configuration from template...");

            var templatePath = TemplateHelper.GetTemplatePath(PackageNames.ApacheFiles.Templates.HttpdConf);
            var targetPath = pathResolver.GetConfigPath(PackageNames.Apache, PackageNames.ApacheFiles.HttpdConf);

            await TemplateHelper.CopyTemplateWithVersionAsync(templatePath, targetPath);

            // Copy the custom path file. 
            var templateCustomPath = TemplateHelper.GetTemplatePath(PackageNames.ApacheFiles.Templates.PwampCustomPathConf);
            var customConfTargetPath = pathResolver.GetConfigPath(PackageNames.Apache, PackageNames.ApacheFiles.PwampCustomPathConf);

            await TemplateHelper.CopyTemplateWithVersionAsync(templateCustomPath, customConfTargetPath);

            
            var templateVhostsPath = TemplateHelper.GetTemplatePath(PackageNames.ApacheFiles.Templates.PwampVhostsConf);
            var vHostsConfTargetPath = pathResolver.GetConfigPath(PackageNames.Apache, PackageNames.ApacheFiles.PwampVhostsConf);

            await TemplateHelper.CopyTemplateWithVersionAsync(templateVhostsPath, vHostsConfTargetPath);

            logger?.Report("Apache configuration copied from template");
        }

        private static async Task<bool> ValidateConfigurationAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            logger?.Report("Validating Apache configuration...");

            var httpdConf = pathResolver.GetConfigPath(PackageNames.Apache, PackageNames.ApacheFiles.HttpdConf);
            var isValid = FileHelper.ValidateFileExists(httpdConf, "Apache configuration");

            if (!isValid)
            {
                logger?.Report($"✗ Apache configuration not found at: {httpdConf}");
            }

            return isValid;
        }
    }
}