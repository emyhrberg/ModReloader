using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
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

            // remove from the list
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            ModsPanel modsPanel = sys.mainState.modsPanel;

            Log.Info("Clicked on internal: " + internalModName);

            for (int i = modsPanel.enabledMods.Count - 1; i >= 0; i--)
            {
                string modNameEnabledMod = modsPanel.enabledMods[i];
                Log.Info("Checking " + modNameEnabledMod);

                if (modNameEnabledMod == internalModName)
                {
                    Log.Info("Removing " + modNameEnabledMod);
                    modsPanel.enabledMods.RemoveAt(i);

                    // Edit enabled.json and remove the mod from the list and save the file
                    string file = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Documents\\My Games\\Terraria\\tModLoader\\Mods\\enabled.json";

                    if (File.Exists(file))
                    {
                        string json = File.ReadAllText(file);

                        List<string> mods = JsonSerializer.Deserialize<List<string>>(json);
                        mods.Remove(internalModName);

                        string updatedJson = JsonSerializer.Serialize(mods, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(file, updatedJson);

                        Log.Info($"Removed {internalModName} from the list successfully!");
                    }
                }
            }

            // toggle text
            isEnabled = !isEnabled;
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