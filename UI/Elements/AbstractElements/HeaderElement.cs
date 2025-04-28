using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.UI;

namespace ModHelper.UI.Elements.AbstractElements
{
    public class HeaderElement : PanelElement
    {
        private string hover;

        public HeaderElement(string title, string hover = "") : base(title)
        {
            this.hover = hover;
            IsHoverEnabled = false;
            textElement.TextColor = Color.White;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (!string.IsNullOrEmpty(hover) && IsMouseHovering)
            {
                UICommon.TooltipMouseText(hover);
            }
        }
    }
}