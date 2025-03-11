using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class ReloadButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText) : BaseButton(spritesheet, buttonText, hoverText)
    {
        // Check if SP or MP button is toggled
        public enum ReloadButtonMode
        {
            SP,
            MP
        }

        public ReloadButtonMode Mode = ReloadButtonMode.SP;

        /// <summary>
        /// <see cref="ReloadSPButton"/> 
        /// <see cref="ReloadMPButton"/> 
        /// </summary>
        /// <param name="evt"></param>
        public override void LeftClick(UIMouseEvent evt)
        {
            // Toggle the SP/MP mode if left click and holding/pressing either of these 3 keys:
            // - LeftAlt
            // - Shift
            // - Control
            if (Main.keyState.IsKeyDown(Keys.LeftAlt) || Main.keyState.PressingShift() || Main.keyState.PressingControl())
            {
                Mode = Mode == ReloadButtonMode.SP ? ReloadButtonMode.MP : ReloadButtonMode.SP;
                Log.Info($"ReloadButton: Mode toggled to {Mode}");
                return;
            }
        }
    }
}