using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wampoon.Installer.Core;
using Wampoon.Installer.Core.Paths;
using Wampoon.Installer.Helpers.Logging;
using Wampoon.Installer.Helpers;

namespace Wampoon.Installer.Helpers.Common
{
    public abstract class BaseConfigHelper
    {
        protected abstract string PackageName { get; }
        protected abstract string DisplayName { get; }
        protected abstract string BinaryFileName { get; }
        protected abstract string TemplateFileName { get; }
        protected abstract string ConfigFileName { get; }

        public async Task ConfigureAsync(string installPath, IProgress<string> logger)
        {
            var pathResolver = PathResolverFactory.CreatePathResolver(installPath);
            await ConfigureAsync(pathResolver, logger);
        }

        public async Task ConfigureAsync(IPathResolver pathResolver, IProgress<string> progressReporter)
        {
            try
            {
                progressReporter?.Report($"Configuring {DisplayName}...");

                // Validate prerequisites.
                if (!await ValidatePrerequisitesAsync(pathResolver, progressReporter))
                {
                    throw new Exception($"{DisplayName} prerequisites validation failed. Check the detailed error messages above for specific issues with {DisplayName} binary or template files.");
                }

                // Create necessary directories.
                await CreateDirectoriesAsync(pathResolver, progressReporter);

                // Configure using template.
                await ConfigureFromTemplateAsync(pathResolver, progressReporter);

                // Validate configuration was created.
                if (!await ValidateConfigurationAsync(pathResolver, progressReporter))
                {
                    throw new Exception($"{DisplayName} configuration validation failed");
                }

                progressReporter?.Report($"✓ {DisplayName} configured successfully");
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogExceptionInfo(ex);
                progressReporter?.Report($"✗ {DisplayName} configuration failed: {ex.Message}");
                throw;
            }
        }

        protected virtual async Task<bool> ValidatePrerequisitesAsync(IPathResolver pathResolver, IProgress<string> progressReporter)
        {
            progressReporter?.Report($"Validating {DisplayName} prerequisites...");
            var validationErrors = new List<string>();

            // Check if binary exists.
            var binaryPath = pathResolver.GetBinaryPath(PackageName, BinaryFileName);
            progressReporter?.Report($"Checking {DisplayName} binary at: {binaryPath}");
            
            var binaryExists = FileHelper.ValidateFileExists(binaryPath, $"{DisplayName} binary");

            if (binaryExists)
            {
                progressReporter?.Report($"✓ Found {DisplayName} binary at: {binaryPath}");
            }
            else
            {
                var errorMsg = $"{DisplayName} binary not found at: {binaryPath}";
                progressReporter?.Report($"✗ {errorMsg}");
                validationErrors.Add(errorMsg);
                
                // Check if directory exists.
                var binaryDir = Path.GetDirectoryName(binaryPath);
                if (!Directory.Exists(binaryDir))
                {
                    progressReporter?.Report($"✗ {DisplayName} directory does not exist: {binaryDir}");
                    validationErrors.Add($"{DisplayName} directory missing: {binaryDir}");
                }
                else
                {
                    progressReporter?.Report($"✓ {DisplayName} directory exists: {binaryDir}");
                    await LogDirectoryContentsAsync(binaryDir, GetBinaryFilePattern(), progressReporter);
                }
            }

            // Check if required template exists.
            var templatePath = TemplateHelper.GetTemplatePath(TemplateFileName);
            progressReporter?.Report($"Checking {DisplayName} template at: {templatePath}");
            
            var templateExists = FileHelper.ValidateFileExists(templatePath, $"{DisplayName} template");

            if (templateExists)
            {
                progressReporter?.Report($"✓ Found {DisplayName} template at: {templatePath}");
            }
            else
            {
                var errorMsg = $"{DisplayName} template not found at: {templatePath}";
                progressReporter?.Report($"✗ {errorMsg}");
                validationErrors.Add(errorMsg);
                
                // Check if template directory exists.
                var templateDir = Path.GetDirectoryName(templatePath);
                if (!Directory.Exists(templateDir))
                {
                    progressReporter?.Report($"✗ Template directory does not exist: {templateDir}");
                    validationErrors.Add($"Template directory missing: {templateDir}");
                }
                else
                {
                    progressReporter?.Report($"✓ Template directory exists: {templateDir}");
                    await LogDirectoryContentsAsync(templateDir, GetTemplateFilePattern(), progressReporter);
                }
            }

            var isValid = binaryExists && templateExists;
            
            if (!isValid)
            {
                progressReporter?.Report($"✗ {DisplayName} prerequisites validation failed with {validationErrors.Count} error(s):");
                foreach (var error in validationErrors)
                {
                    progressReporter?.Report($"  - {error}");
                }
            }
            else
            {
                progressReporter?.Report($"✓ All {DisplayName} prerequisites validated successfully");
            }

            return isValid;
        }

        protected virtual async Task CreateDirectoriesAsync(IPathResolver pathResolver, IProgress<string> progressReporter)
        {
            progressReporter?.Report($"Creating {DisplayName} directories...");
            
            var packageDir = pathResolver.GetPackageDirectory(PackageName);
            await FileHelper.CreateDirectoryIfNotExistsAsync(packageDir);
            
            await CreatePackageSpecificDirectoriesAsync(pathResolver, packageDir, progressReporter);
        }

        protected virtual async Task CreatePackageSpecificDirectoriesAsync(IPathResolver pathResolver, string packageDir, IProgress<string> progressReporter)
        {
            // Default implementation - can be overridden by derived classes.
            await Task.CompletedTask;
        }

        protected virtual async Task ConfigureFromTemplateAsync(IPathResolver pathResolver, IProgress<string> progressReporter)
        {
            progressReporter?.Report($"Copying {DisplayName} configuration from template...");

            var templatePath = TemplateHelper.GetTemplatePath(TemplateFileName);
            var targetPath = pathResolver.GetConfigPath(PackageName, ConfigFileName);

            TemplateHelper.CopyTemplateWithVersion(templatePath, targetPath);

            await ConfigureAdditionalTemplatesAsync(pathResolver, progressReporter);

            progressReporter?.Report($"{DisplayName} configuration copied from template");
        }

        protected virtual async Task ConfigureAdditionalTemplatesAsync(IPathResolver pathResolver, IProgress<string> progressReporter)
        {
            // Default implementation - can be overridden by derived classes for additional templates.
            await Task.CompletedTask;
        }

        protected virtual Task<bool> ValidateConfigurationAsync(IPathResolver pathResolver, IProgress<string> progressReporter)
        {
            progressReporter?.Report($"Validating {DisplayName} configuration...");

            var configPath = pathResolver.GetConfigPath(PackageName, ConfigFileName);
            var isValid = FileHelper.ValidateFileExists(configPath, $"{DisplayName} configuration");

            if (!isValid)
            {
                progressReporter?.Report($"✗ {DisplayName} configuration not found at: {configPath}");
            }

            return Task.FromResult(isValid);
        }

        protected virtual string GetBinaryFilePattern()
        {
            return "*.exe";
        }

        protected virtual string GetTemplateFilePattern()
        {
            return "*.conf";
        }

        private async Task LogDirectoryContentsAsync(string directory, string pattern, IProgress<string> progressReporter)
        {
            try
            {
                var files = Directory.GetFiles(directory, pattern);
                if (files.Length > 0)
                {
                    progressReporter?.Report($"Found {files.Length} {pattern} files: {string.Join(", ", files.Select(Path.GetFileName))}");
                }
                else
                {
                    progressReporter?.Report($"No {pattern} files found in directory");
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogExceptionInfo(ex);
                progressReporter?.Report($"Error listing directory contents: {ex.Message}");
            }
            await Task.CompletedTask;
        }
    }
}