using System;

namespace Wampoon.Installer.Helpers.Logging
{
    public class ProgressToLoggerAdapter : IProgress<string>
    {
        private readonly ILogger _logger;
        private readonly string _packageName;

        public ProgressToLoggerAdapter(ILogger logger, string packageName = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _packageName = packageName;
        }

        public void Report(string value)
        {
            if (string.IsNullOrEmpty(value))
                return;

            // Simple heuristic to determine log level based on message content.
            if (value.StartsWith("✗") || value.Contains("failed") || value.Contains("error"))
            {
                _logger.LogError(value, _packageName);
            }
            else if (value.StartsWith("⚠") || value.Contains("warning"))
            {
                _logger.LogWarning(value, _packageName);
            }
            else
            {
                _logger.LogInfo(value, _packageName);
            }
        }
    }
}