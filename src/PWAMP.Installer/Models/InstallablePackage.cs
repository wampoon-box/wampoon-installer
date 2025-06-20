using System;
using System.Collections.Generic;

namespace PWAMP.Installer.Models
{
    public class InstallablePackage
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public Uri DownloadUrl { get; set; }
        public string ArchiveFormat { get; set; }
        public string InstallPath { get; set; }
        public string ChecksumUrl { get; set; }
        public long EstimatedSize { get; set; }
        public List<string> Dependencies { get; set; }
        public string Description { get; set; }
        public PackageType Type { get; set; }
        public string ServerName { get; set; }

        public InstallablePackage()
        {
            Dependencies = new List<string>();
            ArchiveFormat = "zip";
        }

        public string GetDisplayName()
        {
            return string.Format("{0} {1}", Name, Version);
        }

        public string GetSizeText()
        {
            if (EstimatedSize == 0) return "Unknown";
            
            var mb = EstimatedSize / (1024.0 * 1024.0);
            return mb < 1 ? string.Format("{0} KB", EstimatedSize / 1024) : string.Format("{0:F1} MB", mb);
        }
    }

    public enum PackageType
    {
        Apache,
        MariaDB,
        MySQL,
        PHP,
        PhpMyAdmin
    }
}