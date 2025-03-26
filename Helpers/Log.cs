using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using EliteTestingMod.Common.Configs;
using log4net;
using log4net.Appender;
using Terraria;
using Terraria.ModLoader;

namespace EliteTestingMod.Helpers
{
    /// <summary>
    /// Factory class for creating TimeSpans.
    /// </summary>
    public static class TimeSpanFactory
    {
        public static TimeSpan FromSeconds(double seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }
    }

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
                    return ModLoader.GetMod("EliteTestingMod");
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
        public static void SlowInfo(string message, int seconds = 3)
        {
            Config c = ModContent.GetInstance<Config>();
            if (c != null && Conf.LogToLogFile == false) return;

            // Use TimeSpanFactory to create a 3-second interval.
            TimeSpan interval = TimeSpanFactory.FromSeconds(seconds);
            if (DateTime.UtcNow - lastLogTime >= interval)
            {
                ModInstance.Logger.Info(message);
                lastLogTime = DateTime.UtcNow;
            }
        }

        public static void Info(string message, [CallerFilePath] string callerFilePath = "")
        {
            Config c = ModContent.GetInstance<Config>();
            if (c != null && Conf.LogToLogFile == false) return;

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
            Config c = ModContent.GetInstance<Config>();
            if (c != null && Conf.LogToLogFile == false) return;

            var instance = ModInstance;
            if (instance == null || instance.Logger == null)
                return; // Skip logging if the mod is unloading or null

            instance.Logger.Warn(message);
        }

        public static void Error(string message)
        {
            Config c = ModContent.GetInstance<Config>();
            if (c != null && Conf.LogToLogFile == false) return;

            var instance = ModInstance;
            if (instance == null || instance.Logger == null)
                return; // Skip logging if the mod is unloading or null

            instance.Logger.Error(message);
        }

        public static void ClearClientLog()
        {
            // Get all file appenders from log4net's repository
            var appenders = LogManager.GetRepository().GetAppenders().OfType<FileAppender>();

            foreach (var appender in appenders)
            {
                // Close the file to release the lock.
                var closeFileMethod = typeof(FileAppender).GetMethod("CloseFile", BindingFlags.NonPublic | BindingFlags.Instance);
                closeFileMethod?.Invoke(appender, null);

                // Overwrite the file with an empty string.
                File.WriteAllText(appender.File, string.Empty);

                // Reactivate the appender so that logging resumes.
                appender.ActivateOptions();
            }
            if (Conf.LogToChat) Main.NewText("Client.log cleared.");
        }

        public static void OpenLogFolder()
        {
            if (Conf.LogToChat) Main.NewText("Opening log folder...");

            try
            {
                string steamPath = GetSteamPath();
                if (string.IsNullOrEmpty(steamPath))
                {
                    if (Conf.LogToChat) Main.NewText("Steam path not found.");
                    Error("Steam path not found.");
                    return;
                }

                string folder = Path.Combine(GetSteamPath(), "tModLoader-Logs");
                Process.Start(new ProcessStartInfo($@"{folder}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                if (Conf.LogToChat) Main.NewText("Error opening log folder: " + ex.Message);
                Error("Error opening log folder: " + ex.Message);
            }
        }

        public static string GetSteamPath()
        {
            // get the DLL file from the steam path
            string tMLDLL = Assembly.GetEntryAssembly()?.Location;
            string steamPath = Path.GetDirectoryName(tMLDLL);
            return steamPath;
        }

        public static void OpenClientLog()
        {
            if (Conf.LogToChat) Main.NewText("Opening client.log...");

            try
            {
                string steamPath = GetSteamPath();
                if (string.IsNullOrEmpty(steamPath))
                {
                    if (Conf.LogToChat) Main.NewText("Steam path is null or empty.");
                    Log.Error("Steam path is null or empty.");
                    return;
                }

                string file = Path.Combine(steamPath, "tModLoader-Logs", "client.log");
                Process.Start(new ProcessStartInfo($@"{file}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                if (Conf.LogToChat) Main.NewText("Error opening client.log: " + ex.Message);
                Log.Error("Error opening client.log: " + ex.Message);
            }
        }

        public static void OpenEnabledJson()
        {
            if (Conf.LogToChat) Main.NewText("Opening enabled.json...");

            // Get the path to the enabled.json file in $USERPROFILE$\Documents\My Games\Terraria\tModLoader\Mods
            try
            {
                string file = Path.Combine(ModLoader.ModPath, "enabled.json");
                Process.Start(new ProcessStartInfo($@"{file}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Error("Error opening enabled.json: " + ex.Message);
            }
        }

        public static void OpenEnabledJsonFolder()
        {
            if (Conf.LogToChat) Main.NewText("Opening enabled.json folder...");

            try
            {
                string folder = Path.Combine(ModLoader.ModPath); // gets Documents/My Games/Terraria/tModLoader/Mods
                // go up one directory (to the tModLoader folder)
                folder = Path.GetFullPath(Path.Combine(folder, @"..\"));
                Process.Start(new ProcessStartInfo($@"{folder}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Error("Error opening enabled.json folder: " + ex.Message);
            }
        }
    }
}