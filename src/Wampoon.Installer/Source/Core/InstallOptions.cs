using System;

namespace Wampoon.Installer.Core
{
    public class InstallOptions
    {
        public string InstallPath { get; set; }
        public bool InstallApache { get; set; }
        public bool InstallMariaDB { get; set; }
        public bool InstallPHP { get; set; }
        public bool InstallPhpMyAdmin { get; set; }
        public bool InstallDashboard { get; set; }
        public bool InstallControlPanel { get; set; }
        public bool InstallXdebug { get; set; }

        public InstallOptions()
        {
            InstallPath = @"C:\Wampoon";
            InstallApache = true;
            InstallMariaDB = true;
            InstallPHP = true;
            InstallPhpMyAdmin = true;
            InstallDashboard = true;
            InstallControlPanel = true;
            InstallXdebug = false;
        }

        public string[] GetSelectedPackages()
        {
            var packages = new System.Collections.Generic.List<string>();
            
            if (InstallApache) packages.Add(AppSettings.PackageNames.Apache);
            if (InstallMariaDB) packages.Add(AppSettings.PackageNames.MariaDB);
            if (InstallPHP) packages.Add(AppSettings.PackageNames.PHP);
            if (InstallPhpMyAdmin) packages.Add(AppSettings.PackageNames.PhpMyAdmin);
            if (InstallDashboard) packages.Add(AppSettings.PackageNames.Dashboard);
            if (InstallControlPanel) packages.Add(AppSettings.PackageNames.ControlPanel);
            if (InstallXdebug) packages.Add(AppSettings.PackageNames.Xdebug);
            
            return packages.ToArray();
        }
    }
}