using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using static SquidTestingMod.UI.Elements.Option;

namespace SquidTestingMod.UI.Elements
{
    // Contains:
    // Icon image
    // Mod name
    // Enabled text
    public class ModElement : UIPanel
    {
        public string modName;
        private string internalName;
        private ModEnabledText enabledText;
        public ModEnabledIcon modIcon;
        private State state = State.Enabled;

        public void SetState(State state)
        {
            this.state = state;
            enabledText.SetTextState(state);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (modIcon.IsMouseHovering)
                return;

            base.LeftClick(evt);

            // Toggle state
            state = state == State.Enabled ? State.Disabled : State.Enabled;
            enabledText.SetTextState(state);
            bool enabled = state == State.Enabled;

            Log.Info("Setting mod enabled: " + internalName + " to " + enabled);

            // Use reflection to call SetModEnabled on internalModName
            var setModEnabled = typeof(ModLoader).GetMethod("SetModEnabled", BindingFlags.NonPublic | BindingFlags.Static);
            setModEnabled?.Invoke(null, [internalName, enabled]);
        }

        public ModElement(string modName, string internalName)
        {
            this.modName = modName;
            this.internalName = internalName;

            // size and position
            Width.Set(-35f, 1f);
            Height.Set(30, 0);
            Left.Set(5, 0);

            // mod icon
            // string path = $"{internalName}/icon";
            // Asset<Texture2D> ass = ModContent.Request<Texture2D>(path);

            // passing a temp icon because above doesnt work
            // maybe because path its not loaded yet.
            Texture2D temp = TextureAssets.MagicPixel.Value;

            modIcon = new(temp, internalName);
            Append(modIcon);

            // mod name
            if (modName.Length > 20)
                modName = string.Concat(modName.AsSpan(0, 20), "...");
            OptionTitleText modNameText = new(text: modName, hover: internalName);
            modNameText.Left.Set(30, 0);
            modNameText.VAlign = 0.5f;
            Append(modNameText);

            // enabled text
            enabledText = new(text: "Enabled");
            Append(enabledText);
        }
    }
}