using System;
using System.IO;
using System.Threading.Tasks;
using Wampoon.Installer.Helpers.Common;
using Wampoon.Installer.Core;
using Wampoon.Installer.Core.Paths;

namespace Wampoon.Installer.Helpers
{
    public static class ApacheConfigHelper
    {
        private static readonly ApacheConfigHelperImpl _impl = new ApacheConfigHelperImpl();

        public static async Task ConfigureApacheAsync(string installPath, IProgress<string> logger)
        {
            await _impl.ConfigureAsync(installPath, logger);
        }

        public static async Task ConfigureApacheAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            await _impl.ConfigureAsync(pathResolver, logger);
        }

        private class ApacheConfigHelperImpl : BaseConfigHelper
        {
            protected override string PackageName => PackageNames.Apache;
            protected override string DisplayName => "Apache HTTP Server";
            protected override string BinaryFileName => PackageNames.ApacheFiles.HttpdExe;
            protected override string TemplateFileName => PackageNames.ApacheFiles.Templates.HttpdConf;
            protected override string ConfigFileName => PackageNames.ApacheFiles.HttpdConf;

            protected override async Task CreatePackageSpecificDirectoriesAsync(IPathResolver pathResolver, string packageDir, IProgress<string> logger)
            {
                var confDir = pathResolver.GetSubdirectoryPath(PackageNames.Apache, "conf");
                await FileHelper.CreateDirectoryIfNotExistsAsync(confDir);
                
                // Create required Apache folders.
                await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(packageDir, "logs"));
                await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(packageDir, "tmp"));
            }

            protected override async Task ConfigureAdditionalTemplatesAsync(IPathResolver pathResolver, IProgress<string> logger)
            {
                // Copy the custom path file.
                var templateCustomPath = TemplateHelper.GetTemplatePath(PackageNames.ApacheFiles.Templates.PwampCustomPathConf);
                var customConfTargetPath = pathResolver.GetConfigPath(PackageNames.Apache, PackageNames.ApacheFiles.PwampCustomPathConf);
                await TemplateHelper.CopyTemplateWithVersionAsync(templateCustomPath, customConfTargetPath);

                // Copy the vhosts file.
                var templateVhostsPath = TemplateHelper.GetTemplatePath(PackageNames.ApacheFiles.Templates.PwampVhostsConf);
                var vHostsConfTargetPath = pathResolver.GetConfigPath(PackageNames.Apache, PackageNames.ApacheFiles.PwampVhostsConf);
                await TemplateHelper.CopyTemplateWithVersionAsync(templateVhostsPath, vHostsConfTargetPath);
            }
        }
    }
}