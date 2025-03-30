using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Terraria.ModLoader;

namespace ModHelper.Helpers
{
    public static class Log
    {
        // Log a message once every 5 second
        private static DateTime lastLogTime = DateTime.UtcNow;

        private static Mod ModInstance
        {
            // try catch get
            get
            {
                try
                {
                    return ModLoader.GetMod("ModHelper");
                }
                catch (Exception ex)
                {
                    Error("Error getting mod instance: " + ex.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Log a message once every 3 second
        /// </summary>
        public static void SlowInfo(string message, int seconds = 3, [CallerFilePath] string callerFilePath = "")
        {
            // Extract the class name from the caller's file path.
            string className = Path.GetFileNameWithoutExtension(callerFilePath);
            var instance = ModInstance;
            if (instance == null || instance.Logger == null)
                return; // Skip logging if the mod is unloading or null

            // Use TimeSpanFactory to create a 3-second interval.
            TimeSpan interval = TimeHelper.FromSeconds(seconds);
            if (DateTime.UtcNow - lastLogTime >= interval)
            {
                // Prepend the class name to the log message.
                instance.Logger.Info($"[{className}] {message}");
                lastLogTime = DateTime.UtcNow;
            }
        }

        public static void Info(string message, [CallerFilePath] string callerFilePath = "")
        {
            // Extract the class name from the caller's file path.
            string className = Path.GetFileNameWithoutExtension(callerFilePath);
            var instance = ModInstance;
            if (instance == null || instance.Logger == null)
                return; // Skip logging if the mod is unloading or null

            // Prepend the class name to the log message.
            instance.Logger.Info($"[{className}] {message}");
        }

        public static void Warn(string message)
        {
            var instance = ModInstance;
            if (instance == null || instance.Logger == null)
                return; // Skip logging if the mod is unloading or null

            instance.Logger.Warn(message);
        }

        public static void Error(string message)
        {
            var instance = ModInstance;
            if (instance == null || instance.Logger == null)
                return; // Skip logging if the mod is unloading or null

            instance.Logger.Error(message);
        }

        public static string GetSteamPath()
        {
            // get the DLL file from the steam path
            string tMLDLL = Assembly.GetEntryAssembly()?.Location;
            string steamPath = Path.GetDirectoryName(tMLDLL);
            return steamPath;
        }
    }
}