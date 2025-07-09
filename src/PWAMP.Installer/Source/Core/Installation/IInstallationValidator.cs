using System.Threading.Tasks;

namespace Wampoon.Installer.Core.Installation
{
    public interface IInstallationValidator
    {
        Task ValidateInstallationPathAsync(string installPath);
        Task<bool> ValidatePackageInstallationAsync(string packageName, string installPath);
        Task<bool> ValidateCompleteInstallationAsync(InstallOptions options);
    }
}