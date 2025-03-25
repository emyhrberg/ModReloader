using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
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
        public string internalName;
        private ModEnabledText enabledText;
        public ModEnabledIcon modIcon;
        private State state = State.Enabled; // enabled by default

        public State GetState() => state;

        public void SetState(State state)
        {
            this.state = state;
            enabledText.SetTextState(state);
        }

        public ModElement(string modName, string internalModName = "", Texture2D icon = null)
        {
            this.modName = modName;
            this.internalName = internalModName;

            // size and position
            Width.Set(-35f, 1f);
            Height.Set(30, 0);
            Left.Set(5, 0);

            // mod icon

            // passing a temp icon because above doesnt work
            // maybe because path its not loaded yet.
            Texture2D temp = TextureAssets.MagicPixel.Value;

            modIcon = new(temp, internalModName, icon: icon);
            Append(modIcon);

            // mod name
            if (modName.Length > 20)
                modName = string.Concat(modName.AsSpan(0, 20), "...");

            // if icon is not null, it means the mod is not loaded.
            // this is because we send the icon in the constructor for unloaded mods.
            // so we should not allow the user to click on it.
            // so we send no hover option
            if (icon == null)
            {
                // "Enabled Mods"
                OptionTitleText modNameText = new(text: modName, hover: $"Open {modName} config", internalModName: internalModName);
                modNameText.Left.Set(30, 0);
                modNameText.VAlign = 0.5f;
                Append(modNameText);
            }
            else
            {
                // "All Mods"
                OptionTitleText modNameText = new(text: modName, internalModName: internalModName, hover: $"{internalModName}");
                modNameText.Left.Set(30, 0);
                modNameText.VAlign = 0.5f;
                Append(modNameText);
            }

            // enabled text
            enabledText = new(text: "Enabled", internalModName: internalModName);
            Append(enabledText);
        }
    }
}