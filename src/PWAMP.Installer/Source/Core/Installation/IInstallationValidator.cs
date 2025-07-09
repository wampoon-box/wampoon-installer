using System.Threading.Tasks;

namespace PWAMP.Installer.Neo.Core.Installation
{
    public interface IInstallationValidator
    {
        Task ValidateInstallationPathAsync(string installPath);
        Task<bool> ValidatePackageInstallationAsync(string packageName, string installPath);
        Task<bool> ValidateCompleteInstallationAsync(InstallOptions options);
    }
}