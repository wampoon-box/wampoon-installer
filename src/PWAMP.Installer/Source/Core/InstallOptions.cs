using System;

namespace PWAMP.Installer.Neo.Core
{
    public class InstallOptions
    {
        public string InstallPath { get; set; }
        public bool InstallApache { get; set; }
        public bool InstallMariaDB { get; set; }
        public bool InstallPHP { get; set; }
        public bool InstallPhpMyAdmin { get; set; }

        public InstallOptions()
        {
            InstallPath = @"C:\PWAMP";
            InstallApache = true;
            InstallMariaDB = true;
            InstallPHP = true;
            InstallPhpMyAdmin = true;
        }

        public string[] GetSelectedPackages()
        {
            var packages = new System.Collections.Generic.List<string>();
            
            if (InstallApache) packages.Add("apache");
            if (InstallMariaDB) packages.Add("mariadb");
            if (InstallPHP) packages.Add("php");
            if (InstallPhpMyAdmin) packages.Add("phpmyadmin");
            
            return packages.ToArray();
        }
    }
}