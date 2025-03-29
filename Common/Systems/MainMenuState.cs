using System;
using System.Linq;
using System.Net.NetworkInformation;
using ModHelper.Helpers;
using Terraria;
using Terraria.UI;

namespace ModHelper.Common.Systems
{
    public class MainMenuState : UIState
    {
        public MainMenuState()
        {
            MainMenuButton btn1 = new(text: "SkipSelect Singleplayer", verticalOffset: 0f, action: SkipSelectSingleplayer);
            MainMenuButton btn2 = new("Host Multiplayer", 0.07f, HostMultiplayer);
            MainMenuButton btn3 = new("SkipSelect Multiplayer", 0.14f, SkipSelectMultiplayer);
            MainMenuButton btn4 = new("Reload selected mod", 0.21f, async () => ReloadSelectedMod());

            Append(btn1);
            Append(btn2);
            Append(btn3);
            Append(btn4);
        }

        private async void ReloadSelectedMod()
        {
            await ReloadUtilities.Reload();
        }

        private void SkipSelectMultiplayer()
        {
            // Simply join localhost, easy.
            Log.Info("EnterMultiplayerWorld() called!");
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

        private void HostMultiplayer()
        {
            // Make this host.
        }

        private void SkipSelectSingleplayer()
        {
            Log.Info("EnterSingleplayerWorld() called!");

            // Loading lists of Players and Worlds
            Main.LoadWorlds();
            Main.LoadPlayers();

            if (Main.PlayerList.Count == 0 || Main.WorldList.Count == 0)
                throw new Exception("No players or worlds found.");

            // Getting Player and World from ClientDataHandler
            var player = Main.PlayerList.FirstOrDefault();

            // Start game with pair
            Main.SelectPlayer(player);
            Log.Info($"Starting game with Player: {player.Name}, World: {Main.WorldList.FirstOrDefault().Name}");
            Main.ActiveWorldFileData = Main.WorldList.FirstOrDefault();
            // Ensure the world's file path is valid
            if (string.IsNullOrEmpty(Main.WorldList.FirstOrDefault().Path))
            {
                Log.Error($"World {Main.WorldList.FirstOrDefault().Name} has an invalid or null path.");
                var worldPath = Main.WorldList.FirstOrDefault()?.Path;
                throw new ArgumentNullException(nameof(worldPath), "World path cannot be null or empty.");
            }
            // Play the selected world in singleplayer
            WorldGen.playWorld();
        }
    }
}