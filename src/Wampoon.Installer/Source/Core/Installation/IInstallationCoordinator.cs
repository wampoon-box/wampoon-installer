using System;
using System.Threading;
using System.Threading.Tasks;

namespace Wampoon.Installer.Core.Installation
{
    public interface IInstallationCoordinator
    {
        Task ExecuteInstallationAsync(InstallOptions options, IProgress<string> progress, CancellationToken cancellationToken);
        Task ExecuteConfigurationAsync(string[] packageNames, IProgress<string> progress, CancellationToken cancellationToken);
        Task<bool> ValidateInstallationAsync(InstallOptions options);
    }
}