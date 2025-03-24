using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    public class OptionTitleText : UIText
    {
        public string hover = "";
        private Action leftClick;
        private string internalModName = "";

        private bool isConfigOpen = false;

        public OptionTitleText(string text, string hover = "", Action leftClick = null, float textSize = 1f, string internalModName = "") : base(text, textSize)
        {
            this.hover = hover;
            this.internalModName = internalModName;
            //Left.Set(30, 0);
            Left.Set(0, 0);
            VAlign = 0.5f;
            this.leftClick = leftClick;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (string.IsNullOrEmpty(internalModName))
            {
                return;
            }

            // if hover is empty, the button is disabled.
            if (string.IsNullOrEmpty(hover))
            {
                return;
            }

            base.LeftClick(evt);

            leftClick?.Invoke();

            if (isConfigOpen)
            {
                hover = $"Open {internalModName} config";
                Main.NewText("Closing config for " + internalModName, new Color(226, 57, 39));
                Main.menuMode = 0;
                Main.InGameUI.SetState(null);
                isConfigOpen = false;

                // Expand the hotbar
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                if (sys != null)
                {
                    sys.mainState.collapse.SetCollapsed(false);
                }

                return;
            }

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

                // Hover text update
                hover = $"Close {internalModName} config";

                // Collapse the hotbar
                isConfigOpen = true;

                Main.playerInventory = false;
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                sys?.mainState?.collapse?.SetCollapsed(true);
                sys.mainState.AreButtonsShowing = false;
                sys.mainState.collapse.UpdateCollapseImage();
            }
            catch (Exception ex)
            {
                Main.NewText($"No config found for mod '{internalModName}'. : {ex.Message}", Color.Red);
                return;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (!string.IsNullOrEmpty(hover) && IsMouseHovering)
            {
                UICommon.TooltipMouseText(hover);
            }
        }
    }
}