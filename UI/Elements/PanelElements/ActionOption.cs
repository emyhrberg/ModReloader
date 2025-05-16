using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModReloader.UI.Elements.PanelElements
{
    public class ActionOption : UIElement
    {
        // The main text on this panel element
        public UIText textElement;
        public string hover { get; set; }

        private Action leftClick;
        private Action rightClick;

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

        public ActionOption(Action leftClick, string text, string hover, Action rightClick = null, float textSize = 0.4f)
        {
            //Size and position
            int panelPadding = 12;
            int panelElementHeight = 25;
            MaxWidth.Set(panelPadding * 2, 1f);
            Width.Set(panelPadding * 2, 1f);
            Height.Set(panelElementHeight, 0f);
            Left.Set(panelPadding, 0f);

            this.hover = hover;
            this.leftClick = leftClick;
            this.rightClick = rightClick;

            // Create the UIText. Default color = Gray
            textElement = new(text: text, textScale: textSize, large: true);
            // textElement.HAlign = 0.5f; // Center horizontally
            textElement.VAlign = 0.5f; // Center vertically
            textElement.TextColor = Color.Gray;
            Append(textElement);
        }

        public override bool ContainsPoint(Vector2 point)
        {
            CalculatedStyle panelDims = GetDimensions();
            CalculatedStyle textDims = textElement.GetDimensions();

            Rectangle hoverRect = new Rectangle(
                (int)textDims.X,
                (int)panelDims.Y,
                (int)textDims.Width,
                (int)panelDims.Height
            );

            return hoverRect.Contains(point.ToPoint());
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            textElement.TextColor = Color.White;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            textElement.TextColor = Color.Gray;
        }

        // ------------------------------------------------------------------
        // Drawing the Tooltip on hover
        // ------------------------------------------------------------------
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // If mod config says show tooltips, and we have text for it,
            // and the mouse is actually over the custom hover zone:
            if (!string.IsNullOrEmpty(hover) && IsMouseHovering)
            {
                UICommon.TooltipMouseText(hover);
            }
        }
    }
}