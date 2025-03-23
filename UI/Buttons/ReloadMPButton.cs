using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using System.Collections;
using System.Reflection;
using MonoMod.RuntimeDetour;
using System.Diagnostics;

namespace SquidTestingMod.UI.Buttons
{
    public class ReloadMPButton : BaseButton
    {
        // Set custom animation dimensions
        private float _scale = 1.25f;
        protected override float Scale => _scale;
        protected override int FrameCount => 5;
        protected override int FrameSpeed => 12;
        protected override int FrameWidth => 65;
        protected override int FrameHeight => 65;

        // Constructor
        public ReloadMPButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, float textSize) : base(spritesheet, buttonText, hoverText, textSize)
        {
            // deactived by default since the SP button is active
            // if (Main.netMode == NetmodeID.SinglePlayer)
            // {
            // Active = false;
            // ButtonText.Active = false;
            // }
            // else if (Main.netMode == NetmodeID.MultiplayerClient)
            // {
            // Active = true;
            // ButtonText.Active = true;
            // }
        }

        public async override void LeftClick(UIMouseEvent evt)
        {
            // If alt+click, toggle the mode and return
            bool altClick = Main.keyState.IsKeyDown(Keys.LeftAlt);
            if (altClick)
            {
                ReloadUtilities.PrepareClient(ClientModes.MPMain);

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    await ReloadUtilities.ExitAndKillServer();

                    int clientCount = Main.player.Where((p) => p.active).Count() - 1;
                    List<NamedPipeServerStream> clients = new List<NamedPipeServerStream>();
                    List<Task<string?>> clientResponses = new List<Task<string?>>();

                    Log.Info($"Waiting for {clientCount} clients...");

                    // Wait for conecting all clients
                    for (int i = 0; i < clientCount; i++)
                    {
                        var pipeServer = new NamedPipeServerStream(ReloadUtilities.pipeName, PipeDirection.InOut, clientCount);
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

                    ReloadUtilities.ReloadMod();

                    var logger = LogManager.GetLogger("SQUID");

                    logger.Info("Mod Reloaded");
                    // After building - reload all other clients
                    List<NamedPipeServerStream> clientsAfterRebuild = new List<NamedPipeServerStream>();

                    logger.Info($"Waiting for {clientCount} clients...");

                    for (int i = 0; i < clientCount; i++)
                    {
                        var pipeServer = new NamedPipeServerStream(ReloadUtilities.pipeNameAfterRebuild, PipeDirection.InOut, clientCount);
                        GC.SuppressFinalize(pipeServer);
                        pipeServer.WaitForConnection();
                        clientsAfterRebuild.Add(pipeServer);
                        logger.Info($"Client {i + 1} connected after building!");
                    }

                    foreach (var client in clientsAfterRebuild)
                    {
                        client.Close();
                        client.Dispose();
                    }


                }
                else if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    await ReloadUtilities.ExitWorldOrServer();
                    ReloadUtilities.ReloadMod();
                }
                return;
            }

            ReloadUtilities.PrepareClient(ClientModes.MPMain);

            // we must be in multiplayer for this to have an effect
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                await ReloadUtilities.ExitAndKillServer();

                int clientCount = Main.player.Where((p) => p.active).Count() - 1;
                List<NamedPipeServerStream> clients = new List<NamedPipeServerStream>();
                List<Task<string?>> clientResponses = new List<Task<string?>>();

                Log.Info($"Waiting for {clientCount} clients...");

                // Wait for conecting all clients
                for (int i = 0; i < clientCount; i++)
                {
                    var pipeServer = new NamedPipeServerStream(ReloadUtilities.pipeName, PipeDirection.InOut, clientCount);
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

                int worldIdToLoad = ClientDataHandler.WorldID;

                ReloadUtilities.BuildAndReloadMod(() =>
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
                                        return nameProperty?.GetValue(mod)?.ToString() == "SquidTestingMod";
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
                await ReloadUtilities.ExitWorldOrServer();

                int worldIdToLoad = ClientDataHandler.WorldID;

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

                ReloadUtilities.BuildAndReloadMod();
            }
        }
        // public override void RightClick(UIMouseEvent evt)
        // {
        //     // If right click, toggle the mode and return
        //     Active = false;
        //     ButtonText.Active = false;

        //     // set MP active
        //     MainSystem sys = ModContent.GetInstance<MainSystem>();
        //     foreach (var btn in sys?.mainState?.AllButtons)
        //     {
        //         if (btn is ReloadSPButton spBtn)
        //         {
        //             spBtn.Active = true;
        //             spBtn.ButtonText.Active = true;
        //         }
        //     }
        //     return;
        // }
    }
}