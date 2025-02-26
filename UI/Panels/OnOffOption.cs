using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    public class OnOffOption : PanelElement
    {
        // Variables
        private string HoverText;
        private bool hover = false;

        public OnOffOption(string text, string hoverText) : base(text)
        {
            HoverText = hoverText;

            // Text element
            textElement.TextColor = Color.DimGray;
            textElement.OnMouseOver += (evt, element) =>
            {
                textElement.TextColor = Color.White;
                hover = true;
                Log.Info("hovering on text element");
            };
            textElement.OnMouseOut += (evt, element) =>
            {
                textElement.TextColor = Color.DimGray;
                hover = false;
            };
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
            if (C.ShowTooltipsDebugPanels && hover)
            {
                UICommon.TooltipMouseText(HoverText);
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // handle regular base click event like acting on its event
            // e.g PlayerCheats.ToggleGodMode() for GodMode option
            base.LeftClick(evt);

            // handle toggle event
            // set text to replace to either on or off
            UpdateText(textElement.Text.Contains("On") ? textElement.Text.Replace("On", "Off") : textElement.Text.Replace("Off", "On"));
        }
    }
}