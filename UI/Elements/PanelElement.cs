using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    /// <summary>
    /// Superclass for options inside player, debug, world panel
    /// Contains a text element and some size and positioning only.
    /// </summary>
    public abstract class PanelElement : UIElement
    {
        private int panelPadding = 12;
        public int panelElementHeight = 25;

        // The main text on this panel element
        public float TextScale = 1.0f;
        public UIText textElement;
        public string Title;

        // Hover text for tooltips
        public string HoverText { get; set; }
        public bool IsHoverEnabled { get; set; } = true;

        public PanelElement(string title, string hoverText = "")
        {
            MaxWidth.Set(panelPadding * 2, 1f);
            Width.Set(panelPadding * 2, 1f);
            Height.Set(panelElementHeight, 0f);
            Left.Set(-panelPadding, 0f);

            Title = title;
            HoverText = hoverText;

            // Create the UIText. Default color = Gray
            textElement = new UIText(text: title, textScale: TextScale, large: false);
            textElement.HAlign = 0.5f; // Center horizontally
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

            if (IsHoverEnabled)
            {
                textElement.TextColor = Color.White;
            }
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);

            if (IsHoverEnabled)
            {
                textElement.TextColor = Color.Gray;
            }
        }

        // ------------------------------------------------------------------
        // Drawing the Tooltip on hover
        // ------------------------------------------------------------------
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // If mod config says show tooltips, and we have text for it,
            // and the mouse is actually over the custom hover zone:
            if (!string.IsNullOrEmpty(HoverText) && IsMouseHovering)
            {
                UICommon.TooltipMouseText(HoverText);
            }
        }

        // For convenience, if you need to update the text dynamically
        public void UpdateText(string text)
        {
            textElement.SetText(text);
        }
    }
}
