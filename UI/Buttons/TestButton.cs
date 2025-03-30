using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ReLogic.Content;
using Terraria;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class TestButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // This button is temporary, so there is no image for config
        protected override int FrameWidth => 37;
        protected override int FrameHeight => 15;

        int count = 0;

        public override void LeftClick(UIMouseEvent evt)
        {
            count++;
            Main.NewText("TestButton clicked for the " + count + " time!");
        }
    }
}
