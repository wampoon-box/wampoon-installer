using System;
using System.IO;
using System.Threading.Tasks;
using Wampoon.Installer.Helpers.Common;
using Wampoon.Installer.Core;
using Wampoon.Installer.Core.Paths;

namespace Wampoon.Installer.Helpers
{
    public static class PHPConfigHelper
    {
        private static readonly PHPConfigHelperImpl _impl = new PHPConfigHelperImpl();

        public static async Task ConfigurePHPAsync(string installPath, IProgress<string> logger)
        {
            await _impl.ConfigureAsync(installPath, logger);
        }

        public static async Task ConfigurePHPAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            await _impl.ConfigureAsync(pathResolver, logger);
        }

        private class PHPConfigHelperImpl : BaseConfigHelper
        {
            protected override string PackageName => AppSettings.PackageNames.PHP;
            protected override string DisplayName => "PHP Scripting Language";
            protected override string BinaryFileName => AppSettings.PHPFiles.PhpExe;
            protected override string TemplateFileName => AppSettings.PHPFiles.Templates.PhpIni;
            protected override string ConfigFileName => AppSettings.PHPFiles.PhpIni;

            protected override async Task CreatePackageSpecificDirectoriesAsync(IPathResolver pathResolver, string packageDir, IProgress<string> logger)
            {
                // Create required PHP folders.
                await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(packageDir, "logs"));
                await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(packageDir, "sessions"));
                await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(packageDir, "temp"));
            }

            protected override string GetTemplateFilePattern()
            {
                return "*.ini";
            }
        }
    }
}