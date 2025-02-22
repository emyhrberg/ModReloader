using log4net.Appender;
using log4net;
using SquidTestingMod.Common.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using System.IO;

namespace SquidTestingMod.Helpers
{
    //Class basically for universal helping functions
    internal class Utilities
    {
        public static int FindPlayerId()
        {
            Main.LoadPlayers();
            var playerId = Main.PlayerList.FindIndex(p => p.Path == Main.ActivePlayerFileData.Path);
            return playerId;
        }

        public static int FindWorldId()
        {
            Main.LoadWorlds();
            var worldId = Main.WorldList.FindIndex(w => w.Path == Main.ActiveWorldFileData.Path);
            return worldId;
        }

        public static void ClearClientLog()
        {
            Log.Info("Clearing client logs....");
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
    }
}
