using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using ReLogic.Content;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class ReloadMPButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set custom animation dimensions
        private float _scale = 1.15f;
        protected override int FrameCount => 5;
        protected override int FrameSpeed => 12;
        protected override int FrameWidth => 65;
        protected override int FrameHeight => 65;
        protected override float Scale => _scale;

        public override void LeftClick(UIMouseEvent evt)
        {
            ChatHelper.NewText("ReloadMPButton not implemented yet...");
        }
    }
}
