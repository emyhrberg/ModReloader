using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    public class ModEnabledIcon : UIImage
    {
        private string internalModName;
        public Texture2D updatedTex;
        private Action leftClick;

        public bool IsHovered => IsMouseHovering;

        public ModEnabledIcon(Texture2D tex, string internalModName, Action leftClick = null) : base(tex)
        {
            this.leftClick = leftClick;
            this.internalModName = internalModName;

            float size = 25f;
            MaxHeight.Set(size, 0f);
            MaxWidth.Set(size, 0f);
            Width.Set(size, 0f);
            Height.Set(size, 0f);
            VAlign = 0.5f;

            // custom top
            Top.Set(-1, 0);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            leftClick?.Invoke();

            // Temp try to open config
            try
            {
                // Use reflection to get the private ConfigManager.Configs property.
                FieldInfo configsProp = typeof(ConfigManager).GetField("Configs", BindingFlags.Static | BindingFlags.NonPublic);
                var configs = configsProp.GetValue(null) as IDictionary<Mod, List<ModConfig>>;

                // Get the mod name from the modPath.
                // string modName = Path.GetFileName(modPath);
                string modName = internalModName;
                Mod modInstance = ModLoader.GetMod(modName);
                if (modInstance == null)
                {
                    Main.NewText($"Mod '{modName}' not found.", Color.Red);
                    return;
                }

                // Check if there are any configs for this mod.
                if (!configs.TryGetValue(modInstance, out List<ModConfig> modConfigs) || modConfigs.Count == 0)
                {
                    Main.NewText("No config available for mod: " + modName, Color.Yellow);
                    return;
                }

                // Use the first available config.
                ModConfig config = modConfigs[0];

                // Open the config UI.
                // Use reflection to set the mod and config for the modConfig UI.
                Assembly assembly = typeof(Main).Assembly;
                Type interfaceType = assembly.GetType("Terraria.ModLoader.UI.Interface");
                var modConfigField = interfaceType.GetField("modConfig", BindingFlags.Static | BindingFlags.NonPublic);
                var modConfigInstance = modConfigField.GetValue(null);
                var setModMethod = modConfigInstance.GetType().GetMethod("SetMod", BindingFlags.Instance | BindingFlags.NonPublic);

                // Invoke the SetMod method to set the mod and config for the modConfig UI.
                setModMethod.Invoke(modConfigInstance, [modInstance, config, false, null, null, true]);

                // Open the mod config UI.
                Main.InGameUI.SetState(modConfigInstance as UIState);
                Main.menuMode = 10024;
                Main.NewText("Opening config for " + modName, Color.Yellow);
            }
            catch (Exception ex)
            {
                Log.Error("Error opening mod config: " + ex.Message);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            string path = $"{internalModName}/icon";

            updatedTex = ModContent.Request<Texture2D>(path).Value;

            if (updatedTex != null)
            {
                DrawHelper.DrawProperScale(spriteBatch, this, updatedTex);
            }
        }
    }
}