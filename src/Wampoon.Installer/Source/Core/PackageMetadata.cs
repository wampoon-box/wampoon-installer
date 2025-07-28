using System;
using System.Collections.Generic;
using Wampoon.Installer.Models;

namespace Wampoon.Installer.Core
{
    public static class PackageMetadata
    {
        public static readonly Dictionary<PackageType, PackageMetadataInfo> PackageMap = new Dictionary<PackageType, PackageMetadataInfo>
        {
            {
                PackageType.Apache, new PackageMetadataInfo
                {
                    Type = PackageType.Apache,
                    ServerName = "Apache",
                    EstimatedSize = 12582912,
                    Description = "Apache HTTP Server - The world's most widely used web server software.",
                    InstallPath = "apps/apache",
                    ArchiveFormat = "zip",
                    Dependencies = new List<PackageType>()
                }
            },
            {
                PackageType.MariaDB, new PackageMetadataInfo
                {
                    Type = PackageType.MariaDB,
                    ServerName = "MariaDB",
                    EstimatedSize = 94371840,
                    Description = "MariaDB Server - Popular MySQL-compatible database server.",
                    InstallPath = "apps/mariadb",
                    ArchiveFormat = "zip",
                    Dependencies = new List<PackageType>()
                }
            },
            {
                PackageType.PHP, new PackageMetadataInfo
                {
                    Type = PackageType.PHP,
                    ServerName = "PHP",
                    EstimatedSize = 34078720,
                    Description = "PHP - Server-side scripting language designed for web development.",
                    InstallPath = "apps/php",
                    ArchiveFormat = "zip",
                    Dependencies = new List<PackageType> { PackageType.Apache }
                }
            },
            {
                PackageType.PhpMyAdmin, new PackageMetadataInfo
                {
                    Type = PackageType.PhpMyAdmin,
                    ServerName = "phpMyAdmin",
                    EstimatedSize = 15100000,
                    Description = "phpMyAdmin - Web-based database administration tool for MySQL and MariaDB.",
                    InstallPath = "apps/phpmyadmin",
                    ArchiveFormat = "zip",
                    Dependencies = new List<PackageType> { PackageType.PHP, PackageType.MariaDB }
                }
            },
            {
                PackageType.WampoonDashboard, new PackageMetadataInfo
                {
                    Type = PackageType.WampoonDashboard,
                    ServerName = "Wampoon Dashboard",
                    EstimatedSize = 5000000,
                    Description = "Wampoon Dashboard - Web-based management interface for Wampoon.",
                    InstallPath = "apps/wampoon-dashboard",
                    ArchiveFormat = "zip",
                    Dependencies = new List<PackageType>()
                }
            },
            {
                PackageType.WampoonControlPanel, new PackageMetadataInfo
                {
                    Type = PackageType.WampoonControlPanel,
                    ServerName = "Wampoon Control Panel",
                    EstimatedSize = 3000000,
                    Description = "Wampoon Control Panel - Desktop control panel for managing Wampoon services.",
                    InstallPath = "wampoon-control-panel",
                    ArchiveFormat = "zip",
                    Dependencies = new List<PackageType>()
                }
            },
            {
                PackageType.Xdebug, new PackageMetadataInfo
                {
                    Type = PackageType.Xdebug,
                    ServerName = "Xdebug",
                    EstimatedSize = 500000,
                    Description = "Xdebug - PHP extension for debugging and profiling.",
                    InstallPath = "apps/php/ext",
                    ArchiveFormat = "dll",
                    Dependencies = new List<PackageType> { PackageType.PHP }
                }
            }
        };

        public static PackageMetadataInfo GetMetadata(PackageType packageId)
        {
            return PackageMap.TryGetValue(packageId, out var metadata) ? metadata : null;
        }
    }

    public class PackageMetadataInfo
    {
        public PackageType Type { get; set; }
        public string ServerName { get; set; }
        public long EstimatedSize { get; set; }
        public string Description { get; set; }
        public string InstallPath { get; set; }
        public string ArchiveFormat { get; set; }
        public List<PackageType> Dependencies { get; set; }

        public PackageMetadataInfo()
        {
            Dependencies = new List<PackageType>();
        }
    }
}