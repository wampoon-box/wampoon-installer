namespace Wampoon.Installer.Core
{
    /// <summary>
    /// Defines the source for package information loading.
    /// </summary>
    public enum PackageSource
    {
        /// <summary>
        /// Automatically determine source: Local file first, then web manifest.
        /// </summary>
        Auto = 0,
        
        /// <summary>
        /// Use only local packagesInfo.json file for version control.
        /// </summary>
        LocalOnly = 1,
        
        /// <summary>
        /// Use only web manifest from GitHub for latest packages.
        /// </summary>
        WebOnly = 2
    }
    
    /// <summary>
    /// Extension methods for PackageSource enum.
    /// </summary>
    public static class PackageSourceExtensions
    {
        public static string GetDisplayName(this PackageSource source)
        {
            switch (source)
            {
                case PackageSource.Auto:
                    return "🔄 Auto (Local → Web)";
                case PackageSource.LocalOnly:
                    return "📁 Local File Only (Version Control)";
                case PackageSource.WebOnly:
                    return "🌐 Web Manifest Only (Latest Versions)";
                default:
                    return source.ToString();
            }
        }
        
        public static string GetDescription(this PackageSource source)
        {
            switch (source)
            {
                case PackageSource.Auto:
                    return "Tries local file first for fast startup, then web for updates.";
                case PackageSource.LocalOnly:
                    return "Uses only the local packagesInfo.json file. Allows you to control exact package versions.";
                case PackageSource.WebOnly:
                    return "Downloads package information from GitHub. Always gets the latest package versions.";
                default:
                    return "";
            }
        }
    }
}