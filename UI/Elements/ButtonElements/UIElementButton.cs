using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Systems;
using ReLogic.Content;
using Terraria.UI;

namespace ModReloader.UI.Elements.ButtonElements

{
    public class UIElementButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Sprite size
        protected override float BaseAnimScale => 1.3f;
        protected override int FrameWidth => 28;
        protected override int FrameHeight => 24;

        // Sprite animation
        protected override int FrameCount => 4;
        protected override int FrameSpeed => 10;

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);

            // Toggle all elements
            UIElementSystem elementSystem = ModContent.GetInstance<UIElementSystem>();
            UIElementState elementState = elementSystem.debugState;
            elementState.ToggleShowAll();
        }
    }
}