namespace Wampoon.Installer.Core
{
    /// <summary>
    /// Centralized constants for WAMPoon package names and file names.        
    /// </summary>
    public static class AppSettings
    {


        public static class PackageNames
        {
            /// <summary>
            /// Apache HTTP Server package identifier.
            /// </summary>
            public const string Apache = "apache";

            /// <summary>
            /// MariaDB Database Server package identifier.
            /// </summary>
            public const string MariaDB = "mariadb";

            /// <summary>
            /// PHP Scripting Language package identifier.
            /// </summary>
            public const string PHP = "php";

            /// <summary>
            /// phpMyAdmin Database Manager package identifier.
            /// </summary>
            public const string PhpMyAdmin = "phpmyadmin";

            /// <summary>
            /// Gets all available package names.
            /// </summary>
            /// <returns>Array of all package names</returns>
            public static string[] GetAllPackageNames()
            {
                return new[] { Apache, MariaDB, PHP, PhpMyAdmin };
            }

            /// <summary>
            /// Validates if a package name is supported.
            /// </summary>
            /// <param name="packageName">The package name to validate</param>
            /// <returns>True if the package is supported, false otherwise</returns>
            public static bool IsValidPackageName(string packageName)
            {
                switch (packageName)
                {
                    case Apache:
                    case MariaDB:
                    case PHP:
                    case PhpMyAdmin:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// Apache HTTP Server file names and configurations.
        /// </summary>
        public static class ApacheFiles
        {
            // Binary files.
            public const string HttpdExe = "httpd.exe";

            // Configuration files.
            public const string HttpdConf = "httpd.conf";
            public const string WampoonCustomPathConf = "wampoon-custom-path.conf";
            public const string WampoonVhostsConf = "wampoon-vhosts.conf";

            // Template names (used for TemplateHelper.GetTemplatePath)
            public static class Templates
            {
                public const string HttpdConf = "httpd.conf";
                public const string WampoonCustomPathConf = "wampoon-custom-path.conf";
                public const string WampoonVhostsConf = "wampoon-vhosts.conf";
            }
        }

        /// <summary>
        /// MariaDB Database Server file names and configurations.
        /// </summary>
        public static class MariaDBFiles
        {
            // Binary files.
            public const string MysqldExe = "mysqld.exe";
            public const string MysqlExe = "mysql.exe";
            public const string MariaDbInstallDbExe = "mariadb-install-db.exe";

            // Configuration files.
            public const string MyIni = "my.ini";
            public const string MyCnf = "my.cnf";

            // Template names
            public static class Templates
            {
                public const string MyIni = "my.ini";
            }
        }

        /// <summary>
        /// PHP Scripting Language file names and configurations.
        /// </summary>
        public static class PHPFiles
        {
            // Binary files.
            public const string PhpExe = "php.exe";
            public const string PhpCgiExe = "php-cgi.exe";

            // Configuration files.
            public const string PhpIni = "php.ini";
            public const string PhpIniDevelopment = "php.ini-development";
            public const string PhpIniProduction = "php.ini-production";

            // Template names
            public static class Templates
            {
                public const string PhpIni = "php.ini";
            }
        }

        /// <summary>
        /// phpMyAdmin Database Manager file names and configurations.
        /// </summary>
        public static class PhpMyAdminFiles
        {
            // Application files.
            public const string IndexPhp = "index.php";

            // Configuration files.
            public const string ConfigIncPhp = "config.inc.php";
            public const string ConfigSampleIncPhp = "config.sample.inc.php";

            // Template names.
            public static class Templates
            {
                public const string ConfigIncPhp = "config.inc.php";
            }
        }
    }
}