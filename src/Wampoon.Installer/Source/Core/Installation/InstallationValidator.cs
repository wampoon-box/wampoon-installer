using System;
using System.Threading.Tasks;
using Wampoon.Installer.Helpers.Common;
using Wampoon.Installer.Helpers;

namespace Wampoon.Installer.Core.Installation
{
    public class InstallationValidator : IInstallationValidator
    {
        public async Task ValidateInstallationPathAsync(string installPath)
        {
            if (string.IsNullOrWhiteSpace(installPath))
            {
                throw new ArgumentException("Installation path cannot be null or empty", nameof(installPath));
            }

            try
            {
                await FileHelper.CreateDirectoryIfNotExistsAsync(installPath);
            }
            catch (Exception ex)
            {
                ErrorLogHelper.LogExceptionInfo(ex);
                throw new Exception($"Cannot create installation directory: {ex.Message}", ex);
            }
        }

        public Task<bool> ValidatePackageInstallationAsync(string packageName, string installPath)
        {
            if (string.IsNullOrWhiteSpace(packageName))
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(FileHelper.ValidatePackageConfiguration(installPath, packageName));
        }

        public async Task<bool> ValidateCompleteInstallationAsync(InstallOptions options)
        {
            if (options == null)
            {
                return false;
            }

            var selectedPackages = options.GetSelectedPackages();
            var installPath = options.InstallPath;

            foreach (var package in selectedPackages)
            {
                var isValid = await ValidatePackageInstallationAsync(package, installPath);
                if (!isValid)
                {
                    return false;
                }
            }

            return true;
        }
    }
}