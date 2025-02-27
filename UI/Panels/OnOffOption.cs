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

        public OnOffOption(string text, string hoverText) : base(text, hoverText)
        {
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