using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using SquidTestingMod.src;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class RefreshButton : BaseButton
    {
        public RefreshButton(Asset<Texture2D> texture, string hoverText)
            : base(texture, hoverText)
        {
        }

        public async void HandleClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Config config = ModContent.GetInstance<Config>();

            if (config.SaveWorld)
            {
                Log.Warn("Saving and quitting...");
                WorldGen.SaveAndQuit();
                await Task.Delay(config.WaitingTime);
            }
            else
            {
                Log.Warn("Just quitting...");
                WorldGen.JustQuit();
            }

            object modSourcesInstance = NavigateToDevelopMods();

            if (config.InvokeBuildAndReload)
            {
                await Task.Delay(config.WaitingTime);
                BuildReload(modSourcesInstance);
            }
        }

        private void BuildReload(object modSourcesInstance)
        {
            var items = (System.Collections.IEnumerable)modSourcesInstance
                .GetType()
                .GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(modSourcesInstance);

            object modSourceItem = null;
            int modSourceItemCount = 0;

            foreach (var item in items)
            {
                // Log the name of each item
                Log.Info($"Item: {item.GetType().Name}");

                if (item.GetType().Name == "UIModSourceItem")
                {
                    modSourceItemCount++;
                    if (modSourceItemCount == 2)
                    {
                        modSourceItem = item;
                    }

                    // Extract and log the mod name
                    var modNameField = item.GetType().GetField("_modName", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (modNameField != null)
                    {
                        var modNameValue = modNameField.GetValue(item);
                        if (modNameValue is UIText uiText)
                        {
                            string modName = uiText.Text;
                            Log.Info($"Mod Name: {modName}");
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

            var method = modSourceItem.GetType()
                .GetMethod("BuildAndReload", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(modSourceItem, [null, null]);
        }

        private object NavigateToDevelopMods()
        {
            try
            {
                Log.Warn("Attempting to navigate to Develop Mods...");

                Assembly tModLoaderAssembly = typeof(Main).Assembly;
                Type interfaceType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");

                FieldInfo modSourcesField = interfaceType.GetField("modSources", BindingFlags.NonPublic | BindingFlags.Static);
                object modSourcesInstance = modSourcesField?.GetValue(null);

                FieldInfo modSourcesIDField = interfaceType.GetField("modSourcesID", BindingFlags.NonPublic | BindingFlags.Static);
                int modSourcesID = (int)(modSourcesIDField?.GetValue(null) ?? -1);

                Main.menuMode = modSourcesID;

                Log.Warn($"Successfully navigated to Develop Mods (MenuMode: {modSourcesID}).");

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