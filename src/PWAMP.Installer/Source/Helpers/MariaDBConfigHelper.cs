using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Wampoon.Installer.Helpers.Common;
using Wampoon.Installer.Core;
using Wampoon.Installer.Core.Paths;

namespace Wampoon.Installer.Helpers
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

        public static async Task InitializeMariaDBDataDirectoryAsync(IPathResolver pathResolver, IProgress<string> logger)
        {
            await _impl.InitializeDataDirectoryAsync(pathResolver, logger);
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

            public async Task InitializeDataDirectoryAsync(IPathResolver pathResolver, IProgress<string> logger)
            {
                logger?.Report("Initializing MariaDB data directory...");
                
                var packageDir = pathResolver.GetPackageDirectory(PackageNames.MariaDB);
                var binDir = Path.Combine(packageDir, "bin");
                var dataDir = Path.Combine(packageDir, "data");
                var installDbPath = Path.Combine(binDir, PackageNames.MariaDBFiles.MariaDbInstallDbExe);

                if (!File.Exists(installDbPath))
                {
                    throw new FileNotFoundException($"MariaDB installation database executable not found at: {installDbPath}");
                }

                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                }

                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = installDbPath,
                        Arguments = $"--datadir=../data",
                        WorkingDirectory = binDir,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    logger?.Report("Executing mariadb-install-db.exe --datadir=../data");

                    using (var process = Process.Start(startInfo))
                    {
                        if (process == null)
                        {
                            throw new InvalidOperationException("Failed to start MariaDB data directory initialization process");
                        }

                        var output = await process.StandardOutput.ReadToEndAsync();
                        var error = await process.StandardError.ReadToEndAsync();

                        process.WaitForExit();

                        if (process.ExitCode != 0)
                        {
                            throw new InvalidOperationException($"MariaDB data directory initialization failed with exit code {process.ExitCode}. Error: {error}");
                        }

                        logger?.Report("MariaDB data directory initialized successfully");
                        
                        if (!string.IsNullOrWhiteSpace(output))
                        {
                            logger?.Report($"MariaDB Init Output: {output}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger?.Report($"Error initializing MariaDB data directory: {ex.Message}");
                    throw;
                }
            }
        }
    }
}