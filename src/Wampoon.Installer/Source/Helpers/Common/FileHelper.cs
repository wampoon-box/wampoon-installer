using System;
using System.IO;
using System.Threading.Tasks;

namespace Wampoon.Installer.Helpers.Common
{
    public static class FileHelper
    {
        public static async Task CreateDirectoryIfNotExistsAsync(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                await Task.Delay(10); // Small delay to ensure directory creation.
            }
        }

        public static bool ValidateFileExists(string filePath, string description = "")
        {
            var exists = File.Exists(filePath);
            if (!exists)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {description} not found at: {filePath}");
            }
            return exists;
        }

        public static bool ValidateDirectoryExists(string directoryPath, string description = "")
        {
            var exists = Directory.Exists(directoryPath);
            if (!exists)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {description} directory not found at: {directoryPath}");
            }
            return exists;
        }

        public static bool ValidatePackagePrerequisites(string installPath, string packageName)
        {
            switch (packageName.ToLower())
            {
                case "apache":
                    var apachePath = Path.Combine(installPath, "apps", "apache", "bin", "httpd.exe");
                    var apacheExists = ValidateFileExists(apachePath, "Apache binary");
                    
                    // Also check the Apache24 path in case the folder structure wasn't flattened.
                    if (!apacheExists)
                    {
                        var apache24Path = Path.Combine(installPath, "apps", "apache", "Apache24", "bin", "httpd.exe");
                        apacheExists = ValidateFileExists(apache24Path, "Apache binary (Apache24 folder)");
                    }
                    
                    return apacheExists;
                case "mariadb":
                    return ValidateFileExists(Path.Combine(installPath, "apps", "mariadb", "bin", "mysqld.exe"), "MariaDB binary");
                case "php":
                    return ValidateFileExists(Path.Combine(installPath, "apps", "php", "php.exe"), "PHP binary");
                case "phpmyadmin":
                    return ValidateFileExists(Path.Combine(installPath, "apps", "phpmyadmin", "index.php"), "phpMyAdmin");
                default:
                    return true; // Unknown packages are considered valid for extensibility
            }
        }

        public static bool ValidatePackageConfiguration(string installPath, string packageName)
        {
            switch (packageName.ToLower())
            {
                case "apache":
                    return ValidateFileExists(Path.Combine(installPath, "apps", "apache", "conf", "httpd.conf"), "Apache configuration");
                case "mariadb":
                    return ValidateFileExists(Path.Combine(installPath, "apps", "mariadb", "my.ini"), "MariaDB configuration");
                case "php":
                    return ValidateFileExists(Path.Combine(installPath, "apps", "php", "php.ini"), "PHP configuration");
                case "phpmyadmin":
                    return ValidateFileExists(Path.Combine(installPath, "apps", "phpmyadmin", "config.inc.php"), "phpMyAdmin configuration");
                default:
                    return true; // Unknown packages are considered valid for extensibility
            }
        }

        public static async Task MoveDirectoryContentsAsync(string sourceDirectory, string targetDirectory)
        {
            if (!Directory.Exists(sourceDirectory))
            {
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDirectory}");
            }

            await CreateDirectoryIfNotExistsAsync(targetDirectory);

            // Move all files
            var files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var relativePath = file.Substring(sourceDirectory.Length + 1);
                var targetFile = Path.Combine(targetDirectory, relativePath);
                var targetDir = Path.GetDirectoryName(targetFile);
                
                await CreateDirectoryIfNotExistsAsync(targetDir);
                
                // If target file exists, delete it first.  
                if (File.Exists(targetFile))
                {
                    File.Delete(targetFile);
                }
                
                File.Move(file, targetFile);
            }

            // Move all empty directories.
            var directories = Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories);
            foreach (var directory in directories)
            {
                var relativePath = directory.Substring(sourceDirectory.Length + 1);
                var targetDir = Path.Combine(targetDirectory, relativePath);
                
                if (!Directory.Exists(targetDir))
                {
                    await CreateDirectoryIfNotExistsAsync(targetDir);
                }
            }
        }
    }
}