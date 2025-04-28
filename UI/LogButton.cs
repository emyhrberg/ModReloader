using Microsoft.Xna.Framework.Graphics;
using ModHelper.UI.Elements.AbstractElements;
using ReLogic.Content;
using Terraria.UI;

namespace ModHelper.UI
{
    public class LogButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set custom animation dimensions
        private float _scale = 0.5f;
        protected override float Scale => _scale;
        protected override int FrameCount => 16;
        protected override int FrameSpeed => 4;
        protected override int FrameWidth => 74;
        protected override int FrameHeight => 78;

        public override void RightClick(UIMouseEvent evt)
        {
            // Log.OpenClientLog();
        }
    }
}