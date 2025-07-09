using System;
using System.Threading;
using System.Threading.Tasks;
using PWAMP.Installer.Neo.Events;
using PWAMP.Installer.Neo.Models;

namespace PWAMP.Installer.Neo.Core.PackageOperations
{
    public interface IPackageDownloadService : IDisposable
    {
        Task<string> DownloadPackageAsync(InstallablePackage package, string downloadDirectory, 
            IProgress<string> progress, CancellationToken cancellationToken = default);
        event EventHandler<DownloadProgressEventArgs> DownloadProgressReported;
    }
}