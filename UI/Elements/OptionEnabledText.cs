using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using EliteTestingMod.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;
using static EliteTestingMod.UI.Elements.Option;

namespace EliteTestingMod.UI.Elements
{
    public class OptionEnabledText : UIText
    {
        private Color red = new(226, 57, 39);

        private State state;

        public OptionEnabledText(string text) : base(text)
        {
            // text and size and position
            // enabledText.ShadowColor = new Color(226, 57, 39); // TODO change background color to this, shadowcolor is not it.
            TextColor = red;

            // Position: Centered vertically, 65 pixels from the right
            VAlign = 0.5f;
            float def = -65f;
            Left.Set(def, 1f);
        }

        public void SetTextState(State state)
        {
            this.state = state;
            TextColor = state == State.Enabled ? Color.Green : red;
            SetText(state == State.Enabled ? "Enabled" : "Disabled");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                if (state == State.Enabled)
                {
                    UICommon.TooltipMouseText("Click to disable");
                }
                else
                {
                    UICommon.TooltipMouseText("Click to enable");
                }
            }
        }
    }
}