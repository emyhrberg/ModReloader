using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class LogButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText) : BaseButton(spritesheet, buttonText, hoverText)
    {
        protected override int MaxFrames => 3;
        protected override int FrameSpeed => 8;
        protected override int FrameWidth => 38;
        protected override int FrameHeight => 48;

        public override void LeftClick(UIMouseEvent evt)
        {
            Conf.ConfigInstance.Open();
        }
    }
}