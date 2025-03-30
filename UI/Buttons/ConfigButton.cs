using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ReLogic.Content;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class ConfigButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // This button is temporary, so there is no image for config

        protected override float Scale => 0.2f;
        protected override int FrameCount => 5;
        protected override int FrameSpeed => 10;
        protected override int FrameWidth => 640;
        protected override int FrameHeight => 640;

        public override void LeftClick(UIMouseEvent evt)
        {
            Conf.C.Open();
        }
    }
}
