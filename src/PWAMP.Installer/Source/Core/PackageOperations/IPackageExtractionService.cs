using System;
using System.Threading;
using System.Threading.Tasks;
using PWAMP.Installer.Neo.Events;
using PWAMP.Installer.Neo.Models;

namespace PWAMP.Installer.Neo.Core.PackageOperations
{
    public interface IPackageExtractionService : IDisposable
    {
        Task<string> ExtractPackageAsync(InstallablePackage package, string archivePath, string extractPath,
            IProgress<string> progress, CancellationToken cancellationToken = default);
        event EventHandler<InstallationProgressEventArgs> ExtractionProgressReported;
    }
}