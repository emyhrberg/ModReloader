using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.PacketHandlers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using static SquidTestingMod.Common.Configs.Config;

namespace SquidTestingMod.Helpers
{
    //All functions, related to reload
    internal class ReloadUtilities
    {
        public static void PrepareClient(ClientMode clientMode)
        {
            ClientDataHandler.Mode = clientMode;
            ClientDataHandler.PlayerId = Utilities.FindPlayerId();
            ClientDataHandler.WorldId = Utilities.FindWorldId();
        }

        public static async Task ReloadOrBuildAndReloadAsync(bool shoudBeBuilded)
        {
            object modSourcesInstance = null;

            // Wait 600 ms before going from menu to mod sources menu
            await Task.Delay(600);

            if (shoudBeBuilded)
            {
                modSourcesInstance = NavigateToDevelopMods();
            }

            if (shoudBeBuilded)
            {
                // Wait 600 ms when at mod sources menu until actually building and reloading
                await Task.Delay(600);
                BuildAndReloadMod(modSourcesInstance);
            }
            else
            {
                ReloadMod();
            }
        }

        public static void ExitWorldOrServer()
        {

            if (Conf.SaveWorldOnReload)
            {
                Log.Warn("Saving and quitting...");
                var tcs = new TaskCompletionSource();

                void Callback()
                {
                    tcs.SetResult();
                }

                WorldGen.SaveAndQuit(Callback);
                return tcs.Task;
            }
            else
            {
                Log.Warn("Just quitting...");
                WorldGen.JustQuit();
                return Task.CompletedTask;
            }

        }

        public static Task ExitAndKillServer()
        {
            ModNetHandler.RefreshServer.SendKillingServer(255, Main.myPlayer, Conf.SaveWorldOnReload);
            var tcs = new TaskCompletionSource();

            void Callback()
            {
                tcs.SetResult();
            }

            WorldGen.SaveAndQuit(Callback);
            return tcs.Task;
        }

        public static void ReloadMod()
        {
            Main.menuMode = 10002;
        }

        public async static Task BuildAndReloadMod()
        {
            object modSourcesInstance = null;
            modSourcesInstance = NavigateToDevelopMods();
            await Task.Delay(600);


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
                            string lc = modName.ToLower();

                            Log.Info($"Mod Name: {modName}");
                            if (modName.Equals(Conf.ModToReload, StringComparison.OrdinalIgnoreCase))
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
