using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class RefreshButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText) : BaseButton(buttonImgText, buttonImgNoText, hoverText)
    {
        public override async void LeftClick(UIMouseEvent evt)
        {
            Config c = ModContent.GetInstance<Config>();

            // 1) Kill a server
            if (Main.netMode == NetmodeID.MultiplayerClient && c.Reload.AttemptToKillServer)
            {
                //TODO: Steal PacketHandler from https://github.com/tModLoader/tModLoader/wiki/intermediate-netcode#good-practice-managing-many-packets
                ModPacket killServerPacket = ModContent.GetInstance<SquidTestingMod>().GetPacket();
                killServerPacket.Write(true);
                killServerPacket.Send();
            }

            // 2) Exit world (maybe no longer needed if server is killed but idk)
            ExitWorld(c);

            // 3) Navigate to Develop Mods
            if (c.Reload.WaitingTimeBeforeNavigatingToModSources > 0)
                await Task.Delay(c.Reload.WaitingTimeBeforeNavigatingToModSources);
            object modSourcesInstance = NavigateToDevelopMods();

            // 4) Build and reload
            if (c.Reload.InvokeBuildAndReload)
            {
                await Task.Delay(c.Reload.WaitingTimeBeforeBuildAndReload);
                BuildReload(modSourcesInstance);
            }

            // 5) Autoload player into world. Handled automatically in AutoloadSingleplayerSystem)
            
        }

        public override void RightClick(UIMouseEvent evt)
        {
            // navigate to Main Menu
            WorldGen.JustQuit();
            Log.Info("Navigated to Main Menu.");
            // wait for 2 second
            Task.Delay(2000);
            Main.menuMode = 10000; // mod browser
            // NavigateToManageMods();
        }

        private void NavigateToManageMods()
        {
            try
            {
                Log.Info("Attempting to navigate to Manage Mods...");

                Assembly tModLoaderAssembly = typeof(Main).Assembly;
                Type interfaceType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");

                FieldInfo modBrowserIDField = interfaceType.GetField("modBrowserID", BindingFlags.NonPublic | BindingFlags.Static);
                int modBrowserID = (int)(modBrowserIDField?.GetValue(null) ?? -1);

                modBrowserID = 10007;

                Main.menuMode = modBrowserID != -1 ? modBrowserID : 10007;
                // Main.menuMode = Interface.modNameField;

                Log.Info($"Successfully navigated to Manage Mods (MenuMode: {modBrowserID}).");
            }
            catch (Exception ex)
            {
                Log.Error($"Error navigating to Develop Mods: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private async static void ExitWorld(Config c)
        {
            if (c.Reload.SaveAndQuitWorldWithoutSaving)
            {
                Log.Warn("Just quitting...");
                WorldGen.JustQuit();

            }
            else
            {
                Log.Warn("Saving and quitting...");
                WorldGen.SaveAndQuit();
                await Task.Delay(c.Reload.WaitingTimeBeforeNavigatingToModSources);
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
                            if (modName == c.Reload.ModToReload)
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
                Log.Info("modSourcesID: " + modSourcesID);

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