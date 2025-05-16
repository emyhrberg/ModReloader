using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Helpers;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModReloader.UI.Elements.PanelElements.ModElements
{
    public class ModConfigIcon : UIImage
    {
        private static ModConfigIcon currentlyOpenConfig = null;

        private Asset<Texture2D> tex;
        private string hover;
        public bool isConfigOpen = false;
        public string modName;
        private string cleanModName;

        public ModConfigIcon(Asset<Texture2D> texture, string modPath, string hover = "", string cleanModName = "") : base(texture)
        {
            tex = texture;
            this.hover = hover;
            modName = System.IO.Path.GetFileName(modPath);
            this.cleanModName = cleanModName;

            float size = 23f;
            MaxHeight.Set(size, 0f);
            MaxWidth.Set(size, 0f);
            Width.Set(size, 0f);
            Height.Set(size, 0f);
            VAlign = 1.0f;
            Top.Set(6, 0);
        }

        public void SetStateToClosed()
        {
            hover = $"Open config";
            // hover = $"Open {cleanModName} config";

            tex = Ass.ConfigOpen;
            // Main.NewText("Closing config for " + modName, new Color(226, 57, 39));
            Main.menuMode = 0;
            //Main.InGameUI.SetState(null);
            IngameFancyUI.Close();
            isConfigOpen = false;

            // If this is the currently open config, clear the static reference
            if (currentlyOpenConfig == this)
            {
                currentlyOpenConfig = null;
            }
        }

        public void SetStateToOpen()
        {
            // hover = $"Close {cleanModName} config";
            hover = $"Close config";

            isConfigOpen = true;
            Main.playerInventory = false;
            tex = Ass.ConfigClose;
            currentlyOpenConfig = this;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            SoundEngine.PlaySound(SoundID.MenuClose);

            if (isConfigOpen)
            {
                SetStateToClosed();
                return;
            }

            // Close any other open config
            if (currentlyOpenConfig != null && currentlyOpenConfig != this)
            {
                currentlyOpenConfig.SetStateToClosed();
            }

            try
            {
                // TODO: Draw it above mine.

                // Use reflection to get the private ConfigManager.Configs property.
                FieldInfo configsProp = typeof(ConfigManager).GetField("Configs", BindingFlags.Static | BindingFlags.NonPublic);
                var configs = configsProp.GetValue(null) as IDictionary<Mod, List<ModConfig>>;

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

                // Open the config UI using reflection
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
                // Main.NewText("Opening config for " + modName, Color.Green);

                // Hover text update
                SetStateToOpen();
            }
            catch (Exception ex)
            {
                Main.NewText($"No config found for mod '{modName}'. : {ex.Message}", Color.Red);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Only check for manual closure if we think config is still open
            if (isConfigOpen)
            {
                bool configClosed = false;

                // Check if Main.menuMode has changed from the config mode
                if (Main.menuMode != 10024)
                {
                    configClosed = true;
                }
                // Double-check with the actual UI state
                else if (Main.InGameUI != null)
                {
                    try
                    {
                        var currentStateProp = Main.InGameUI.GetType().GetProperty("CurrentState", BindingFlags.Public | BindingFlags.Instance);
                        if (currentStateProp != null)
                        {
                            var currentState = currentStateProp.GetValue(Main.InGameUI);

                            // If UI state is null or not a config UI
                            if (currentState == null)
                            {
                                configClosed = true;
                            }
                            else
                            {
                                // Get the type of the mod config UI for comparison
                                Assembly assembly = typeof(Main).Assembly;
                                Type interfaceType = assembly.GetType("Terraria.ModLoader.UI.Interface");
                                var modConfigField = interfaceType?.GetField("modConfig", BindingFlags.Static | BindingFlags.NonPublic);

                                if (modConfigField != null)
                                {
                                    var modConfigInstance = modConfigField.GetValue(null);

                                    // If current state is not the mod config UI
                                    if (modConfigInstance != null && currentState.GetType() != modConfigInstance.GetType())
                                    {
                                        configClosed = true;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle any reflection errors
                        Log.Info("Error checking config state: " + ex.Message);
                        configClosed = true;
                    }
                }

                // If we detected the config was closed manually
                if (configClosed)
                {
                    SetStateToClosed();
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            DrawHelper.DrawProperScale(spriteBatch, this, tex.Value, scale: 1.0f);

            if (!string.IsNullOrEmpty(hover) && IsMouseHovering)
            {
                UICommon.TooltipMouseText(hover);
            }
        }
    }
}