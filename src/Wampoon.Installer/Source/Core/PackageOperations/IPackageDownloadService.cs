using System;
using System.Threading;
using System.Threading.Tasks;
using Wampoon.Installer.Events;
using Wampoon.Installer.Models;

namespace Wampoon.Installer.Core.PackageOperations
{
    public interface IPackageDownloadService : IDisposable
    {
        Task<string> DownloadPackageAsync(InstallablePackage package, string downloadDirectory, 
            IProgress<string> progress, CancellationToken cancellationToken = default);
        event EventHandler<DownloadProgressEventArgs> DownloadProgressReported;
    }
}