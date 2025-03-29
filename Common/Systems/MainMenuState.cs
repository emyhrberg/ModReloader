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
        private float verticalOffset = 0.06f;

        public MainMenuState()
        {
            MainMenuButton button = new MainMenuButton("Mod Helper", 0, null, 0.75f);

            Append(button);

            int hoverHeightOffset = 38;
            AddButton("Singleplayer Join", SkipSelectSingleplayer, "Join first available\n world");
            AddButton("Multiplayer Host", HostMultiplayer, "Host a multiplayer game\n with the first available world", hoverHeightOffset);
            AddButton("Multiplayer Join", SkipSelectMultiplayer, "Join localhost server\n with first available player", hoverHeightOffset * 2);
            AddButton("Reload Selected Mod", ReloadSelectedMod, "Reload the selected mod\n in config", hoverHeightOffset * 3);
        }

        private void AddButton(string text, Action action, string tooltip = "", int yOffset = 0)
        {
            MainMenuButton button = new(text: text, verticalOffset: verticalOffset, action: action, tooltip: tooltip, yOffset: yOffset);
            this.verticalOffset += 0.04f; // Increment the offset for the next button
            Append(button);
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