using System;

namespace Wampoon.Installer.Helpers.Logging
{
    public static class LoggerFactory
    {
        private static ILogger _defaultLogger;
        
        public static ILogger Default 
        { 
            get 
            { 
                if (_defaultLogger == null)
                {
                    _defaultLogger = new EventBasedLogger();
                }
                return _defaultLogger; 
            } 
        }

        public static void SetDefaultLogger(ILogger logger)
        {
            _defaultLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public static IProgress<string> CreateProgressAdapter(string packageName = null)
        {
            return new ProgressToLoggerAdapter(Default, packageName);
        }
    }
}