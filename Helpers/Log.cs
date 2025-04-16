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

        public static string GetSteamPath()
        {
            // get the DLL file from the steam path
            string tMLDLL = Assembly.GetEntryAssembly()?.Location;
            string steamPath = Path.GetDirectoryName(tMLDLL);
            return steamPath;
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

        #region Client log stuff
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

        #endregion
    }
}