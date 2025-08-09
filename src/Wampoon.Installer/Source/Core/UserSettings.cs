using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Wampoon.Installer.Helpers.Logging;

namespace Wampoon.Installer.Core
{
    /// <summary>
    /// Manages user preferences and settings for the Wampoon Installer.
    /// </summary>
    public class UserSettings
    {
        private static readonly string SettingsFilePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory,
            "user-settings.json");

        private static UserSettings _instance;
        private static readonly object _lock = new object();
        private readonly ILogger _logger;

        /// <summary>
        /// Package source preference for loading package information.
        /// </summary>
        public PackageSource PackageSource { get; set; } = PackageSource.Auto;

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
            _logger = LoggerFactory.Default;
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
                            _instance.Load();
                        }
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Loads settings from the settings file.
        /// </summary>
        public void Load()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    var loadedSettings = JsonConvert.DeserializeObject<UserSettings>(json);
                    if (loadedSettings != null)
                    {
                        PackageSource = loadedSettings.PackageSource;
                        LastInstallPath = loadedSettings.LastInstallPath ?? InstallerConstants.DefaultInstallPath;
                        ShowAdvancedOptions = loadedSettings.ShowAdvancedOptions;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not load user settings: {ex.Message}. Using defaults.");
            }
        }

        /// <summary>
        /// Saves settings to the settings file.
        /// </summary>
        public void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(SettingsFilePath, json);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Could not save user settings: {ex.Message}");
            }
        }

        /// <summary>
        /// Resets settings to defaults and saves.
        /// </summary>
        public void Reset()
        {
            PackageSource = PackageSource.Auto;
            LastInstallPath = InstallerConstants.DefaultInstallPath;
            ShowAdvancedOptions = false;
            Save();
        }
    }
}