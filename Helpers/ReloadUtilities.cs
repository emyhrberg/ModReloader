using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ModHelper.Common.Configs;
using ModHelper.PacketHandlers;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.ID;

namespace ModHelper.Helpers
{
    // All functions, related to reload
    internal class ReloadUtilities
    {
        public const string pipeName = "ModHelperPipe";
        public const string pipeNameAfterRebuild = "ModHelperPipeAfterRebuild";

        //public static NamedPipeServerStream Pipe {  get; set; }\

        public static void PrepareClient(ClientModes clientMode)
        {
            ClientDataHandler.ClientMode = clientMode;
            ClientDataHandler.PlayerID = Utilities.FindPlayerId();
            ClientDataHandler.WorldID = Utilities.FindWorldId();
        }

        /// <summary> Returns a list of mod sources found via reflection. </summary>
        private static List<string> FindModSourcesFolderPaths()
        {
            // Find mod sources via reflection
            Assembly assembly = typeof(Main).Assembly;
            Type modCompileType = assembly.GetType("Terraria.ModLoader.Core.ModCompile");
            MethodInfo findModSourcesMethod = modCompileType.GetMethod("FindModSources", BindingFlags.NonPublic | BindingFlags.Static);
            string[] modSources = (string[])findModSourcesMethod.Invoke(null, null);

            // Check if modSources is null or empty.
            return modSources.ToList();
        }

        /// <summary> Main function to reload mod. </summary>
        public static async Task Reload()
        {
            // 1: Null / error checking. 
            // Ensure that the config option is a mod that exists in mod sources.
            List<string> modSources = FindModSourcesFolderPaths();
            List<string> modNames = modSources.Select(Path.GetFileName).ToList();
            if (!modNames.Contains(Conf.C.ModToReload, StringComparer.InvariantCultureIgnoreCase))
            {
                Log.Warn($"{Conf.C.ModToReload}' not found in mod sources. Please enter a valid mod name in the config.");
                ChatHelper.NewText($"Mod To Reload '{Conf.C.ModToReload}' not found in mod sources. Valid options are:");
                for (int i = 1; i < modNames.Count; i++)
                {
                    ChatHelper.NewText($"{modNames[i]}");
                }
                return;
            }

            // 2: Prepare client data
            PrepareClient(ClientModes.SinglePlayer);

            // 3 Exit world
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                await ExitWorld();
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                await ExitAndKillServer();
            }

            // 4 Reload
            BuildAndReloadMods();
        }

        public static Task ExitWorld()
        {
            if (Conf.C.SaveWorldOnReload)
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

        public static Task ExitAndKillServer()
        {
            // Sending packet to server to inform about reloading mod in multiplayer
            ModNetHandler.RefreshServer.SendKillingServer(255, Main.myPlayer, Conf.C.SaveWorldOnReload);

            // idk if that needed for exiting server, but maybe we need to save player data idk
            var tcs = new TaskCompletionSource();
            WorldGen.SaveAndQuit(tcs.SetResult);
            return tcs.Task;
        }

        public static void ReloadMod()
        {
            // Going to reload mod menu(that automaticly invokes reload)
            Main.menuMode = 10002;
        }

        public static void BuildAndReloadMods(Action actionAfterBuild = null)
        {
            Assembly assembly = typeof(Main).Assembly;

            // Getting method for finding modSources paths
            Type modCompileType = assembly.GetType("Terraria.ModLoader.Core.ModCompile");
            MethodInfo findModSourcesMethod = modCompileType.GetMethod("FindModSources", BindingFlags.NonPublic | BindingFlags.Static);
            string[] modSources = (string[])findModSourcesMethod.Invoke(null, null);

            // Get all modPaths for future
            Log.Info("Executing Mods to reload: " + string.Join(", ", ModsToReload.modsToReload));

            var modPaths = ModsToReload.modsToReload.Select((modName) =>
                modSources.FirstOrDefault(p =>
                    !string.IsNullOrEmpty(p) &&
                    Directory.Exists(p) &&
                    Path.GetFileName(p)?.Equals(modName, StringComparison.InvariantCultureIgnoreCase) == true));

            // 4. Getting method for reloading a mod
            // 4.1 Getting UIBuildMod Instance
            Type interfaceType = assembly.GetType("Terraria.ModLoader.UI.Interface");
            FieldInfo buildModField = interfaceType.GetField("buildMod", BindingFlags.NonPublic | BindingFlags.Static);
            object buildModInstance = buildModField?.GetValue(null);

            // 4.2 Getting correct BuildMod method of UIBuildMod
            Type uiBuildModType = assembly.GetType("Terraria.ModLoader.UI.UIBuildMod");
            MethodInfo buildModMethod = uiBuildModType.GetMethod("BuildMod", BindingFlags.Instance | BindingFlags.NonPublic, [typeof(Action<>).MakeGenericType(modCompileType), typeof(bool)]);

            // 4.3 Getting correct Build method from ModCompile
            MethodInfo mcBuildModFolder = modCompileType.GetMethod("Build", BindingFlags.NonPublic | BindingFlags.Instance, [typeof(string)]);

            // 5. Setting a hook on BuildMod method of UIBuildMod
            Hook buildModMethodHook = null;
            buildModMethodHook = new Hook(buildModMethod, (Func<object, Action<object>, bool, Task> orig, object self, Action<object> buildAction, bool reload) =>
            {
                Task origTask = orig(self, buildAction, reload); // Call original method correctly

                return origTask.ContinueWith(t =>
                {
                    actionAfterBuild?.Invoke(); // Execute custom action after the method finishes
                    buildModMethodHook?.Dispose(); // Disable hook
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
                                if (string.IsNullOrWhiteSpace(modPath))
                                {
                                    Log.Error("Encountered empty or null modPath. Skipping.");
                                    continue;
                                }
                                try
                                {
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

        // string can be replaced with json if needed
        // for me the fact of sending messages would be enough
        public static async Task<string?> ReadPipeMessage(NamedPipeServerStream pipe)
        {
            using StreamReader reader = new StreamReader(pipe);
            return await reader.ReadLineAsync();
        }
    }
}
