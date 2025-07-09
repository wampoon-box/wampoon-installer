using System;
using System.Threading;
using System.Threading.Tasks;
using Wampoon.Installer.Events;
using Wampoon.Installer.Models;

namespace Wampoon.Installer.Core.PackageOperations
{
    public interface IPackageExtractionService : IDisposable
    {
        Task<string> ExtractPackageAsync(InstallablePackage package, string archivePath, string extractPath,
            IProgress<string> progress, CancellationToken cancellationToken = default);
        event EventHandler<InstallationProgressEventArgs> ExtractionProgressReported;
    }
}