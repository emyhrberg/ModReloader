using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class ConfigButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText) : BaseButton(spritesheet, buttonText, hoverText)
    {
        protected override float SpriteScale => 0.2f;
        protected override int MaxFrames => 5;
        protected override int FrameSpeed => 10;
        protected override int FrameWidth => 640;
        protected override int FrameHeight => 640;

        public override void LeftClick(UIMouseEvent evt)
        {
            C.ConfigInstance.Open();
        }
    }
}