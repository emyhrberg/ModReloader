using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using log4net;
using log4net.Appender;
using Terraria;
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
        /// Log a message once every x second(s)
        /// </summary>
        public static void SlowInfo(string message, int seconds = 1, [CallerFilePath] string callerFilePath = "")
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
            string fileName = Path.GetFileName(Logging.LogPath);
            Main.NewText($"{fileName} cleared.");
        }

        public static void OpenLogFolder()
        {
            Main.NewText("Opening log folder...");

            try
            {
                string steamPath = GetSteamPath();
                if (string.IsNullOrEmpty(steamPath))
                {
                    Main.NewText("Steam path not found.");
                    Error("Steam path not found.");
                    return;
                }

                // Maybe better Alternative:
                // string folder = Logging.LogPath;

                string folder = Path.Combine(GetSteamPath(), "tModLoader-Logs");
                Process.Start(new ProcessStartInfo($@"{folder}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Main.NewText("Error opening log folder: " + ex.Message);
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

        /// <summary>
        /// Opens the client log file in the default text editor for the right player.
        public static void OpenClientLog()
        {
            try
            {
                string path = Logging.LogPath;
                string fileName = Path.GetFileName(path);

                Log.Info($"Open {fileName}");
                Main.NewText("Opening " + fileName);
                Process.Start(new ProcessStartInfo($@"{path}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Main.NewText("Error opening client log: " + ex.Message);
                Log.Error("Error opening client log: " + ex.Message);
            }
        }

        public static void OpenServerLog()
        {
            Main.NewText("Opening server.log");

            try
            {
                string steamPath = GetSteamPath();
                if (string.IsNullOrEmpty(steamPath))
                {
                    Main.NewText("Steam path is null or empty.");
                    Log.Error("Steam path is null or empty.");
                    return;
                }

                string file = Path.Combine(steamPath, "tModLoader-Logs", "server.log");
                Process.Start(new ProcessStartInfo($@"{file}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Main.NewText("Error opening server.log: " + ex.Message);
                Log.Error("Error opening server.log: " + ex.Message);
            }
        }

        // note: unused
        public static void OpenEnabledJson()
        {
            Main.NewText("Opening enabled.json");

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
            Main.NewText("Opening enabled.json folder...");

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