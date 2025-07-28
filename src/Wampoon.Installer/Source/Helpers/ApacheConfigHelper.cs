using System;
using System.IO;
using System.Net.Http;
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
            protected override string PackageName => AppSettings.PackageNames.Apache;
            protected override string DisplayName => "Apache HTTP Server";
            protected override string BinaryFileName => AppSettings.ApacheFiles.HttpdExe;
            protected override string TemplateFileName => AppSettings.ApacheFiles.Templates.HttpdConf;
            protected override string ConfigFileName => AppSettings.ApacheFiles.HttpdConf;

            protected override async Task CreatePackageSpecificDirectoriesAsync(IPathResolver pathResolver, string packageDir, IProgress<string> logger)
            {
                var confDir = pathResolver.GetSubdirectoryPath(AppSettings.PackageNames.Apache, "conf");
                await FileHelper.CreateDirectoryIfNotExistsAsync(confDir);
                
                // Create required Apache folders.
                await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(packageDir, "logs"));
                await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(packageDir, "tmp"));
            }

            protected override async Task ConfigureAdditionalTemplatesAsync(IPathResolver pathResolver, IProgress<string> logger)
            {
                // Copy the custom path file.
                var templateCustomPath = TemplateHelper.GetTemplatePath(AppSettings.ApacheFiles.Templates.WampoonCustomPathConf);
                var customConfTargetPath = pathResolver.GetConfigPath(AppSettings.PackageNames.Apache, AppSettings.ApacheFiles.WampoonCustomPathConf);
                TemplateHelper.CopyTemplateWithVersion(templateCustomPath, customConfTargetPath);

                // Copy the vhosts file.
                var templateVhostsPath = TemplateHelper.GetTemplatePath(AppSettings.ApacheFiles.Templates.WampoonVhostsConf);
                var vHostsConfTargetPath = pathResolver.GetConfigPath(AppSettings.PackageNames.Apache, AppSettings.ApacheFiles.WampoonVhostsConf);
                TemplateHelper.CopyTemplateWithVersion(templateVhostsPath, vHostsConfTargetPath);
            }

        }
    }
}