using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using ModReloader.Common.Configs;
using ModReloader.PacketHandlers;
using MonoMod.RuntimeDetour;
using ReLogic.OS;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.UI;
using Terraria.Social;
namespace ModReloader.Helpers
{
    public static class ReloadUtilities
    {
        private static int clientsCountInServer;

        private const string pipeNameBeforeRebuild = "ModReloaderReloadPipeBeforeRebuild";

        private const string pipeNameAfterRebuild = "ModReloaderReloadPipeAfterRebuild";

        public static bool IsModsToReloadEmpty => Conf.C.ModsToReload.Count <= 0;

        /// <summary>
        /// Forces the reload to just reload the mods without building them again.
        /// </summary>
        public static bool forceJustReload = false;

        /// <summary>
        /// Main function to build and reload all the mods in the ModsToReload list for Singleplayer.
        /// </summary>
        public static async Task SinglePlayerReload()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                // Prepare client. Use singleplayer by default for now.
                PrepareClient(clientMode: ClientMode.SinglePlayer);

                // Exit the game.
                await ExitWorld();

                // Mod was found, we can Reload
                BuildOrReloadMods();
                return;
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                clientsCountInServer = Main.player.Where((p) => p.active).Count() - 1;

                // Check if there are any clients connected to the server
                if (clientsCountInServer > 0)
                {
                    Main.NewText($"{clientsCountInServer} additional client(s) is connected. Can't proceed with SP reload");
                    Log.Warn($"{clientsCountInServer} additional client(s) is connected. Can't proceed with SP reload");
                    return;
                }
                else
                {
                    // Prepare client.
                    PrepareClient(clientMode: ClientMode.SinglePlayer);

                    // Kill the server process (just in case)
                    Main.tServer?.WaitForExit();

                    // Exit the game.
                    await ExitWorld();

                    // Build or reload mods
                    BuildOrReloadMods();
                    return;
                }
            }
        }

        /// <summary>
        /// Main function to build and reload all the mods in the ModsToReload list for Multiplayer.<br/>
        /// It sends a packet to the server to inform about reloading mod in multiplayer.
        /// </summary>
        /// <returns></returns>
        public static async Task MultiPlayerMainReload()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                PrepareClient(clientMode: ClientMode.MPMajor);

                // Exit the game.
                await ExitWorld();

                // Create a hook for Unload to create a server after unloading
                CreateServerBeforeUnloadHook(ClientDataJsonHelper.WorldID);

                // Mod was found, we can Reload
                BuildOrReloadMods();
                return;
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                clientsCountInServer = Main.player.Where((p) => p.active).Count() - 1;

                // Sending packet to server to reload server and all clients
                ModNetHandler.RefreshServer.SendReloadMP(255, -1, Conf.C.SaveWorldBeforeReloading, IsModsToReloadEmpty);
            }
        }

        /// <summary>
        /// Function to build and reload all the mods in the ModsToReload list for MajorMP client.<br/>
        /// It waits for all clients to finish unloading and then rebuilds the mods.<br/>
        /// After rebuilding, it allows all clients to reload the mods.
        /// </summary>
        /// <returns></returns>
        public static async Task MultiPlayerMajorReload(int serverPID, int serverWorldID)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // Prepare client data
                PrepareClient(ClientMode.MPMajor, Utilities.FindPlayerId(), serverWorldID);

                // Exit the game.
                await ExitWorld();

                // Creating hook for Unload 
                CreateServerBeforeUnloadHook(ClientDataJsonHelper.WorldID);

                if (!IsModsToReloadEmpty)
                {
                    // Create a list of pipes for clients
                    List<NamedPipeServerStream> clients = new List<NamedPipeServerStream>();
                    List<Task<string>> clientResponses = new List<Task<string>>();

                    Log.Info($"Waiting for {clientsCountInServer} clients...");

                    // Wait for conecting all clients
                    for (int i = 0; i < clientsCountInServer; i++)
                    {
                        // Creating pipe for each client
                        var pipeServer = new NamedPipeServerStream(pipeNameBeforeRebuild, PipeDirection.InOut, clientsCountInServer);
                        GC.SuppressFinalize(pipeServer);

                        // Wait for connection
                        pipeServer.WaitForConnection();
                        clients.Add(pipeServer);
                        Log.Info($"Client {i + 1} connected!");
                    }

                    // Dispose of all clients
                    foreach (var client in clients)
                    {
                        client.Close();
                        client.Dispose();
                    }

                    // Wait for the server to exit
                    try
                    {
                        Log.Info("Waiting for server to exit...");
                        Process.GetProcessById(serverPID)?.WaitForExit();
                    }
                    catch (ArgumentException)
                    {
                        Log.Warn($"Server process with PID {serverPID} not found.\n" +
                            $"Maybe already exited");
                    }
                    catch (InvalidOperationException)
                    {
                        Log.Warn($"Server process with PID {serverPID} is not running.");
                    }
                    finally
                    {
                        BuildAndReloadMods(() =>
                        {
                            Log.Info("Mod Builded");

                            // After building - reload all other clients
                            List<NamedPipeServerStream> clientsAfterRebuild = new List<NamedPipeServerStream>();
                            Log.Info($"Waiting for {clientsCountInServer} clients...");

                            // Wait for conecting all clients
                            for (int i = 0; i < clientsCountInServer; i++)
                            {
                                // Creating pipe for each client
                                var pipeServer = new NamedPipeServerStream(pipeNameAfterRebuild, PipeDirection.InOut, clientsCountInServer);
                                GC.SuppressFinalize(pipeServer);

                                // Wait for connection
                                pipeServer.WaitForConnection();
                                clientsAfterRebuild.Add(pipeServer);
                                Log.Info($"Client {i + 1} connected after building!");
                            }

                            foreach (var client in clientsAfterRebuild)
                            {
                                client.Close();
                                client.Dispose();
                            }
                        });
                    }
                }
                else
                {
                    ReloadMods();
                }
            }
        }

        /// <summary>
        /// Function to build and reload all the mods in the ModsToReload list for MinorMP client.<br/>
        /// It starts to reload the mods and then waits for the MajorMP client to finish unloading and then rebuilds the mods.<br/>
        /// </summary>
        /// <returns></returns>
        public static async Task MultiPlayerMinorReload(bool onlyReload)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // Prepare client data
                PrepareClient(ClientMode.MPMinor);

                // Exit the game.
                await ExitWorld();

                // Kill the server process (just in case)
                Main.tServer?.WaitForExit();

                if (!onlyReload)
                {
                    // Creating a hook for Unload to wait for the major client to rebuild mod(s)
                    Hook hookForUnload = null;
                    hookForUnload = new Hook(typeof(ModLoader).GetMethod("Unload", BindingFlags.NonPublic | BindingFlags.Static), (Func<bool> orig) =>
                    {
                        var logger = LogManager.GetLogger("MODRELOADER_UNLOAD");

                        bool o = orig();

                        object loadMods = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.Interface").GetField("loadMods", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

                        //UILoadMods loadMods = Terraria.ModLoader.UI.Interface.loadMods;

                        typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.UILoadMods").GetMethod("SetProgressText", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(loadMods, ["Waiting for main client", "Waiting for main client"]);


                        using (var pipeClient = new NamedPipeClientStream(".", pipeNameBeforeRebuild, PipeDirection.InOut))
                        {
                            logger.Info($"Waiting for main client");
                            pipeClient.Connect();
                        }

                        logger.Info("Wait to continue loading");

                        using var pipeClientafterRebuild = new NamedPipeClientStream(".", pipeNameAfterRebuild, PipeDirection.InOut);
                        pipeClientafterRebuild.Connect();

                        logger.Info("Loading mods");

                        hookForUnload?.Dispose();

                        return o;
                    });

                    //stops GC from deleting it
                    GC.SuppressFinalize(hookForUnload);
                }

                ReloadMods();
            }
        }

        /// <summary>
        /// Builds and reloads the mods in the ModsToReload list.<br/>
        /// </summary>
        /// <param name="actionAfterBuild">Action to invoke after building or before reloading.</param>
        private static void BuildOrReloadMods()
        {
            if (IsModsToReloadEmpty || forceJustReload)
            {
                ReloadMods();
            }
            else
            {
                BuildAndReloadMods();
            }
        }

        /// <summary>
        /// Sets up a hook for creating a server after unloading the mods.
        /// </summary>
        /// <param name="worldIdToLoad">ID of the world to load.</param>
        private static void CreateServerBeforeUnloadHook(int worldIdToLoad)
        {
            Hook hookForUnload = null;

            hookForUnload = new Hook(typeof(ModLoader).GetMethod("Unload", BindingFlags.NonPublic | BindingFlags.Static), (Func<bool> orig) =>
            {
                var logger = LogManager.GetLogger("MODRELOADER_UNLOAD");

                try
                {
                    Main.LoadWorlds();

                    if (Main.WorldList.Count == 0)
                        throw new Exception("No worlds found.");

                    // Getting Player and World from ClientDataHandler
                    Main.ActiveWorldFileData = Main.WorldList[worldIdToLoad];

                    string text = "-autoshutdown -password \"" + Main.ConvertToSafeArgument(Netplay.ServerPassword) + "\" -lang " + Language.ActiveCulture.LegacyId;
                    if (Platform.IsLinux)
                        text = nint.Size != 8 ? text + " -x86" : text + " -x64";

                    text = !Main.ActiveWorldFileData.IsCloudSave ? text + Main.instance.SanitizePathArgument("world", Main.worldPathName) : text + Main.instance.SanitizePathArgument("cloudworld", Main.worldPathName);
                    text = text + " -worldrollbackstokeep " + Main.WorldRollingBackupsCountToKeep;

                    // TML options
                    text += $@" -modpath ""{ModOrganizer.modPath}""";

                    // if (Program.LaunchParameters.TryGetValue("-tmlsavedirectory", out var tmlsavedirectory))
                    // text += $@" -tmlsavedirectory ""{tmlsavedirectory}""";
                    // else if (Program.LaunchParameters.TryGetValue("-savedirectory", out var savedirectory))
                    // text += $@" -savedirectory ""{savedirectory}""";

                    if (Main.showServerConsole)
                        text += " -showserverconsole";

                    var tServer = new Process();

                    tServer.StartInfo.FileName = Environment.ProcessPath;
                    tServer.StartInfo.Arguments = "tModLoader.dll -server " + text;
                    if (Main.libPath != "")
                    {
                        ProcessStartInfo startInfo = tServer.StartInfo;
                        startInfo.Arguments = startInfo.Arguments + " -loadlib " + Main.libPath;
                    }

                    tServer.StartInfo.UseShellExecute = true;
                    if (!Main.showServerConsole)
                        tServer.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    if (SocialAPI.Network != null)
                        SocialAPI.Network.LaunchLocalServer(tServer, Main.MenuServerMode);
                    else
                        tServer.Start();
                }
                catch (Exception e)
                {
                    logger.Error("Failed to start local server" + e.Message);
                }

                bool o = orig();

                hookForUnload?.Dispose();
                return o;
            });
        }

        /// <summary>
        /// Prepares the client data for the reload process.
        /// </summary>
        /// <param name="clientMode"></param>
        private static void PrepareClient(ClientMode clientMode)
        {
            PrepareClient(clientMode, Utilities.FindPlayerId(), Utilities.FindWorldId());
        }

        private static void PrepareClient(ClientMode clientMode, int playerID, int worldID)
        {
            ClientDataJsonHelper.ClientMode = clientMode;
            ClientDataJsonHelper.PlayerID = playerID;
            ClientDataJsonHelper.WorldID = worldID;
            Log.Info("set player and worldid to " + ClientDataJsonHelper.PlayerID + " and " + ClientDataJsonHelper.WorldID);
        }

        /// <summary>
        /// Bruh just read the name of the function
        /// </summary>
        private static void ReloadMods()
        {
            // Going to reload mod menu(that automaticly invokes reload)
            Main.menuMode = 10002;
        }

        /// <summary>
        /// Exits the world and saves it if needed.
        /// </summary>
        /// <returns></returns>
        private static Task ExitWorld()
        {
            // We cant exit any world if in the main menu, so we just return a completed task
            if (Main.gameMenu)
            {
                return Task.CompletedTask;
            }

            if (Conf.C.SaveWorldBeforeReloading)
            {
                Log.Info("Saving and quitting...");

                // Creating task that will delay reloading a mod until world finish saving
                var tcs = new TaskCompletionSource();
                WorldGen.SaveAndQuit(tcs.SetResult);
                return tcs.Task;
            }
            else
            {
                Log.Info("Just quitting...");
                WorldGen.JustQuit();
                return Task.CompletedTask;
            }
        }

        /// Builds and reloads the mods in the ModsToReload list.<br/>
        /// After building, it invokes the actionAfterBuild action if provided.
        /// </summary>
        /// <param name="actionAfterBuild">Action to invoke after building the mods.</param>
        private static void BuildAndReloadMods(Action actionAfterBuild = null)
        {
            // Find all mod sources
            string[] modSources = ModCompile.FindModSources();

            Log.Info("Executing Mods to reload: " + string.Join(", ", Conf.C.ModsToReload));

            // Get mods paths of mods to reload
            var modPaths = Conf.C.ModsToReload.Select((modName) =>
                modSources.FirstOrDefault(p =>
                    !string.IsNullOrEmpty(p) &&
                    Directory.Exists(p) &&
                    Path.GetFileName(p)?.Equals(modName, StringComparison.InvariantCultureIgnoreCase) == true)).ToList();

            Log.Info("Starting to build mods..." + string.Join(", ", modPaths));

            // Create a task to build the mods
            Main.menuMode = 10003;
            Task.Run(() =>
            {
                try
                {
                    return Interface.buildMod.BuildMod(
                        (mc) =>
                        {
                            for (int i = 0; i < modPaths.Count; i++)
                            {
                                string modPath = modPaths[i];
                                Log.Info("Building mod: " + modPath);
                                try
                                {
                                    mc.Build(modPath);
                                }
                                catch (TargetInvocationException ex)
                                {
                                    throw ex.InnerException!;
                                }
                            }

                            // Invoke the action after building
                            actionAfterBuild?.Invoke();
                        },
                        true
                    );
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException!;
                }
            });
        }
    }
}