# Making WAMP Portable

## PHP

- Download link: https://windows.php.net/downloads/releases/
- Should get the thread safe version.
https://windows.php.net/downloads/releases/php-8.4.7-Win32-vs17-x64.zip

## Apache

- What Apache version should I get? safe thread?
- Download link: https://www.apachelounge.com/download/
https://www.apachelounge.com/download/VS17/binaries/httpd-2.4.63-250207-win64-VS17.zip

### Steps to Make Apache Portable

- Create a custom config file containing `Define` directives
Define SRVROOT "D:\DevWorkspace\XAMPP\pwamp\pwampp\apache"
Define DOCROOT "D:\DevWorkspace\XAMPP\pwamp\pwampp\htdocs"

#### Apache Modules

- `LoadModule rewrite_module modules/mod_rewrite.so`


## MariaDB

- Initialize the Data Directory:
 `bin\mysqld --initialize-insecure --basedir=. --datadir=./data`
