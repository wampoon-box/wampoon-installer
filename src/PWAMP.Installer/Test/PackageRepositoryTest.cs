using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PWAMP.Installer.Core;
using PWAMP.Installer.Models;

namespace PWAMP.Installer.Test
{
    /// <summary>
    /// Simple test class to validate PackageRepository functionality
    /// This is not a formal unit test framework, just validation code.
    /// </summary>
    public static class PackageRepositoryTest
    {
        public static async Task RunAllTests()
        {
            Console.WriteLine("Running PackageRepository Tests...");
            
            await TestJsonLoading();
            await TestFallbackBehavior();
            await TestDependencyResolution();
            
            Console.WriteLine("All tests completed.");
        }

        private static async Task TestJsonLoading()
        {
            Console.WriteLine("Test 1: JSON Loading...");
            
            try
            {
                using (var repo = new PackageRepository())
                {
                    var packages = await repo.GetAvailablePackagesAsync();
                    
                    if (packages == null || !packages.Any())
                    {
                        Console.WriteLine("❌ No packages loaded");
                        return;
                    }
                    
                    Console.WriteLine($"✅ Loaded {packages.Count} packages");
                    
                    // Verify essential packages
                    var apache = packages.FirstOrDefault(p => p.Type == PackageType.Apache);
                    var mariadb = packages.FirstOrDefault(p => p.Type == PackageType.MariaDB);
                    var php = packages.FirstOrDefault(p => p.Type == PackageType.PHP);
                    
                    if (apache != null) Console.WriteLine($"✅ Apache found: {apache.Name} {apache.Version}");
                    if (mariadb != null) Console.WriteLine($"✅ MariaDB found: {mariadb.Name} {mariadb.Version}");
                    if (php != null) Console.WriteLine($"✅ PHP found: {php.Name} {php.Version}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test failed: {ex.Message}");
            }
        }

        private static async Task TestFallbackBehavior()
        {
            Console.WriteLine("\nTest 2: Fallback Behavior...");
            
            try
            {
                // Temporarily rename the JSON file to test fallback.
                var dataDir = Path.Combine(Environment.CurrentDirectory, "Data");
                var jsonPath = Path.Combine(dataDir, "packages.json");
                var backupPath = Path.Combine(dataDir, "packages.json.backup");
                
                bool fileRenamed = false;
                if (File.Exists(jsonPath))
                {
                    File.Move(jsonPath, backupPath);
                    fileRenamed = true;
                }
                
                using (var repo = new PackageRepository())
                {
                    var packages = await repo.GetAvailablePackagesAsync();
                    
                    if (packages != null && packages.Any())
                    {
                        Console.WriteLine($"✅ Fallback worked: {packages.Count} packages loaded");
                    }
                    else
                    {
                        Console.WriteLine("❌ Fallback failed: no packages loaded");
                    }
                }
                
                // Restore the file.    
                if (fileRenamed && File.Exists(backupPath))
                {
                    File.Move(backupPath, jsonPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test failed: {ex.Message}");
            }
        }

        private static async Task TestDependencyResolution()
        {
            Console.WriteLine("\nTest 3: Dependency Resolution...");
            
            try
            {
                using (var repo = new PackageRepository())
                {
                    var packages = await repo.GetAvailablePackagesAsync();
                    var phpMyAdmin = packages.FirstOrDefault(p => p.Type == PackageType.PhpMyAdmin);
                    
                    if (phpMyAdmin != null)
                    {
                        var selected = new[] { phpMyAdmin }.ToList();
                        var resolved = repo.ResolveDependencies(selected);
                        
                        Console.WriteLine($"✅ Selected phpMyAdmin, resolved {resolved.Count} packages:");
                        foreach (var pkg in resolved)
                        {
                            Console.WriteLine($"  - {pkg.Name}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("❌ phpMyAdmin not found for dependency test");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Test failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Simple method to test JSON serialization/deserialization.
        /// </summary>
        public static void TestJsonSerialization()
        {
            Console.WriteLine("\nTest 4: JSON Serialization...");
            
            try
            {
                var testPackage = new InstallablePackage
                {
                    Name = "Test Package",
                    Version = new Version(1, 0, 0),
                    DownloadUrl = new Uri("https://example.com/test.zip"),
                    Type = PackageType.Apache,
                    ServerName = "Test",
                    EstimatedSize = 1024,
                    Description = "Test package for serialization",
                    InstallPath = "test/path",
                    Dependencies = new System.Collections.Generic.List<string> { "Dependency1" }
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(testPackage, Newtonsoft.Json.Formatting.Indented);
                Console.WriteLine("✅ Serialization successful");
                
                var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<InstallablePackage>(json);
                
                if (deserialized != null && 
                    deserialized.Name == testPackage.Name && 
                    deserialized.Version.ToString() == testPackage.Version.ToString() &&
                    deserialized.DownloadUrl.ToString() == testPackage.DownloadUrl.ToString())
                {
                    Console.WriteLine("✅ Deserialization successful");
                }
                else
                {
                    Console.WriteLine("❌ Deserialization failed - data mismatch");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Serialization test failed: {ex.Message}");
            }
        }
    }
} 