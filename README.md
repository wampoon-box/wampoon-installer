# Wampoon Installer

An installer for a portable WAMP (Windows, Apache, MariaDB, PHP) stack, giving you a ready-to-use local web development setup without complex configuration

## Installation

Download and run `Wampoon-Installer.exe` to set up your local WAMP environment.

## Supported Packages

Wampoon Installer includes the latest versions of the following components:

- **Apache**: Web server for hosting your applications,
- **MariaDB**: Database server (MySQL-compatible),
- **PHP**: Server-side scripting language,
- **phpMyAdmin**:  Web-based database administration tool.
- **Composer**:  Dependency manager for PHP projects.
- **XDebug**:  PHP extension for debugging and profiling applications.
- **Wampoon Dashboard**:  Centralized interface to monitor and manage your WAMP stack.
- **Wampoon Control Panel**:  Desktop app to start, stop, and control your local servers..

## Building the Installer

This installer is written in C#/.NET Framework 4.8.

### Prerequisites

- Visual Studio 2022 or later with .NET Framework 4.8 development tools (already included in Windows 10 or later).

### Build Steps

1. Clone the repository
2. Open the solution file in Visual Studio
3. Restore NuGet packages if needed
4. Build the solution (Build → Build Solution or Ctrl+Shift+B)
5. The installer executable will be generated in the output directory

## Issues

If you encounter any issues or have suggestions for improvements, please [report them on the project's GitHub repository](https://github.com/wampoon-box/wampoon-installer/issues).

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues on the GitHub repository:

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
