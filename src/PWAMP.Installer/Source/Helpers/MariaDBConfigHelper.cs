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
        private static readonly MariaDBConfigHelperImpl _impl = new MariaDBConfigHelperImpl();

        public static async Task ConfigureMariaDBAsync(string installPath, IProgress<string> logger)
        {
            await _impl.ConfigureAsync(installPath, logger);
        }

        public static async Task ConfigureMariaDBAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            await _impl.ConfigureAsync(pathResolver, logger);
        }

        private class MariaDBConfigHelperImpl : BaseConfigHelper
        {
            protected override string PackageName => PackageNames.MariaDB;
            protected override string DisplayName => "MariaDB Database";
            protected override string BinaryFileName => PackageNames.MariaDBFiles.MysqldExe;
            protected override string TemplateFileName => PackageNames.MariaDBFiles.Templates.MyIni;
            protected override string ConfigFileName => PackageNames.MariaDBFiles.MyIni;

            protected override async Task CreatePackageSpecificDirectoriesAsync(IPathResolver pathResolver, string packageDir, IProgress<string> logger)
            {
                // Create required MariaDB folders
                await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(packageDir, "data"));
                await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(packageDir, "logs"));
                await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(packageDir, "tmp"));
                await FileHelper.CreateDirectoryIfNotExistsAsync(Path.Combine(packageDir, "secure-files"));
            }

            protected override string GetTemplateFilePattern()
            {
                return "*.ini";
            }
        }
    }
}