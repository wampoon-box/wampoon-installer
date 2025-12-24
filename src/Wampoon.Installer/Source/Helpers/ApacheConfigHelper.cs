using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
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

                // Create SSL directory for HTTPS support.
                var sslDir = Path.Combine(confDir, "ssl");
                await FileHelper.CreateDirectoryIfNotExistsAsync(sslDir);
            }

            protected override async Task ConfigureAdditionalTemplatesAsync(IPathResolver pathResolver, IProgress<string> logger, CancellationToken cancellationToken = default)
            {
                // Copy the custom path file.
                var templateCustomPath = TemplateHelper.GetTemplatePath(AppSettings.ApacheFiles.Templates.WampoonCustomPathConf);
                var customConfTargetPath = pathResolver.GetConfigPath(AppSettings.PackageNames.Apache, AppSettings.ApacheFiles.WampoonCustomPathConf);
                TemplateHelper.CopyTemplateWithVersion(templateCustomPath, customConfTargetPath);

                // Copy the vhosts file.
                var templateVhostsPath = TemplateHelper.GetTemplatePath(AppSettings.ApacheFiles.Templates.WampoonVhostsConf);
                var vHostsConfTargetPath = pathResolver.GetConfigPath(AppSettings.PackageNames.Apache, AppSettings.ApacheFiles.WampoonVhostsConf);
                TemplateHelper.CopyTemplateWithVersion(templateVhostsPath, vHostsConfTargetPath);

                // Copy the HTTPS/SSL configuration file.
                var templateSslPath = TemplateHelper.GetTemplatePath(AppSettings.ApacheFiles.Templates.WampoonSslConf);
                var sslConfTargetPath = pathResolver.GetConfigPath(AppSettings.PackageNames.Apache, AppSettings.ApacheFiles.WampoonSslConf);
                TemplateHelper.CopyTemplateWithVersion(templateSslPath, sslConfTargetPath);

                // Generate SSL certificates for HTTPS support.
                await GenerateSSLCertificatesAsync(pathResolver, logger, cancellationToken);
            }

            private async Task GenerateSSLCertificatesAsync(IPathResolver pathResolver, IProgress<string> logger, CancellationToken cancellationToken = default)
            {
                try
                {
                    logger?.Report("Generating SSL certificates for HTTPS support...");

                    var apacheDir = pathResolver.GetPackageDirectory(AppSettings.PackageNames.Apache);
                    var opensslPath = Path.Combine(apacheDir, "bin", "openssl.exe");
                    var sslDir = Path.Combine(apacheDir, "conf", "ssl");
                    var keyPath = Path.Combine(sslDir, "server.key");
                    var certPath = Path.Combine(sslDir, "server.crt");

                    // Check if certificates already exist.
                    if (File.Exists(keyPath) && File.Exists(certPath))
                    {
                        logger?.Report("✓ SSL certificates already exist, skipping generation");
                        return;
                    }

                    // Check if OpenSSL exists.
                    if (!File.Exists(opensslPath))
                    {
                        logger?.Report("✗ OpenSSL not found, skipping SSL certificate generation");
                        return;
                    }

                    // Generate self-signed certificate valid for 10 years.
                    var opensslConfPath = Path.Combine(apacheDir, "conf", "openssl.cnf");
                    var arguments = $"req -x509 -nodes -days 3650 -newkey rsa:2048 " +
                                    $"-config \"{opensslConfPath}\" " +
                                    $"-keyout \"{keyPath}\" -out \"{certPath}\" " +
                                    $"-subj \"/CN=localhost\" " +
                                    $"-addext \"subjectAltName=DNS:localhost,DNS:*.localhost,IP:127.0.0.1\"";

                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = opensslPath,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        WorkingDirectory = sslDir
                    };

                    using (var process = new Process { StartInfo = processStartInfo })
                    {
                        process.Start();

                        var output = await process.StandardOutput.ReadToEndAsync();
                        var error = await process.StandardError.ReadToEndAsync();

                        await Task.Run(() => process.WaitForExit(), cancellationToken);

                        if (process.ExitCode == 0 && File.Exists(keyPath) && File.Exists(certPath))
                        {
                            logger?.Report("✓ SSL certificates generated successfully");
                        }
                        else
                        {
                            logger?.Report($"✗ Failed to generate SSL certificates: {error}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    logger?.Report("SSL certificate generation was cancelled");
                    throw;
                }
                catch (Exception ex)
                {
                    ErrorLogHelper.LogExceptionInfo(ex);
                    logger?.Report($"✗ Failed to generate SSL certificates: {ex.Message}");
                    // Don't throw - HTTPS is optional, installation can continue without it.
                }
            }
        }
    }
}