using EliteTestingMod.Common.Configs;
using EliteTestingMod.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Diagnostics;
using System.IO;
using Terraria;
using Terraria.UI;

namespace EliteTestingMod.UI.Buttons
{
    public class LaunchButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, float textSize) : BaseButton(spritesheet, buttonText, hoverText, textSize)
    {
        // Set the button icon size
        private float _scale = 0.45f;
        protected override float Scale => _scale;
        protected override int FrameWidth => 100;
        protected override int FrameHeight => 100;

        public override void LeftClick(UIMouseEvent evt)
        {
            try
            {
                string file = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\tModLoader\\start-tModLoader.bat";

                Process.Start(new ProcessStartInfo($@"{file}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Log.Error("Error opening tmodloader: " + ex.Message);
            }

            try
            {
                string steamPath = Log.GetSteamPath();
                if (string.IsNullOrEmpty(steamPath))
                {
                    if (Conf.LogToChat) Main.NewText("Steam path is null or empty.");
                    Log.Error("Steam path is null or empty.");
                    return;
                }

                string file = Path.Combine(steamPath, "start-tModLoader.bat");
                if (Conf.LogToChat) Main.NewText("Opening another client...");
            }
            catch (Exception ex)
            {
                if (Conf.LogToChat) Main.NewText("Error opening another client: " + ex.Message);
                Log.Error("Error opening another client: " + ex.Message);
            }
        }
    }
}