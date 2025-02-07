using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class RefreshButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText) : BaseButton(buttonImgText, buttonImgNoText, hoverText)
    {
        public override async void HandleClick()
        {
            Config c = ModContent.GetInstance<Config>();

            // 1) Exit world 
            ExitWorld(c);

            // 2) Navigate to Develop Mods
            if (c.WaitingTimeBeforeNavigatingToModSources > 0)
                await Task.Delay(c.WaitingTimeBeforeNavigatingToModSources);
            object modSourcesInstance = NavigateToDevelopMods();

            // 3) Build and reload
            if (c.InvokeBuildAndReload)
            {
                await Task.Delay(c.WaitingTimeBeforeBuildAndReload);
                BuildReload(modSourcesInstance);
            }

            // 4) Autoload player into world. Handled automatically in AutoloadSingleplayerSystem)
        }

        private async static void ExitWorld(Config c)
        {
            if (c.SaveWorld)
            {
                Log.Warn("Saving and quitting...");
                WorldGen.SaveAndQuit();
                await Task.Delay(c.WaitingTimeBeforeNavigatingToModSources);
            }
            else
            {
                Log.Warn("Just quitting...");
                WorldGen.JustQuit();
            }
        }

        private static void BuildReload(object modSourcesInstance)
        {
            if (modSourcesInstance == null)
            {
                Log.Warn("modSourcesInstance is null.");
                return;
            }

            var itemsField = modSourcesInstance.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance);
            if (itemsField == null)
            {
                Log.Warn("_items field not found.");
                return;
            }

            var items = (System.Collections.IEnumerable)itemsField.GetValue(modSourcesInstance);
            if (items == null)
            {
                Log.Warn("_items is null.");
                return;
            }

            object modSourceItem = null;
            string modNameFound = "";

            foreach (var item in items)
            {
                if (item.GetType().Name == "UIModSourceItem")
                {
                    // Extract and log the mod name
                    var modNameField = item.GetType().GetField("_modName", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (modNameField != null)
                    {
                        var modNameValue = modNameField.GetValue(item);
                        if (modNameValue is UIText uiText)
                        {
                            string modName = uiText.Text;
                            Log.Info($"Mod Name: {modName}");
                            Config c = ModContent.GetInstance<Config>();
                            if (modName == c.ModToReload)
                            {
                                modSourceItem = item;
                                modNameFound = modName;
                                break;
                            }
                        }
                        else
                        {
                            Log.Warn("Mod name is not a UIText.");
                        }
                    }
                }
            }

            if (modSourceItem == null)
            {
                Log.Warn("Second UIModSourceItem not found.");
                return;
            }

            var method = modSourceItem.GetType().GetMethod("BuildAndReload", BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null)
            {
                Log.Warn("BuildAndReload method not found.");
                return;
            }

            Log.Info($"Invoking BuildAndReload method with {modNameFound} UIModSourceItem...");
            method.Invoke(modSourceItem, [null, null]);
        }

        private static object NavigateToDevelopMods()
        {
            try
            {
                Log.Info("Attempting to navigate to Develop Mods...");

                Assembly tModLoaderAssembly = typeof(Main).Assembly;
                Type interfaceType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");

                FieldInfo modSourcesField = interfaceType.GetField("modSources", BindingFlags.NonPublic | BindingFlags.Static);
                object modSourcesInstance = modSourcesField?.GetValue(null);

                FieldInfo modSourcesIDField = interfaceType.GetField("modSourcesID", BindingFlags.NonPublic | BindingFlags.Static);
                int modSourcesID = (int)(modSourcesIDField?.GetValue(null) ?? -1);

                Main.menuMode = modSourcesID;

                Log.Info($"Successfully navigated to Develop Mods (MenuMode: {modSourcesID}).");

                return modSourcesInstance;
            }
            catch (Exception ex)
            {
                Log.Error($"Error navigating to Develop Mods: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }
    }
}