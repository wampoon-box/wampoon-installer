using System;

namespace Wampoon.Installer.Core.Events
{
    public class ExistingPackagesEventArgs : EventArgs
    {
        public string[] ExistingPackages { get; }
        public bool OverwriteRequested { get; set; }

        public ExistingPackagesEventArgs(string[] existingPackages)
        {
            ExistingPackages = existingPackages ?? throw new ArgumentNullException(nameof(existingPackages));
            OverwriteRequested = false;
        }
    }
}