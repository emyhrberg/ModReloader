using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    public class ModEnabledText : UIText
    {
        private bool isEnabled = true;
        private string internalModName;

        public ModEnabledText(string text, string internalModName) : base(text)
        {
            this.internalModName = internalModName;

            // text and size and position
            // enabledText.ShadowColor = new Color(226, 57, 39); // TODO change background color to this, shadowcolor is not it.
            float def = -65f;
            TextColor = Color.Green;
            VAlign = 0.5f;
            Left.Set(def, 1f);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            Log.Info("Clicked on mod: " + internalModName);
            // Toggle state
            bool newState = !isEnabled;
            // Use reflection to invoke the internal method
            var setModEnabled = typeof(ModLoader).GetMethod("SetModEnabled", BindingFlags.NonPublic | BindingFlags.Static);
            if (setModEnabled != null)
            {
                setModEnabled.Invoke(null, new object[] { internalModName, newState });
                Log.Info($"{(newState ? "Enabled" : "Disabled")} {internalModName} via reflection.");
            }
            else
            {
                Log.Error("Could not find SetModEnabled method via reflection.");
            }
            // Update the mods panel's enabled mods list
            var sys = ModContent.GetInstance<MainSystem>();
            var modsPanel = sys.mainState.modsPanel;
            if (newState)
            {
                if (!modsPanel.enabledMods.Contains(internalModName))
                    modsPanel.enabledMods.Add(internalModName);
            }
            else
            {
                modsPanel.enabledMods.Remove(internalModName);
            }
            // Toggle UI state
            isEnabled = newState;
            TextColor = isEnabled ? Color.Green : Color.Red;
            SetText(isEnabled ? "Enabled" : "Disabled");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText(isEnabled ? "Click to disable" : "Click to enable");
            }
        }
    }
}