using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace ModReloader.UI.Elements.PanelElements.ModElements
{
    public class ModToggleAllPanel : UIPanel
    {
        private readonly string hoverText;
        private readonly Action clickAction;

        public ModToggleAllPanel(string text, Color color, string hover = "", Action onClick = null)
        {
            // Store parameters
            hoverText = hover;
            clickAction = onClick;

            // w = 100, h = 25, halign  = 0.86
            Width.Set(100, 0f);
            Height.Set(25, 0f);
            HAlign = 0.88f; // right aligned

            // Background color (slightly transparent dark gray)
            // BackgroundColor = new Color(25, 25, 25, 200);
            // BorderColor = new Color(70, 70, 70, 255);

            // Add centered text
            UIText label = new UIText(text, 1.0f, false)
            {
                Width = { Pixels = 0 },
                Height = { Pixels = 0 },
                HAlign = 0.5f,
                VAlign = 0.5f,
                TextColor = color,
            };
            Append(label);

            // Make panel clickable if a click action was provided
            if (clickAction != null)
            {
                OnLeftClick += (evt, element) => clickAction();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Show hover text if provided
            if (!string.IsNullOrEmpty(hoverText) && IsMouseHovering)
            {
                UICommon.TooltipMouseText(hoverText);
            }
        }
    }
}