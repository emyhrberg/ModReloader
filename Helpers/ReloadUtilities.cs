using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using ModHelper.Common.Configs;
using ModHelper.PacketHandlers;
using ModHelper.UI;
using ModHelper.UI.Buttons;
using ModHelper.UI.Elements;
using MonoMod.RuntimeDetour;
using ReLogic.OS;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.UI;
using Terraria.Social;
namespace ModHelper.Helpers
{
    public static class ReloadUtilities
    {
        // Global list of all the mods that should be built and reloaded when a reload is executed
        public static List<string> ModsToReload = [];

        private const string pipeName = "ModHelperReloadPipe";

        private const string pipeNameAfterRebuild = "ModHelperReloadPipeAfterRebuild";

        /// <summary>
        /// Main function to build and reload all the mods in the ModsToReload list.
        /// </summary>
        public static async Task SinglePlayerReload()
        {
            // Log.Info("mods to reload 2: " + string.Join(", ", ReloadHelper.ModsToReload));

            // Check if list is empty
            if (!CheckIfModsToReloadIsEmpty())
            {
                Main.NewText("No mods selected, please select mods to reload");
                Log.Warn("No mods selected, please select mods to reload");
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

        public static async Task MultiPlayerMainReload()
        {
            // Log.Info("mods to reload 2: " + string.Join(", ", ReloadHelper.ModsToReload));

            // Check if list is empty
            if (!CheckIfModsToReloadIsEmpty())
            {
                Main.NewText("No mods selected, please select mods to reload");
                Log.Warn("No mods selected, please select mods to reload");
                return;
            }

            // Another null check for modstoreload, check if modsources contains any mod
            if (!CheckThatModExists())
            {
                Main.NewText("No mods were existing with that/those names. Please check the mod names and try again.");
                Log.Warn("No mods were found to reload.");
                return;
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                PrepareClient(clientMode: ClientMode.MPMajor);

                // Exit the game.
                await ExitWorld();


                CreateServerAfterUnloadHook(ClientDataJsonHelper.WorldID);

                // Mod was found, we can Reload
                BuildAndReloadMods();
            }
            else
            {
                // Prepare client. Use singleplayer by default for now.
                ModNetHandler.RefreshServer.SendReloadMP(255, -1, Conf.C.SaveWorldBeforeReloading);
            }
        }

        public static async Task MultiPlayerMajorReload()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                PrepareClient(clientMode: ClientMode.MPMajor);
            
                // Exit the game.
                await ExitWorld();

                // Kill the server process
                Main.tServer?.Kill();

                // Get how many clients are connected
                int clientCount = Main.player.Where((p) => p.active).Count() - 1;

                // Create a list of pipes for clients
                List<NamedPipeServerStream> clients = new List<NamedPipeServerStream>();
                List<Task<string?>> clientResponses = new List<Task<string?>>();

                Log.Info($"Waiting for {clientCount} clients...");

                // Wait for conecting all clients
                for (int i = 0; i < clientCount; i++)
                {
                    // Creating pipe for each client
                    var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut, clientCount);
                    GC.SuppressFinalize(pipeServer);

                    // Wait for connection
                    await pipeServer.WaitForConnectionAsync();
                    clients.Add(pipeServer);
                    Log.Info($"Client {i + 1} connected!");
                }


                // Wait for all clients to finish unloading
                foreach (var client in clients)
                {
                    clientResponses.Add(ReadPipeMessage(client));
                }

                await Task.WhenAll(clientResponses);

                // Dispose of all clients
                foreach (var client in clients)
                {
                    client.Close();
                    client.Dispose();
                }

                BuildAndReloadMods(() =>
                {
                    Log.Info("Mod Builded");

                    // After building - reload all other clients
                    List<NamedPipeServerStream> clientsAfterRebuild = new List<NamedPipeServerStream>();
                    Log.Info($"Waiting for {clientCount} clients...");

                    // Wait for conecting all clients
                    for (int i = 0; i < clientCount; i++)
                    {
                        // Creating pipe for each client
                        var pipeServer = new NamedPipeServerStream(ReloadUtilities.pipeNameAfterRebuild, PipeDirection.InOut, clientCount);
                        GC.SuppressFinalize(pipeServer);

                        // Wait for connection
                        pipeServer.WaitForConnection();
                        clientsAfterRebuild.Add(pipeServer);
                        Log.Info($"Client {i + 1} connected after building Yay!");
                    }

                    foreach (var client in clientsAfterRebuild)
                    {
                        client.Close();
                        client.Dispose();
                    }

                    // Creating hook for Unload 
                    CreateServerAfterUnloadHook(ClientDataJsonHelper.WorldID);
                });
            }
        }

        private static void CreateServerAfterUnloadHook(int worldIdToLoad)
        {
            Hook hookForUnload = null;

            hookForUnload = new Hook(typeof(ModLoader).GetMethod("Unload", BindingFlags.NonPublic | BindingFlags.Static), (Func<bool> orig) =>
            {
                var logger = LogManager.GetLogger("SQUID");

                bool o = orig();

                /*
                var modOrganizerType = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.Core.ModOrganizer");
                var findModsMethod = modOrganizerType.GetMethod("FindMods", BindingFlags.NonPublic | BindingFlags.Static);

                if (findModsMethod != null)
                {
                    var localMods = findModsMethod.Invoke(null, new object[] { true });

                    if (localMods is IEnumerable modsEnumerable)
                    {
                        var targetMod = modsEnumerable.Cast<object>()
                            .FirstOrDefault(mod =>
                            {
                                Type modType = mod.GetType();
                                var nameProperty = modType.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
                                return nameProperty?.GetValue(mod)?.ToString() == "ModHelper";
                            });

                        if (targetMod != null)
                        {
                            // Get `modFile` field from `LocalMod`
                            Type localModType = targetMod.GetType();
                            var modFileField = localModType.GetField("modFile", BindingFlags.Public | BindingFlags.Instance);
                            var modFileInstance = modFileField?.GetValue(targetMod);

                            if (modFileInstance != null)
                            {
                                // Get `Hash` property from `TmodFile`
                                Type tmodFileType = modFileInstance.GetType();
                                var hashProperty = tmodFileType.GetProperty("Hash", BindingFlags.Public | BindingFlags.Instance);
                                var hashValue = hashProperty?.GetValue(modFileInstance) as byte[];

                                if (hashValue != null)
                                {
                                    logger.Info("Hash: " + BitConverter.ToString(hashValue));
                                    foreach (var client in clientsAfterRebuild)
                                    {
                                        using BinaryWriter writer = new BinaryWriter(client);
                                        writer.Write(hashValue[0]);
                                    }
                                }
                                else
                                {
                                    logger.Info("Hash property not found or is null.");
                                }
                            }
                            else
                            {
                                logger.Info("modFile is null.");
                            }
                        }
                        else
                        {
                            logger.Info("Mod not found.");
                        }
                    }
                }*/

                try
                {
                    Main.LoadWorlds();

                    if (Main.WorldList.Count == 0)
                        throw new Exception("No worlds found.");

                    // Getting Player and World from ClientDataHandler
                    var world = Main.WorldList[worldIdToLoad];

                    string text = "-autoshutdown -password \"" + Main.ConvertToSafeArgument(Netplay.ServerPassword) + "\" -lang " + Language.ActiveCulture.LegacyId;
                    if (Platform.IsLinux)
                        text = ((IntPtr.Size != 8) ? (text + " -x86") : (text + " -x64"));

                    text = ((!Main.ActiveWorldFileData.IsCloudSave) ? (text + Main.instance.SanitizePathArgument("world", Main.worldPathName)) : (text + Main.instance.SanitizePathArgument("cloudworld", Main.worldPathName)));
                    text = text + " -worldrollbackstokeep " + Main.WorldRollingBackupsCountToKeep;

                    // TML options
                    text += $@" -modpath ""{ModOrganizer.modPath}""";

                    if (Program.LaunchParameters.TryGetValue("-tmlsavedirectory", out var tmlsavedirectory))
                        text += $@" -tmlsavedirectory ""{tmlsavedirectory}""";
                    else if (Program.LaunchParameters.TryGetValue("-savedirectory", out var savedirectory))
                        text += $@" -savedirectory ""{savedirectory}""";

                    if (Main.showServerConsole)
                        text += " -showserverconsole";

                    Main.tServer = new Process();

                    Main.tServer.StartInfo.FileName = Environment.ProcessPath;
                    Main.tServer.StartInfo.Arguments = "tModLoader.dll -server " + text;
                    if (Main.libPath != "")
                    {
                        ProcessStartInfo startInfo = Main.tServer.StartInfo;
                        startInfo.Arguments = startInfo.Arguments + " -loadlib " + Main.libPath;
                    }

                    Main.tServer.StartInfo.UseShellExecute = true;
                    if (!Main.showServerConsole)
                        Main.tServer.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    if (SocialAPI.Network != null)
                        SocialAPI.Network.LaunchLocalServer(Main.tServer, Main.MenuServerMode);
                    else
                        Main.tServer.Start();
                }
                catch (Exception e)
                {
                    // log it
                    logger.Error("Failed to start server!!! C:/Program Files (x86)/Steam/steamapps/common/tModLoader/start-tModLoaderServer.bat" + e.Message);
                }

                hookForUnload?.Dispose();
                return o;
            });
        }

        public static async Task MultiPlayerMinorReload()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                PrepareClient(ClientMode.MPMinor);

                await ExitWorld();

                Hook hookForUnload = null;

                hookForUnload = new Hook(typeof(ModLoader).GetMethod("Unload", BindingFlags.NonPublic | BindingFlags.Static), (Func<bool> orig) =>
                {
                    var logger = LogManager.GetLogger("SQUID");

                    bool o = orig();

                    object loadMods = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.Interface").GetField("loadMods", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

                    //UILoadMods loadMods = Terraria.ModLoader.UI.Interface.loadMods;

                    typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.UILoadMods").GetMethod("SetProgressText", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(loadMods, ["Waiting for main client", "Waiting for main client"]);


                    using (var pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut))
                    {
                        logger.Info($"Waiting for main client");
                        pipeClient.Connect();

                        using var writer = new StreamWriter(pipeClient) { AutoFlush = true };
                        writer.WriteLine("Im here and ready to reload!");
                    }

                    logger.Info("Wait to continue loading");

                    using var pipeClientafterRebuild = new NamedPipeClientStream(".", ReloadUtilities.pipeNameAfterRebuild, PipeDirection.InOut);
                    pipeClientafterRebuild.Connect();
                    /*
                    using BinaryReader reader = new BinaryReader(pipeClientafterRebuild);
                    int number = reader.ReadByte();
                    logger.Info($"Number from hash: {number}");
                    */
                    logger.Info("Clearing modsDirCache");

                    var cache = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.Core.ModOrganizer").GetField("modsDirCache", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

                    if (cache is IDictionary dictionary)
                    {
                        dictionary.Clear(); // Clears the dictionary without needing LocalMod type
                        Console.WriteLine("Cache cleared successfully.");
                    }

                    logger.Info("Loading mods");

                    hookForUnload?.Dispose();

                    return o;
                });

                //stops GC from deleting it
                GC.SuppressFinalize(hookForUnload);

                ReloadMod();
            }
        }

        private static void PrepareClient(ClientMode clientMode)
        {
            ClientDataJsonHelper.ClientMode = clientMode;
            ClientDataJsonHelper.PlayerID = Utilities.FindPlayerId();
            ClientDataJsonHelper.WorldID = Utilities.FindWorldId();
            Log.Info("set player and worldid to " + ClientDataJsonHelper.PlayerID + " and " + ClientDataJsonHelper.WorldID);
        }

        /// <summary> A little messy function to close all other panels and open the mods panel. </summary>
        private static bool CheckIfModsToReloadIsEmpty()
        {
            if (ModsToReload.Count == 0)
            {
                MainSystem sys = ModContent.GetInstance<MainSystem>();

                // Open the mods panel.
                List<DraggablePanel> allPanels = sys?.mainState?.AllPanels;

                // replace with THIS panel
                var modSourcesPanel = sys?.mainState?.modSourcesPanel;

                // Disable all other panels
                // if (!Conf.C.AllowMultiplePanelsOpenSimultaneously)
                // {
                if (allPanels != null)
                {
                    foreach (var p in allPanels?.Except([modSourcesPanel]))
                    {
                        if (p != modSourcesPanel && p.GetActive())
                        {
                            p?.SetActive(false);
                        }
                    }
                }
                // }

                // Set the mods panel active
                modSourcesPanel?.SetActive(true);

                // Set modsbutton parentactive to true, and set the panel active
                List<BaseButton> allButtons = sys?.mainState?.AllButtons;
                var modsButton = allButtons?.FirstOrDefault(b => b is ModSourcesButton);
                if (modsButton != null)
                {
                    modsButton.ParentActive = true;
                }

                // Disable World, Log, UI, Mods buttons
                // if (!Conf.C.AllowMultiplePanelsOpenSimultaneously)
                // {
                foreach (var button in sys.mainState.AllButtons)
                {
                    if (button is ModsButton)
                    {
                        button.ParentActive = false;
                    }
                }
                // }

                return false;
            }
            return true;
        }

        private static void ReloadMod()
        {
            // Going to reload mod menu(that automaticly invokes reload)
            Main.menuMode = 10002;
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

        private static void BuildAndReloadMods(Action actionAfterBuild = null)
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
                    Path.GetFileName(p)?.Equals(modName, StringComparison.InvariantCultureIgnoreCase) == true)).ToList();

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
                            for (int i = 0; i < modPaths.Count; i++)
                            {
                                string modPath = modPaths[i];
                                Log.Info("Building mod: " + modPath);
                                try
                                {
                                    mcBuildModFolder.Invoke(mc, [modPath]);
                                }
                                catch (TargetInvocationException ex)
                                {
                                    throw ex.InnerException!;
                                }
                            }
                        }),
                        true
                    ]);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException!;
                }

            }).ContinueWith(t =>
            {
                actionAfterBuild?.Invoke(); // Execute custom action after the method finishes
            });
        }

        private static Task ExitAndKillServer()
        {
            // Sending packet to server to inform about reloading mod in multiplayer
            ModNetHandler.RefreshServer.SendReloadMP(255, Main.myPlayer, Conf.C.SaveWorldBeforeReloading);

            // idk if that needed for exiting server, but maybe we need to save player data idk
            var tcs = new TaskCompletionSource();
            WorldGen.SaveAndQuit(tcs.SetResult);
            return tcs.Task;
        }

        private static async Task<string?> ReadPipeMessage(NamedPipeServerStream pipe)
        {
            using StreamReader reader = new StreamReader(pipe);
            return await reader.ReadLineAsync();
        }
    }
}