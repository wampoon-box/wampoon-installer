using System.Threading.Tasks;
using Wampoon.Installer.Models;

namespace Wampoon.Installer.Core.PackageDiscovery
{
    public interface IPackageDiscoveryService
    {
        Task<InstallablePackage> GetPackageByNameAsync(string packageName);
        Task<InstallablePackage[]> GetAvailablePackagesAsync();
        bool IsPackageSupported(string packageName);
        string GetPackageDirectoryName(string packageName);
    }
}