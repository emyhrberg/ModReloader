using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using ModHelper.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.Common.Systems
{
    public class MainMenuState : UIState
    {
        private float rightVerticalOffset = 0.06f;
        private float leftVerticalOffset = 0.06f;

        public MainMenuState()
        {
            int hoverHeightOffset = 38;
            MainMenuButtonLeftSide button = new("Mod Helper", 0, null, 0.75f);
            Append(button);
            AddLeftSideButton("Singleplayer Join", SingleplayerJoin, "Join first available\n world");
            AddLeftSideButton("Reload Selected Mod", ReloadSelectedMod, "Reload the selected mod\n in config", hoverHeightOffset);

            MainMenuButtonRightSide button2 = new("Multiplayer Helper", 0, null, 0.75f);
            Append(button2);
            // AddRightSideButton("Multiplayer Start Server", StartServer, "Start a server with\n the first available world");
            AddRightSideButton("Multiplayer Start Client", StartClient, "Starts an additional \n instance of tModLoader", hoverHeightOffset * 1);
            // AddRightSideButton("Multiplayer Join", MultiplayerJoin, "Join localhost server\n with first available player", hoverHeightOffset * 2);
        }

        private void StartServer()
        {
            try
            {
                Main.LoadWorlds();

                if (Main.WorldList.Count == 0)
                    throw new Exception("No worlds found.");

                // Getting Player and World from ClientDataHandler
                var world = Main.WorldList.FirstOrDefault();

                if (string.IsNullOrEmpty(world.Path))
                {
                    Log.Error($"World {world.Name} has an invalid or null path.");
                    var worldPath = world.Path;
                    throw new ArgumentNullException(nameof(worldPath), "World path cannot be null or empty.");
                }

                string steamPath = Log.GetSteamPath();
                string startServerFileName = Path.Combine(steamPath, "start-tModLoaderServer.bat");
                if (!File.Exists(startServerFileName))
                {
                    Log.Error("Failed to find start-tModLoaderServer.bat file.");
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
                Log.Error("Failed to start server (start-tModLoaderServer.bat failed to launch): " + e.Message);
                return;
            }
        }

        private void AddLeftSideButton(string text, Action action, string tooltip = "", int yOffset = 0)
        {
            MainMenuButtonLeftSide button = new(text: text, verticalOffset: leftVerticalOffset, action: action, tooltip: tooltip, yOffset: yOffset);
            this.leftVerticalOffset += 0.04f; // Increment the offset for the next button
            Append(button);
        }

        private void AddRightSideButton(string text, Action action, string tooltip = "", int yOffset = 0)
        {
            MainMenuButtonRightSide button = new(text: text, verticalOffset: rightVerticalOffset, action: action, tooltip: tooltip, yOffset: yOffset);
            this.rightVerticalOffset += 0.04f; // Increment the offset for the next button
            Append(button);
        }

        private async void ReloadSelectedMod()
        {
            // await ReloadUtilities.Reload();
        }

        private void MultiplayerJoin()
        {
            // Simply join localhost, easy.
            Log.Info("EnterMultiplayerWorld() called!");
            Main.LoadPlayers();

            if (Main.PlayerList.Count == 0)
                throw new Exception("No players found.");

            // Getting Player and World from ClientDataHandler
            var player = Main.PlayerList.FirstOrDefault();

            // TODO check if player is already in server and if so, join with a different player.

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
                Log.Error($"Ping failed to destination server: {ex}");
                return;
            }

            if (reply.Status == IPStatus.Success)
            {
                Log.Info($"Ping successful to destination server: {reply.Address}");

                // set the IP AND PORT (the two necessary fields) for the server
                Netplay.ServerIP = new System.Net.IPAddress([127, 0, 0, 1]); // localhost
                Netplay.ListenPort = 7777; // default port

                Netplay.StartTcpClient(); // start the TCP client which is later used to connect to the server
            }
            else
            {
                Log.Error($"Ping failed to destination server, possibly timed out: {reply.Status}");
            }
        }

        private void StartClient()
        {
            try
            {
                string steamPath = Log.GetSteamPath();
                string startGameFileName = Path.Combine(steamPath, "start-tModLoader.bat");
                if (!File.Exists(startGameFileName))
                {
                    Log.Error("Failed to find start-tModLoader.bat file.");
                    return;
                }

                // create a process
                ProcessStartInfo process = new(startGameFileName)
                {
                    UseShellExecute = true,
                };

                // start the process
                Process gameProcess = Process.Start(process);
                Log.Info("Game process started with ID: " + gameProcess.Id + " and name: " + gameProcess.ProcessName);
            }
            catch (Exception e)
            {
                Log.Error("Failed to start game process (start-tModLoader.bat failed to launch): " + e.Message);
                return;
            }
        }

        private void SingleplayerJoin()
        {
            Log.Info("EnterSingleplayerWorld() called!");

            // Loading lists of Players and Worlds
            Main.LoadWorlds();
            Main.LoadPlayers();

            if (Main.PlayerList.Count == 0 || Main.WorldList.Count == 0)
                throw new Exception("No players or worlds found.");

            // Check if the first world has a valid path
            if (string.IsNullOrEmpty(Main.WorldList.FirstOrDefault().Path))
            {
                Log.Error($"World {Main.WorldList.FirstOrDefault().Name} has an invalid or null path.");
                var worldPath = Main.WorldList.FirstOrDefault()?.Path;
                throw new ArgumentNullException(nameof(worldPath), "World path cannot be null or empty.");
            }

            // Getting Player and World from ClientDataHandler
            var player = Main.PlayerList.FirstOrDefault();
            var world = Main.WorldList.FirstOrDefault();

            // Start game with pair
            Main.SelectPlayer(player);
            Main.ActiveWorldFileData = world;

            Log.Info($"Starting game with Player: {player.Name}, World: {Main.WorldList.FirstOrDefault().Name}");

            // Play the selected world in singleplayer
            WorldGen.playWorld();
        }
    }
}