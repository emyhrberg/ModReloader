using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class StartGameButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, float textSize) : BaseButton(spritesheet, buttonText, hoverText, textSize)
    {
        // Set the button icon size
        protected override int FrameWidth => 100;
        protected override int FrameHeight => 100;
        protected override float Scale => 0.5f;

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
        }
    }
}