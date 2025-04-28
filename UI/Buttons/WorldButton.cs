using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.UI.Elements;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class WorldButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Spinning globe pixel art
        // protected override float Scale => 0.08f;
        // protected override int FrameWidth => 1080;
        // protected override int FrameHeight => 576;
        // protected override int FrameCount => 11;
        // protected override int FrameSpeed => 7;

        // Old pylon texture, do not delete
        private float _scale = 0.58f;
        protected override float Scale => _scale;
        protected override int FrameCount => 8;
        protected override int FrameSpeed => 7;
        protected override int FrameWidth => 48;
        protected override int FrameHeight => 68;

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);

            // Save the player and world
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            //sys?.mainState?.worldPanel?.savePlayer();
            //sys?.mainState?.worldPanel?.saveWorld();
        }
    }
}