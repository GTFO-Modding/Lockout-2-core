using BepInEx.Logging;

namespace Lockout_2_core
{
    internal static class L
    {
        private static readonly ManualLogSource _logger;

        static L()
        {
            // We create our private logger instance with the plugin name Mccad.Lockout2Core
            _logger = new ManualLogSource("Mccad.Lockout2Core");
            // We add the private logger instance to BepInEx's logger sources
            // so we will be able to output to the console
            Logger.Sources.Add(_logger);
        }

        // Helper method for formatting messages, currently converts the provided
        // 'msg' object to a string, but it can be extended however you want
        private static string Format(object msg) => msg.ToString();

        // Helper methods for logging

        public static void Info(object data) => _logger.LogMessage(Format(data));
        public static void Verbose(object data)
        {
            // This block of code will only compile if the user is compiling in Debug mode
            // so you can have more informative logging that will be automatically removed
            // when publishing your plugin.
#if DEBUG
            _logger.LogDebug(Format(data));
#endif
        }
        public static void Debug(object data) => _logger.LogDebug(Format(data));
        public static void Error(object data) => _logger.LogError(Format(data));
    }
}
