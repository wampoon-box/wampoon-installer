using System.Threading.Tasks;
using PWAMP.Installer.Neo.Models;

namespace PWAMP.Installer.Neo.Core.PackageDiscovery
{
    public interface IPackageDiscoveryService
    {
        Task<InstallablePackage> GetPackageByNameAsync(string packageName);
        Task<InstallablePackage[]> GetAvailablePackagesAsync();
        bool IsPackageSupported(string packageName);
        string GetPackageDirectoryName(string packageName);
    }
}