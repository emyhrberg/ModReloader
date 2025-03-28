using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ReLogic.Content;
using Terraria;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class LaunchButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set the button icon size
        private float _scale = 0.45f;
        protected override float Scale => _scale;
        protected override int FrameWidth => 100;
        protected override int FrameHeight => 100;

        public override void LeftClick(UIMouseEvent evt)
        {
            bool openLocalFileSuccess = false;

            try
            {
                string file = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\tModLoader\\start-tModLoader.bat";
                if (File.Exists(file))
                {
                    ChatHelper.NewText("Opening tmodloader...");
                    openLocalFileSuccess = true;
                    Process.Start(new ProcessStartInfo($@"{file}") { UseShellExecute = true });
                }
                else
                {
                    ChatHelper.NewText("tmodloader not found in C drive. Retrying...");
                    Log.Error("tmodloader not found in C drive. Retrying...");
                    return;
                }

            }
            catch (Exception ex)
            {
                Log.Error("Error opening tmodloader: " + ex.Message);
            }

            if (!openLocalFileSuccess)
            {
                try
                {
                    string steamPath = Log.GetSteamPath();
                    if (string.IsNullOrEmpty(steamPath))
                    {
                        ChatHelper.NewText("Steam path is null or empty.");
                        Log.Error("Steam path is null or empty.");
                        return;
                    }

                    string file = Path.Combine(steamPath, "start-tModLoader.bat");
                    ChatHelper.NewText("Opening another client...");

                    Process.Start(new ProcessStartInfo($@"{file}") { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    ChatHelper.NewText("Error opening another client: " + ex.Message);
                    Log.Error("Error opening another client: " + ex.Message);
                }
            }
        }
    }
}