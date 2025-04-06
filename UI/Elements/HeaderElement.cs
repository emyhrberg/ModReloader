using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    public class HeaderElement : PanelElement
    {
        private string hover;
        private Action leftClickAction;

        public HeaderElement(string title, string hover = "", Color color = default, float HAlign = 0.5f, Action leftClick = null) : base(title)
        {
            this.hover = hover;
            IsHoverEnabled = false;

            if (color == default)
            {
                color = Color.White;
            }
            textElement.TextColor = color;
            textElement.HAlign = HAlign;
            leftClickAction = leftClick;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            leftClickAction?.Invoke();
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