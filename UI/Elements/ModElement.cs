using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace SquidTestingMod.UI.Elements
{
    // Contains:
    // Icon image
    // Mod name
    // Enabled text
    public class ModElement : UIPanel
    {
        public string modName;
        private ModEnabledText enabledText;
        public ModEnabledIcon modIcon;

        public ModElement(string modName, string internalName)
        {
            this.modName = modName;

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
            UIText modNameText = new(modName);
            modNameText.Left.Set(30, 0);
            modNameText.VAlign = 0.5f;
            Append(modNameText);

            // enabled text
            enabledText = new("Enabled", internalName);
            Append(enabledText);
        }
    }
}