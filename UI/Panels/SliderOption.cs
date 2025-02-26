using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    public class SliderOption : PanelElement
    {
        // Variables
        private string HoverText;

        public SliderOption(string text, string hoverText) : base(text)
        {
            HoverText = hoverText;

            // Text element
            textElement.TextColor = Color.DimGray;
            textElement.OnMouseOver += (evt, element) =>
            {
                textElement.TextColor = Color.White;
            };
            textElement.OnMouseOut += (evt, element) => textElement.TextColor = Color.DimGray;
            Append(textElement);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            textElement.TextColor = Color.White;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            textElement.TextColor = Color.DimGray;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Draw hover text
            if (IsMouseHovering && HoverText != "")
            {
                UICommon.TooltipMouseText(HoverText);
            }
        }

        public override bool ContainsPoint(Vector2 point)
        {
            // Get the text element's outer dimensions (which gives us the full width of the text)
            Rectangle textRect = textElement.GetOuterDimensions().ToRectangle();

            // Use the text element's X position and width, but use the OnOffOption's Y position with a fixed height of 30.
            Rectangle clickableArea = new Rectangle(
                textRect.X,                     // X position from the text element
                (int)GetDimensions().Y,         // Y position from the parent (OnOffOption)
                textRect.Width,                 // full width of the text element
                30                              // fixed height (OnOffOption height)
            );

            return clickableArea.Contains(point.ToPoint());
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // handle regular click event like acting on its event
            // e.g PlayerCheats.ToggleGodMode() for GodMode option
            base.LeftClick(evt);

            // handle toggle event
            // set text to replace to either on or off
            UpdateText(textElement.Text.Contains("On") ? textElement.Text.Replace("On", "Off") : textElement.Text.Replace("Off", "On"));
        }
    }
}