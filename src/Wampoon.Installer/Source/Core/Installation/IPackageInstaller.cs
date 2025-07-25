using System;
using System.Threading;
using System.Threading.Tasks;
using Wampoon.Installer.Core.Paths;

namespace Wampoon.Installer.Core.Installation
{
    public interface IPackageInstaller
    {
        Task InstallAsync(string packageName, string installPath, IProgress<string> progress, CancellationToken cancellationToken);
        Task ConfigureAsync(string packageName, IPathResolver pathResolver, IProgress<string> progress, CancellationToken cancellationToken);
        Task<bool> ValidateAsync(string packageName, string installPath);
        bool CanInstall(string packageName);
        bool RequiresPostInstallProcessing(string packageName);
        Task PostInstallProcessAsync(string packageName, string installPath, IProgress<string> progress);
    }
}