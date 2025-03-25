using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
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
                string steamPath = Log.GetSteamPath();
                if (string.IsNullOrEmpty(steamPath))
                {
                    Main.NewText("Steam path is null or empty.");
                    Log.Error("Steam path is null or empty.");
                    return;
                }

                string file = Path.Combine(steamPath, "tModLoader-Logs", "client.log");
                Process.Start(new ProcessStartInfo($@"{file}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Main.NewText("Error opening client.log: " + ex.Message);
                Log.Error("Error opening client.log: " + ex.Message);
            }
        }
    }
}
