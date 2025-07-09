using System;
using System.Threading.Tasks;
using Wampoon.Installer.Helpers.Common;
using Wampoon.Installer.Core;
using Wampoon.Installer.Core.Paths;

namespace Wampoon.Installer.Helpers
{
    public static class PhpMyAdminConfigHelper
    {
        private static readonly PhpMyAdminConfigHelperImpl _impl = new PhpMyAdminConfigHelperImpl();

        public static async Task ConfigurePhpMyAdminAsync(string installPath, IProgress<string> logger)
        {
            await _impl.ConfigureAsync(installPath, logger);
        }

        public static async Task ConfigurePhpMyAdminAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            await _impl.ConfigureAsync(pathResolver, logger);
        }

        private class PhpMyAdminConfigHelperImpl : BaseConfigHelper
        {
            protected override string PackageName => PackageNames.PhpMyAdmin;
            protected override string DisplayName => "phpMyAdmin Database Manager";
            protected override string BinaryFileName => PackageNames.PhpMyAdminFiles.IndexPhp;
            protected override string TemplateFileName => PackageNames.PhpMyAdminFiles.Templates.ConfigIncPhp;
            protected override string ConfigFileName => PackageNames.PhpMyAdminFiles.ConfigIncPhp;

            protected override async Task CreatePackageSpecificDirectoriesAsync(IPathResolver pathResolver, string packageDir, IProgress<string> logger)
            {
                // phpMyAdmin doesn't need additional directories
                await Task.CompletedTask;
            }

            protected override string GetBinaryFilePattern()
            {
                return "*.php";
            }

            protected override string GetTemplateFilePattern()
            {
                return "*.php";
            }
        }
    }
}