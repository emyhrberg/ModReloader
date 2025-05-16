using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModReloader.UI.Elements.PanelElements
{
    public class OptionTitleText : UIText
    {
        public string hover = "";

        private Action leftClick;
        private Action rightClick;

        public OptionTitleText(string text, string hover = "", float textSize = 1f,
        Action leftClick = null, Action rightClick = null) : base(text, textSize)
        {
            this.hover = hover;
            Left.Set(0, 0);
            VAlign = 0.5f;

            this.leftClick = leftClick;
            this.rightClick = rightClick;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            leftClick?.Invoke();
        }

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);
            rightClick?.Invoke();
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