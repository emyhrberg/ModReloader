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
                var modsButton = allButtons?.FirstOrDefault(b => b is ModSourcesButton);
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

        public static void BuildAndReloadMods()
        {
            // Instead of trying to call ModCompile.Build directly, we should use the Interface.buildMod that's already set up
            Type interfaceType = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.Interface");
            if (interfaceType == null)
            {
                Log.Error("Could not find Interface type");
                return;
            }

            // Get the buildMod field
            FieldInfo buildModField = interfaceType.GetField("buildMod", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (buildModField == null)
            {
                Log.Error("Could not find buildMod field");
                return;
            }

            // Get the UIBuildMod instance
            object buildModInstance = buildModField.GetValue(null);
            if (buildModInstance == null)
            {
                Log.Error("buildMod instance is null");
                return;
            }

            Type buildModType = buildModInstance.GetType();

            // Find the Build method that takes a string and a boolean
            MethodInfo buildMethod = buildModType.GetMethod("Build",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new[] { typeof(string), typeof(bool) },
                null);

            if (buildMethod == null)
            {
                Log.Error("Could not find Build(string, bool) method");
                return;
            }

            // Set ClientDataHandler values to default to prevent auto-reload if errors occur
            Log.Info("Mods to reload: " + string.Join(", ", ModsToReload));
            foreach (var modName in ModsToReload)
            {
                string fullPath = Path.Combine(Main.SavePath, "ModSources", modName);
                try
                {
                    Log.Info($"Building mod: {fullPath}");
                    // Call the Build method with reload=true
                    buildMethod.Invoke(buildModInstance, [fullPath, true]);

                    // This line is reached only if no exception was thrown
                    Log.Info("Successfully built mod: " + fullPath);
                }
                catch (Exception ex)
                {
                    // If any exception occurs, reset ClientDataHandler to prevent auto-reload
                    Log.Error($"Error building mod {modName}: {ex.Message}");
                    ClientDataHandler.ClientMode = ClientMode.FreshClient;
                    ClientDataHandler.PlayerID = -1;
                    ClientDataHandler.WorldID = -1;
                }
            }
        }
    }
}