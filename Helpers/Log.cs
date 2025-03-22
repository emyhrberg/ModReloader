using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Appender;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Helpers
{
    public static class Log
    {
        private static Mod ModInstance => ModContent.GetInstance<SquidTestingMod>();

        /// <summary>
        /// Log a message once every second
        /// </summary>
        public static void SlowInfo(string message)
        {
            if (Main.GameUpdateCount % 60 * 5 == 0)
            {
                ModInstance.Logger.Info(message);
            }
        }

        public static void Info(string message) => ModInstance.Logger.Info(message);

        public static void Warn(string message) => ModInstance.Logger.Warn(message);

        public static void Error(string message) => ModInstance.Logger.Error(message);

        public static void ClearClientLog()
        {
            Info("Clearing client logs....");
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
        }

        public static void OpenLogFolder()
        {
            Main.NewText("Opening log folder...");

            try
            {
                string steamPath = GetSteamPath();
                if (string.IsNullOrEmpty(steamPath))
                {
                    Error("Steam path not found.");
                    return;
                }

                string folder = Path.Combine(GetSteamPath(), "tModLoader-Logs");
                Process.Start(new ProcessStartInfo($@"{folder}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Error("Error opening log folder: " + ex.Message);
            }
        }

        private static string GetSteamPath()
        {
            // get the DLL file from the steam path
            string tMLDLL = Assembly.GetEntryAssembly()?.Location;
            string steamPath = Path.GetDirectoryName(tMLDLL);
            return steamPath;
        }

        public static void OpenClientLog()
        {
            Main.NewText("Opening client.log...");

            try
            {
                string steamPath = GetSteamPath();
                if (string.IsNullOrEmpty(steamPath))
                {
                    Error("Steam path not found.");
                    return;
                }

                string file = Path.Combine(GetSteamPath(), "tModLoader-Logs", "client.log");
                Process.Start(new ProcessStartInfo($@"{file}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Error("Error opening client.log: " + ex.Message);
            }
        }

        public static void OpenEnabledJson()
        {
            Main.NewText("Opening enabled.json...");

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
                string folder = Path.Combine(ModLoader.ModPath);
                Process.Start(new ProcessStartInfo($@"{folder}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Error("Error opening enabled.json folder: " + ex.Message);
            }
        }
    }
}