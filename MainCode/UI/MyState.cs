using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.UI;
using Terraria.Audio;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Reflection;
using System;
using System.Threading.Tasks;
using SkipSelect.MainCode.Other;
using Microsoft.Xna.Framework;

namespace SkipSelect.MainCode.UI
{
    public class MyState : UIState
    {

        // Variables
        private bool Visible;

        public override void OnInitialize()
        {
            Config config = ModContent.GetInstance<Config>();
            if (config.EnableRefresh)
            {
                Asset<Texture2D> buttonRefreshTexture = ModContent.Request<Texture2D>("SkipSelect/MainCode/Assets/ButtonRefresh");
                MyHoverButton buttonRefresh = new(buttonRefreshTexture, "Refresh (Go to Develop Mods)");
                buttonRefresh.Width.Set(100f, 0f);
                buttonRefresh.Height.Set(100f, 0f);
                buttonRefresh.Top.Set(10f, 0f);
                buttonRefresh.HAlign = 0.5f;
                buttonRefresh.OnLeftClick += CloseButtonClicked;
                Append(buttonRefresh);
            }
        }

        private async void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            // init config
            Config config = ModContent.GetInstance<Config>();
            int wait = config.WaitingTime;
            int w = 1000;
            SoundEngine.PlaySound(SoundID.MenuClose);
            ModContent.GetInstance<SkipSelect>().Logger.Warn("Saving and quitting...");

            // 1) Save and quit
            WorldGen.JustQuit();

            // 2) Navigate to Develop Mods
            object inst = NavigateToDevelopMods();
            await Task.Delay(w);

            // 3) Interact with Develop Mods menu
            InteractWithModMenu(inst);
        }

        private void InteractWithModMenu(object modSourcesInstance)
        {
            try
            {
                ModContent.GetInstance<SkipSelect>().Logger.Warn("Inspecting Develop Mods menu...");

                // Check if modSourcesInstance is valid
                if (modSourcesInstance == null)
                {
                    ModContent.GetInstance<SkipSelect>().Logger.Warn("modSources instance is null.");
                    return;
                }

                Type modSourcesType = modSourcesInstance.GetType();

                // Try accessing the `_items` field
                FieldInfo itemsField = modSourcesType.GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
                object items = itemsField?.GetValue(modSourcesInstance);

                //if (items != null && items is System.Collections.IEnumerable itemList)
                //{
                //    ModContent.GetInstance<SkipSelect>().Logger.Warn("Found _items. Iterating over menu items...");

                //    foreach (var item in itemList)
                //    {
                //        ModContent.GetInstance<SkipSelect>().Logger.Warn($"Found menu item: {item.GetType().Name}");

                //        // debug print all items
                //        DebugModSources(modSourcesInstance);
                //        DebugInterfaceMembers();

                //        // Check if the item represents the "Build+Reload" button
                //        Type itemType = item.GetType();
                //        if (itemType.Name.Contains("Build+Reload", StringComparison.OrdinalIgnoreCase))
                //        {
                //            MethodInfo clickMethod = itemType.GetMethod("Click", BindingFlags.Public | BindingFlags.Instance);
                //            if (clickMethod != null)
                //            {
                //                ModContent.GetInstance<SkipSelect>().Logger.Warn("Attempting to click Build+Reload...");
                //                clickMethod.Invoke(item, new object[] { null, null }); // Simulate a click
                //                ModContent.GetInstance<SkipSelect>().Logger.Warn("Successfully clicked Build+Reload.");
                //                return; // Exit after clicking
                //            }
                //        }
                //    }
                //}
                if (2 == 3)
                {

                }
                else
                {
                    ModContent.GetInstance<SkipSelect>().Logger.Warn("_items is not a valid enumerable. Checking _modList...");

                    // Try accessing the `_modList` field
                    FieldInfo modListField = modSourcesType.GetField("_modList", BindingFlags.NonPublic | BindingFlags.Instance);
                    object modList = modListField?.GetValue(modSourcesInstance);

                    if (modList != null)
                    {
                        ModContent.GetInstance<SkipSelect>().Logger.Warn($"_modList found. Type: {modList.GetType().Name}");

                        // Access items in _modList if possible
                        MethodInfo getItemsMethod = modList.GetType().GetMethod("GetItems", BindingFlags.Public | BindingFlags.Instance);
                        var modListItems = getItemsMethod?.Invoke(modList, null) as System.Collections.IEnumerable;

                        DebugModSources(modSourcesInstance);
                        DebugInterfaceMembers();

                        if (modListItems != null)
                        {
                            foreach (var modItem in modListItems)
                            {
                                ModContent.GetInstance<SkipSelect>().Logger.Warn($"Found mod item: {modItem.GetType().Name}");

                                DebugModSources(modSourcesInstance);
                                DebugInterfaceMembers();

                                // Check for the "Build+Reload" button in mod items
                                Type modItemType = modItem.GetType();
                                if (modItemType.Name.Contains("Build+Reload", StringComparison.OrdinalIgnoreCase))
                                {
                                    MethodInfo clickMethod = modItemType.GetMethod("Click", BindingFlags.Public | BindingFlags.Instance);
                                    if (clickMethod != null)
                                    {
                                        ModContent.GetInstance<SkipSelect>().Logger.Warn("Attempting to click Build+Reload...");
                                        clickMethod.Invoke(modItem, new object[] { null, null }); // Simulate a click
                                        ModContent.GetInstance<SkipSelect>().Logger.Warn("Successfully clicked Build+Reload.");
                                        return; // Exit after clicking
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        ModContent.GetInstance<SkipSelect>().Logger.Warn("_modList is null or not accessible.");
                    }
                }
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<SkipSelect>().Logger.Error($"Error interacting with Develop Mods menu: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void DebugModSources(object modSourcesInstance)
        {
            ModContent.GetInstance<SkipSelect>().Logger.Warn("---- Debug Mod Sources -------------------");

            if (modSourcesInstance == null)
            {
                ModContent.GetInstance<SkipSelect>().Logger.Warn("modSources instance is null.");
                return;
            }

            Type modSourcesType = modSourcesInstance.GetType();

            // Log all fields
            foreach (var field in modSourcesType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                ModContent.GetInstance<SkipSelect>().Logger.Warn($"Field: {field.Name} ({field.FieldType.Name})");
            }

            // Log all properties
            foreach (var property in modSourcesType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                ModContent.GetInstance<SkipSelect>().Logger.Warn($"Property: {property.Name} ({property.PropertyType.Name})");
            }
        }

        private Object NavigateToDevelopMods()
        {
            try
            {
                ModContent.GetInstance<SkipSelect>().Logger.Warn("Attempting to navigate to Develop Mods...");

                // Access the Interface type
                Assembly tModLoaderAssembly = typeof(Main).Assembly;
                Type interfaceType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");

                if (interfaceType == null)
                {
                    ModContent.GetInstance<SkipSelect>().Logger.Warn("Interface class not found.");
                    return null;
                }

                // Get the modSources instance (Develop Mods menu)
                FieldInfo modSourcesField = interfaceType.GetField("modSources", BindingFlags.NonPublic | BindingFlags.Static);
                object modSourcesInstance = modSourcesField?.GetValue(null);

                if (modSourcesInstance == null)
                {
                    ModContent.GetInstance<SkipSelect>().Logger.Warn("modSources instance not found.");
                    return null;
                }

                // Get the modSourcesID
                FieldInfo modSourcesIDField = interfaceType.GetField("modSourcesID", BindingFlags.NonPublic | BindingFlags.Static);
                int modSourcesID = (int)(modSourcesIDField?.GetValue(null) ?? -1);

                if (modSourcesID == -1)
                {
                    ModContent.GetInstance<SkipSelect>().Logger.Warn("modSourcesID not found.");
                    return null;
                }

                // Set Main.menuMode to modSourcesID
                Main.menuMode = modSourcesID;

                ModContent.GetInstance<SkipSelect>().Logger.Warn($"Successfully navigated to Develop Mods (MenuMode: {modSourcesID}).");

                return modSourcesInstance;
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<SkipSelect>().Logger.Error($"Error navigating to Develop Mods: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        private void DebugInterfaceMembers()
        {
            ModContent.GetInstance<SkipSelect>().Logger.Warn("---- Debug Interface Members -------------------");

            Assembly tModLoaderAssembly = typeof(Main).Assembly;
            Type interfaceType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");

            if (interfaceType == null)
            {
                ModContent.GetInstance<SkipSelect>().Logger.Warn("Interface class not found.");
                return;
            }

            // Log all fields
            foreach (var field in interfaceType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                ModContent.GetInstance<SkipSelect>().Logger.Warn($"Field: {field.Name} ({field.FieldType.Name})");
            }

            // Log all properties
            foreach (var property in interfaceType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                ModContent.GetInstance<SkipSelect>().Logger.Warn($"Property: {property.Name} ({property.PropertyType.Name})");
            }
        }
    }
}
