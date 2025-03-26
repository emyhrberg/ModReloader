using EliteTestingMod.Common.Configs;
using EliteTestingMod.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Reflection;
using Terraria;
using Terraria.IO;
using Terraria.ModLoader;

namespace EliteTestingMod.Common.Systems
{
    /// <summary>
    /// This system will automatically load a player and world every time AFTER all the mods have been reloaded.
    /// Meaning in OnModLoad. This is useful for testing mods in singleplayer.
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class AutoloadPlayerInWorldSystem : ModSystem
    {
        public override void OnModLoad()
        {
            // Get the OnSuccessfulLoad field using reflection
            FieldInfo onSuccessfulLoadField = typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static);

            if (onSuccessfulLoadField != null)
            {
                Action onSuccessfulLoad = (Action)onSuccessfulLoadField.GetValue(null);

                if (ClientDataHandler.ClientMode == ClientModes.SinglePlayer)
                {
                    onSuccessfulLoad += EnterSingleplayerWorld;
                }
                else if (ClientDataHandler.ClientMode == ClientModes.MPMain)
                {
                    onSuccessfulLoad += EnterLocalServer;
                }
                else if (ClientDataHandler.ClientMode == ClientModes.MPMinor)
                {
                    onSuccessfulLoad += EnterLocalServer;
                }
                // Set the modified delegate back to the field
                onSuccessfulLoadField.SetValue(null, onSuccessfulLoad);
            }
            else
            {
                Mod.Logger.Warn("Failed to access OnSuccessfulLoad field.");
            }
        }

        public override void Unload()
        {
            // Reset some hooks
            typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static)?.SetValue(null, null);
        }

        private void EnterSingleplayerWorld()
        {
            Mod.Logger.Info("EnterSingleplayerWorld() called!");

            // Loading lists of Players and Worlds
            Main.LoadWorlds();
            Main.LoadPlayers();

            if (Main.PlayerList.Count == 0 || Main.WorldList.Count == 0)
                throw new Exception("No players or worlds found.");

            // Getting Player and World from ClientDataHandler
            var player = Main.PlayerList[ClientDataHandler.PlayerID];
            var world = Main.WorldList[ClientDataHandler.WorldID];

            StartGameWithPair(player, world);

            // Reset Mode status (maybe should be moved to Exit World hook but naaaah)
            ClientDataHandler.ClientMode = ClientModes.FreshClient;
        }

        private void StartGameWithPair(PlayerFileData player, WorldFileData world)
        {
            Main.SelectPlayer(player);
            Mod.Logger.Info($"Starting game with Player: {player.Name}, World: {world.Name}");
            Main.ActiveWorldFileData = world;
            // Ensure the world's file path is valid
            if (string.IsNullOrEmpty(world.Path))
            {
                Mod.Logger.Error($"World {world.Name} has an invalid or null path.");
                throw new ArgumentNullException(nameof(world.Path), "World path cannot be null or empty.");
            }
            // Play the selected world in singleplayer
            WorldGen.playWorld();
        }

        private void StartServer()
        {
            try
            {
                Main.LoadWorlds();

                if (Main.WorldList.Count == 0)
                    throw new Exception("No worlds found.");

                // Getting Player and World from ClientDataHandler
                var world = Main.WorldList[ClientDataHandler.WorldID];

                string steamPath = Log.GetSteamPath();
                string startServerFileName = Path.Combine(steamPath, "start-tModLoaderServer.bat");
                if (!File.Exists(startServerFileName))
                {
                    if (Conf.LogToChat) Main.NewText("Failed to find start-tModLoaderServer.bat file.");
                    Mod.Logger.Error("Failed to find start-tModLoaderServer.bat file.");
                    return;
                }

                // create a process
                ProcessStartInfo process = new(startServerFileName)
                {
                    UseShellExecute = true,
                    Arguments = $"-nosteam -world {world.Path}"
                };

                // start the process
                Process serverProcess = Process.Start(process);
                Log.Info("Server process started with ID: " + serverProcess.Id + " and name: " + serverProcess.ProcessName);
            }
            catch (Exception e)
            {
                // log it
                if (Conf.LogToChat) Main.NewText("Failed to start server!!! C:/Program Files (x86)/Steam/steamapps/common/tModLoader/start-tModLoaderServer.bat" + e.Message);
                Log.Error("Failed to start server!!! C:/Program Files (x86)/Steam/steamapps/common/tModLoader/start-tModLoaderServer.bat" + e.Message);
                return;
            }
        }

        private void StartServerAndEnterMultiplayerWorld()
        {
            StartServer();
            EnterLocalServer();
        }

        private void EnterLocalServer()
        {
            Mod.Logger.Info("EnterMultiplayerWorld() called!");
            Main.LoadPlayers();

            if (Main.PlayerList.Count == 0)
                throw new Exception("No players found.");

            // Getting Player and World from ClientDataHandler
            var player = Main.PlayerList[ClientDataHandler.PlayerID];

            Main.SelectPlayer(player);

            // Play the selected world in multiplayer mode
            // Connect to server IP
            Ping pingSender = new();
            PingOptions options = new();
            options.DontFragment = true; // prevent packet from splitting into smaller packets
            string data = "a"; // dummy data to send because the Send method requires it
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data); // convert string to byte array
            int timeout = 2000; // 120 ms timeout before the ping request is considered failed

            // Ping the server IP using the server's IP address
            PingReply reply = null;
            try
            {
                Netplay.ServerIP = new System.Net.IPAddress([127, 0, 0, 1]); // localhost
                reply = pingSender.Send(Netplay.ServerIP, timeout, buffer, options);
            }
            catch (PingException ex)
            {
                Mod.Logger.Error($"Ping failed to destination server: {ex}");
                return;
            }

            if (reply.Status == IPStatus.Success)
            {
                Mod.Logger.Info($"Ping successful to destination server: {reply.Address}");

                // set the IP AND PORT (the two necessary fields) for the server
                Netplay.ServerIP = new System.Net.IPAddress([127, 0, 0, 1]); // localhost
                Netplay.ListenPort = 7777; // default port

                Netplay.StartTcpClient(); // start the TCP client which is later used to connect to the server
            }
            else
            {
                Mod.Logger.Error($"Ping failed to destination server, possibly timed out: {reply.Status}");
            }
        }
    }
}