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
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
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

            ModNetHandler.RefreshServer.SendReloadMP(255, -1, Conf.C.SaveWorldBeforeReloading);


        }

        public static async Task MultiPlayerMajorReload()
        {
            PrepareClient(clientMode: ClientMode.MPMain);

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                //await ExitAndKillServer();

                int clientCount = Main.player.Where((p) => p.active).Count() - 1;
                List<NamedPipeServerStream> clients = new List<NamedPipeServerStream>();
                List<Task<string?>> clientResponses = new List<Task<string?>>();

                Log.Info($"Waiting for {clientCount} clients...");

                // Wait for conecting all clients
                for (int i = 0; i < clientCount; i++)
                {
                    var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.InOut, clientCount);
                    GC.SuppressFinalize(pipeServer);
                    await pipeServer.WaitForConnectionAsync();
                    clients.Add(pipeServer);
                    Log.Info($"Client {i + 1} connected!");
                }

                foreach (var client in clients)
                {
                    clientResponses.Add(ReloadUtilities.ReadPipeMessage(client));
                }

                await Task.WhenAll(clientResponses);

                foreach (var client in clients)
                {
                    client.Close();
                    client.Dispose();
                }

                int worldIdToLoad = ClientDataJsonHelper.WorldID;

                ReloadUtilities.BuildAndReloadMods(() =>
                {
                    //Init logger
                    var logger = LogManager.GetLogger("SQUID");

                    logger.Info("Mod Builded");

                    // After building - reload all other clients
                    List<NamedPipeServerStream> clientsAfterRebuild = new List<NamedPipeServerStream>();
                    logger.Info($"Waiting for {clientCount} clients...");

                    //Creating pipe for each client
                    for (int i = 0; i < clientCount; i++)
                    {
                        var pipeServer = new NamedPipeServerStream(ReloadUtilities.pipeNameAfterRebuild, PipeDirection.InOut, clientCount);
                        GC.SuppressFinalize(pipeServer);
                        pipeServer.WaitForConnection();
                        clientsAfterRebuild.Add(pipeServer);
                        logger.Info($"Client {i + 1} connected after building Yay!");
                    }

                    // Creating hook to unload 

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

                        foreach (var client in clientsAfterRebuild)
                        {
                            client.Close();
                            client.Dispose();
                        }


                        try
                        {
                            Main.LoadWorlds();

                            if (Main.WorldList.Count == 0)
                                throw new Exception("No worlds found.");

                            // Getting Player and World from ClientDataHandler
                            var world = Main.WorldList[worldIdToLoad];

                            string fileNameStartProcess = @"C:\Program Files (x86)\Steam\steamapps\common\tModLoader\start-tModLoaderServer.bat";

                            // create a process
                            ProcessStartInfo process = new(fileNameStartProcess)
                            {
                                UseShellExecute = true,
                                Arguments = $"-nosteam -world {world.Path}"
                            };

                            // start the process
                            Process serverProcess = Process.Start(process);
                            logger.Info("Server process started with ID: " + serverProcess.Id + " and name: " + serverProcess.ProcessName);
                        }
                        catch (Exception e)
                        {
                            // log it
                            logger.Error("Failed to start server!!! C:/Program Files (x86)/Steam/steamapps/common/tModLoader/start-tModLoaderServer.bat" + e.Message);
                        }


                        hookForUnload?.Dispose();
                        return o;
                    });


                });


            }
            else if (Main.netMode == NetmodeID.SinglePlayer)
            {
                await ReloadUtilities.ExitWorld();

                int worldIdToLoad = ClientDataJsonHelper.WorldID;

                Hook hookForUnload = null;

                hookForUnload = new Hook(typeof(ModLoader).GetMethod("Unload", BindingFlags.NonPublic | BindingFlags.Static), (Func<bool> orig) =>
                {
                    var logger = LogManager.GetLogger("SQUID");

                    bool o = orig();

                    try
                    {
                        Main.LoadWorlds();

                        if (Main.WorldList.Count == 0)
                            throw new Exception("No worlds found.");

                        // Getting Player and World from ClientDataHandler
                        var world = Main.WorldList[worldIdToLoad];

                        string fileNameStartProcess = @"C:\Program Files (x86)\Steam\steamapps\common\tModLoader\start-tModLoaderServer.bat";

                        // create a process
                        ProcessStartInfo process = new(fileNameStartProcess)
                        {
                            UseShellExecute = true,
                            Arguments = $"-nosteam -world {world.Path}"
                        };

                        // start the process
                        Process serverProcess = Process.Start(process);
                        logger.Info("Server process started with ID: " + serverProcess.Id + " and name: " + serverProcess.ProcessName);
                    }
                    catch (Exception e)
                    {
                        // log it
                        logger.Error("Failed to start server!!! C:/Program Files (x86)/Steam/steamapps/common/tModLoader/start-tModLoaderServer.bat" + e.Message);
                    }


                    hookForUnload?.Dispose();
                    return o;
                });

                ReloadUtilities.BuildAndReloadMods();
            }

            // Exit the game.
            await ExitWorld();
            // Mod was found, we can Reload
            BuildAndReloadMods();
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
                    

                    using (var pipeClient = new NamedPipeClientStream(".", ReloadUtilities.pipeName, PipeDirection.InOut))
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

            Log.Info("Starting to build mods..." + string.Join(", ", modPaths));

            // 6. Creating a task
            Main.menuMode = 10003;
            Task.Run(() =>
            {
                try
                {
                    return ((Task)buildModMethod.Invoke(buildModInstance,
                    [
                        (Action<object>) (mc =>
                        {
                            foreach (var modPath in modPaths)
                            {
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
                    ])).ContinueWith(t =>
                    {
                        actionAfterBuild?.Invoke(); // Execute custom action after the method finishes
                    });
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException!;
                }

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