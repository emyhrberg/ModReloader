using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ModHelper.Common.Configs;
using ModHelper.UI;
using ModHelper.UI.Buttons;
using ModHelper.UI.Elements;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ModLoader;

namespace ModHelper.Helpers
{
    public static class ReloadUtilities
    {
        // Global list of all the mods that should be built and reloaded when a reload is executed
        public static List<string> ModsToReload = [];

        /// <summary>
        /// Main function to build and reload all the mods in the ModsToReload list.
        /// </summary>
        public static async Task MainReload()
        {
            // Log.Info("mods to reload 2: " + string.Join(", ", ReloadHelper.ModsToReload));

            // Check if list is empty
            if (!CheckIfModsToReloadIsEmpty())
            {
                Main.NewText("No mods to reload.");
                Log.Warn("No mods to reload.");
                return;
            }

            // Another null check for modstoreload, check if modsources contains any mod
            if (!CheckThatModExists())
            {
                Main.NewText("No mods were existing with that/those names. Please check the mod names and try again.");
                Log.Warn("No mods were found to reload.");
                return;
            }

            // Prepare client. Use singleplayer by default for now.
            PrepareClient(clientMode: ClientMode.SinglePlayer);

            // Exit the game.
            await ExitWorld();
            // Mod was found, we can Reload
            BuildAndReloadMods();
        }

        public static void PrepareClient(ClientMode clientMode)
        {
            ClientDataHandler.ClientMode = clientMode;
            ClientDataHandler.PlayerID = Utilities.FindPlayerId();
            ClientDataHandler.WorldID = Utilities.FindWorldId();

            Log.Info("set player and worldid to " + ClientDataHandler.PlayerID + " and " + ClientDataHandler.WorldID);
        }

        private static bool CheckIfModsToReloadIsEmpty()
        {
            if (ModsToReload.Count == 0)
            {
                Main.NewText("No mods to reload.");
                Log.Warn("No mods to reload.");

                MainSystem sys = ModContent.GetInstance<MainSystem>();

                // Open the mods panel.
                List<DraggablePanel> allPanels = sys?.mainState?.AllPanels;

                // replace with THIS panel
                var panel = sys?.mainState?.modsPanel;

                // Disable all other panels
                if (allPanels != null)
                {
                    foreach (var p in allPanels?.Except([panel]))
                    {
                        if (p != panel && p.GetActive())
                        {
                            p?.SetActive(false);
                        }
                    }
                }
                // Set the mods panel active
                panel?.SetActive(true);

                // Set modsbutton parentactive to true, and set the panel active
                List<BaseButton> allButtons = sys?.mainState?.AllButtons;
                var modsButton = allButtons?.FirstOrDefault(b => b is ModsButton);
                if (modsButton != null)
                {
                    modsButton.ParentActive = true;
                }

                // Disable World, Log, UI, Mods buttons
                foreach (var button in sys.mainState.AllButtons)
                {
                    if (button is UIElementButton || button is OptionsButton)
                    {
                        button.ParentActive = false;
                    }
                }
                return false;
            }
            return true;
        }

        private static Task ExitWorld()
        {
            // We cant exit any world if in the main menu, so we just return a completed task
            if (Main.gameMenu)
            {
                return Task.CompletedTask;
            }

            if (Conf.C.SaveWorldBeforeReloading)
            {
                Log.Warn("Saving and quitting...");

                // Creating task that will delay reloading a mod until world finish saving
                var tcs = new TaskCompletionSource();
                WorldGen.SaveAndQuit(tcs.SetResult);
                return tcs.Task;
            }
            else
            {
                Log.Warn("Just quitting...");
                WorldGen.JustQuit();
                return Task.CompletedTask;
            }
        }

        private static bool CheckThatModExists()
        {
            Type modCompileType = typeof(Main).Assembly.GetType("Terraria.ModLoader.Core.ModCompile");
            MethodInfo findModSourcesMethod = modCompileType.GetMethod("FindModSources", BindingFlags.NonPublic | BindingFlags.Static);
            string[] modSources = (string[])findModSourcesMethod.Invoke(null, null);
            string[] modNames = modSources.Select(Path.GetFileName).ToArray();

            foreach (var modName in ModsToReload)
            {
                Log.Info("Reloading mod: " + modName);
                if (modNames.Contains(modName))
                {
                    Log.Info($"Mod '{modName}' found in mod sources.");
                    return true;
                }
            }
            return false;
        }

        public static void BuildAndReloadMods(Action actionAfterBuild = null)
        {
            // 1. Getting Assembly
            Assembly tModLoaderAssembly = typeof(Main).Assembly;

            // 2. Getting method for finding modSources paths
            Type modCompileType = tModLoaderAssembly.GetType("Terraria.ModLoader.Core.ModCompile");
            MethodInfo findModSourcesMethod = modCompileType.GetMethod("FindModSources", BindingFlags.NonPublic | BindingFlags.Static);
            string[] modSources = (string[])findModSourcesMethod.Invoke(null, null);

            // 3. Get all modPaths for future
            Log.Info("Executing Mods to reload: " + string.Join(", ", ModsToReload));

            var modPaths = ModsToReload.Select((modName) =>
                modSources.FirstOrDefault(p =>
                    !string.IsNullOrEmpty(p) &&
                    Directory.Exists(p) &&
                    Path.GetFileName(p)?.Equals(modName, StringComparison.InvariantCultureIgnoreCase) == true));

            // 4. Getting method for reloading a mod
            // 4.1 Getting UIBuildMod Instance
            Type interfaceType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.Interface");
            FieldInfo buildModField = interfaceType.GetField("buildMod", BindingFlags.NonPublic | BindingFlags.Static);
            object buildModInstance = buildModField?.GetValue(null);

            // 4.2 Getting correct BuildMod method of UIBuildMod
            Type uiBuildModType = tModLoaderAssembly.GetType("Terraria.ModLoader.UI.UIBuildMod");
            MethodInfo buildModMethod = uiBuildModType.GetMethod("BuildMod", BindingFlags.Instance | BindingFlags.NonPublic, [typeof(Action<>).MakeGenericType(modCompileType), typeof(bool)]);

            // Check if it exist
            if (buildModMethod == null)
            {
                Log.Warn("No buildMethod were found via reflection.");
                return;
            }

            // 4.3 Getting correct Build method from ModCompile
            MethodInfo mcBuildModFolder = modCompileType.GetMethod("Build", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(string)]);

            // 5. Setting a hook on BuildMod method of UIBuildMod
            Hook buildModMethodHook = null;
            buildModMethodHook = new Hook(buildModMethod, (Func<object, Action<object>, bool, Task> orig, object self, Action<object> buildAction, bool reload) =>
            {
                Task origTask = orig(self, buildAction, reload); // Call original method correctly

                return origTask.ContinueWith(t =>
                {
                    // Check if the task completed successfully or had an exception
                    if (t.IsCompletedSuccessfully)
                    {
                        Log.Info("successfully built mod(s)");
                        // Only continue with reload if build was successful
                        actionAfterBuild?.Invoke();
                    }
                    else
                    {
                        // If there was an error, log it and don't proceed with reload
                        Log.Error("Build failed - halting reload process to show errors");

                        // Optional: You can add code here to ensure the error UI stays visible
                        // For example, setting a flag to prevent automatic world reentry
                        ClientDataHandler.ClientMode = ClientMode.FreshClient;
                        ClientDataHandler.PlayerID = -1;
                        ClientDataHandler.WorldID = -1;
                    }

                    buildModMethodHook?.Dispose(); // Always dispose the hook
                });
            });

            Log.Info("Starting to build mods..." + string.Join(", ", modPaths));

            // 6. Creating a task
            Main.menuMode = 10003;
            Task.Run(() =>
            {
                try
                {
                    return (Task)buildModMethod.Invoke(buildModInstance,
                    [
                        (Action<object>) (mc =>
                                {
                                    foreach (var modPath in modPaths)
                                    {
                                        try
                                        {
                                            Log.Info("Building mod: " + modPath);
                                            mcBuildModFolder.Invoke(mc, [modPath]);
                                        }
                                        catch (Exception buildEx)
                                        {
                                            Log.Error($"Failed to build mod at '{modPath}': {buildEx.Message}");
                                        }
                                    }
                                }),
                                true
                    ]);
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to invoke buildModMethod: {ex.Message}");
                    return Task.CompletedTask;
                }
            });
        }
    }
}
