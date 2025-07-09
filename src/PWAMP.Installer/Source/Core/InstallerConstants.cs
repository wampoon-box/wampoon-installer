using System;

namespace Wampoon.Installer.Core
{
    public static class InstallerConstants
    {
        // Timeouts.
        public static readonly TimeSpan HttpClientTimeout = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan HttpShortTimeout = TimeSpan.FromMinutes(5);
        public static readonly int PingTimeout = 5000;
        
        // Progress reporting.
        public static readonly TimeSpan ProgressReportInterval = TimeSpan.FromMilliseconds(2000);
        public const int ProgressReportFrequency = 20; // Report every 1/20th of items.
        public const double ProgressReportPercentageThreshold = 10.0; // Report every 10% for downloads.
        
        // Buffer sizes - Optimized for modern file downloads.
        public const int FileStreamBufferSize = 65536; // 64KB
        public const int DownloadBufferSize = 65536; // 64KB
        public const int LargeDownloadBufferSize = 262144; // 256KB for files > 10MB
        public const int HugeDownloadBufferSize = 1048576; // 1MB for files > 100MB
        
        // Validation.
        public const long MinimumDiskSpace = 500L * 1024L * 1024L; // 500MB
        public const double FileSizeTolerancePercent = 0.1; // 10%
        
        // Log management.
        public const int MaxLogLines = 1000;
        public const int LogTrimLines = 500;
        
        // Paths.
        public const string DefaultInstallPath = @"C:\Wampoon";
        public const string PackagesFileName = "packagesInfo.json";
        public const string ConfigFileName = "config.json";
        public const string TempDirectoryPrefix = "Wampoon-Installer";
        public const string CacheDirectoryName = "Wampoon-Cache";
        
        // Security.
        public const int BlowfishSecretLength = 32;
        public static readonly string[] AllowedManifestDomains = { "raw.githubusercontent.com", "github.com", "api.github.com" };
        
        // UI.
        public const string DefaultFontFamily = "Segoe UI";
        public const float DefaultFontSize = 9F;
    }
}