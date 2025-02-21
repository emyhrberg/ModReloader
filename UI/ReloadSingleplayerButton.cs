using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ReloadSingleplayerButton(Asset<Texture2D> _image, string hoverText) : BaseButton(_image, hoverText)
    {
        public override void LeftClick(UIMouseEvent evt)
        {
            Config c = ModContent.GetInstance<Config>();

            // 1 Clear logs if needed
            if (c.ClearClientLogOnReload)
                ClearClientLog();

            if (c.SaveWorldOnReload)
            {
                Log.Info("Saving world and quitting...");
                WorldGen.SaveAndQuit(() =>
                {
                    // 2 Navigate to mod sources menu
                    NavigateToModSourcesMenu();
                });
            }
            else
            {
                WorldGen.JustQuit();
                NavigateToModSourcesMenu();
            }

        }

        private void NavigateToModSourcesMenu()
        {
            Main.menuMode = 10001;
        }

        private void ClearClientLog()
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