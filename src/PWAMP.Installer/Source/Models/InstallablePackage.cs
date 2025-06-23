using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PWAMP.Installer.Models
{
    public class InstallablePackage
    {
        public string Name { get; set; }
        
        [JsonConverter(typeof(VersionConverter))]
        public Version Version { get; set; }
        
        [JsonConverter(typeof(UriConverter))]
        public Uri DownloadUrl { get; set; }
        
        public string ArchiveFormat { get; set; }
        public string InstallPath { get; set; }
        public string ChecksumUrl { get; set; }
        public long EstimatedSize { get; set; }
        public List<string> Dependencies { get; set; }
        public string Description { get; set; }
        
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public PackageType Type { get; set; }
        
        public string ServerName { get; set; }

        public InstallablePackage()
        {
            Dependencies = new List<string>();
            ArchiveFormat = "zip";
        }

        public string GetDisplayName()
        {
            return $"{Name} {Version}";
        }

        public string GetSizeText()
        {
            if (EstimatedSize == 0) return "Unknown";
            
            var mb = EstimatedSize / (1024.0 * 1024.0);
            return mb < 1 ? $"{EstimatedSize / 1024} KB" : $"{mb:F1} MB";
        }
    }

    public enum PackageType
    {
        Apache,
        MariaDB,
        MySQL,
        PHP,
        PhpMyAdmin,
        PwampDashboard
    }

    /// <summary>
    /// Custom converter for Version type.
    /// </summary>
    public class VersionConverter : JsonConverter<Version>
    {
        public override void WriteJson(JsonWriter writer, Version value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }

        public override Version ReadJson(JsonReader reader, Type objectType, Version existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string versionString = reader.Value as string;
            return versionString != null ? new Version(versionString) : null;
        }
    }

    /// <summary>
    /// Custom converter for Uri type.
    /// </summary>
    public class UriConverter : JsonConverter<Uri>
    {
        public override void WriteJson(JsonWriter writer, Uri value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }

        public override Uri ReadJson(JsonReader reader, Type objectType, Uri existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string uriString = reader.Value as string;
            return uriString != null ? new Uri(uriString) : null;
        }
    }
}