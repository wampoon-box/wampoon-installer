using System;

namespace Wampoon.Installer.Core
{
    /// <summary>
    /// Manages user preferences and settings for the Wampoon Installer.
    /// Settings are stored in memory only for this installation session.
    /// </summary>
    public class UserSettings
    {
        private static UserSettings _instance;
        private static readonly object _lock = new object();

        /// <summary>
        /// Package source preference for loading package information.
        /// </summary>
        public PackageSource PackageSource { get; set; } = PackageSource.WebOnly;

        /// <summary>
        /// Last used installation path.
        /// </summary>
        public string LastInstallPath { get; set; } = InstallerConstants.DefaultInstallPath;

        /// <summary>
        /// Whether to show advanced options by default.
        /// </summary>
        public bool ShowAdvancedOptions { get; set; } = false;

        private UserSettings()
        {
        }

        /// <summary>
        /// Gets the singleton instance of UserSettings.
        /// </summary>
        public static UserSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new UserSettings();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Resets settings to defaults.
        /// </summary>
        public void Reset()
        {
            PackageSource = PackageSource.WebOnly;
            LastInstallPath = InstallerConstants.DefaultInstallPath;
            ShowAdvancedOptions = false;
        }
    }
}