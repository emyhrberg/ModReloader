using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using System.Collections.Generic;
using Terraria.IO;
using Terraria.ModLoader;
using System.Net.NetworkInformation;

namespace SquidTestingMod.src
{
    /// <summary>
    /// This system will do the following when mod has been loaded (when you click Build & Reload in tModLoader):
    /// 1. terminate a server (if its running)
    /// 2. restart the server with the same world, port, password and other settings.
    /// 3. connect to the server as a client once the server is ready.
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class AutoloadMultiplayerSystem : ModSystem
    {
        private MethodInfo canWorldBePlayedMethod;

        public override void OnModLoad()
        {
            // Get the OnSuccessfulLoad field using reflection
            FieldInfo onSuccessfulLoadField = typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static);

            // Get the CanWorldBePlayed method using reflection
            canWorldBePlayedMethod = typeof(UIWorldSelect).GetMethod("CanWorldBePlayed", BindingFlags.NonPublic | BindingFlags.Static);

            if (onSuccessfulLoadField != null)
            {
                Action onSuccessfulLoad = (Action)onSuccessfulLoadField.GetValue(null);
                var config = ModContent.GetInstance<Config>();
                if (config.AutoloadWorld == "Multiplayer")
                {
                    onSuccessfulLoad += EnterMultiplayerWorld;
                }
                // Set the modified delegate back to the field
                onSuccessfulLoadField.SetValue(null, onSuccessfulLoad);
            }
            else
            {
                Mod.Logger.Warn("Failed to access OnSuccessfulLoad field.");
            }
        }

        public override void Unload() // reset some hooks
        {
            typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static)?.SetValue(null, null);
            canWorldBePlayedMethod = null;
        }

        private void EnterMultiplayerWorld()
        {
            Mod.Logger.Info("EnterMultiplayerWorld() called!");

            Main.LoadWorlds();
            Main.LoadPlayers();

            if (Main.PlayerList.Count == 0 || Main.WorldList.Count == 0)
                throw new Exception("No players or worlds found.");

            // Players 
            var journeyPlayers = Main.PlayerList.ToList().Where(IsJourneyPlayer).ToList();
            var favoritePlayersNonJourney = Main.PlayerList.Where(p => p.IsFavorite).Except(journeyPlayers).ToList();

            // Worlds
            var journeyWorlds = Main.WorldList.ToList().Where(IsJourneyWorld).ToList();
            var favoriteWorldsNonJourney = Main.WorldList.Where(p => p.IsFavorite).Except(journeyWorlds).ToList();

            Mod.Logger.Info($"Favorite players count: {favoritePlayersNonJourney.Count}/{Main.PlayerList.Count}, " +
                $"Favorite worlds count: {favoriteWorldsNonJourney.Count}/{Main.WorldList.Count}");

            // Test combinations, 
            if (TryFindCompatiblePair(favoritePlayersNonJourney, favoriteWorldsNonJourney, out var player, out var world) ||
                TryFindCompatiblePair(favoritePlayersNonJourney, favoriteWorldsNonJourney, out player, out world))
            {
                StartGameWithPair(player, world);
            }
            else
            {
                string log = "Error: No compatible player-world pair found. \n" +
                    "Please favorite a player and a world";
                Mod.Logger.Info(log);
                throw new Exception(log);
            }
        }

        private bool TryFindCompatiblePair(List<PlayerFileData> players, List<WorldFileData> worlds, out PlayerFileData player, out WorldFileData world)
        {
            if (canWorldBePlayedMethod == null)
            {
                Mod.Logger.Error("CanWorldBePlayed method is not cached. Exiting.");
                player = null;
                world = null;
                return false;
            }

            foreach (var p in players)
            {
                Mod.Logger.Info($"Testing player: {p.Name}");
                p.SetAsActive();

                foreach (var w in worlds)
                {
                    if (w == null)
                    {
                        Mod.Logger.Warn("Encountered null WorldFileData. Skipping.");
                        continue;
                    }

                    Mod.Logger.Info($"Testing world: {w.Name} (GameMode: {w.GameMode}, Favorite: {w.IsFavorite})");

                    bool canBePlayed = false;

                    try
                    {
                        canBePlayed = (bool)canWorldBePlayedMethod?.Invoke(null, [w]);
                    }
                    catch (Exception ex)
                    {
                        Mod.Logger.Error($"Error invoking CanWorldBePlayed for world {w.Name}: {ex}");
                        continue;
                    }

                    if (canBePlayed)
                    {
                        Mod.Logger.Info($"Compatible pair found: Player {p.Name}, World {w.Name}");
                        player = p;
                        world = w;
                        return true;
                    }
                }
            }

            player = null;
            world = null;
            return false;
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
            // Play the selected world in multiplayer mode
            // Connect to server IP
            Ping pingSender = new Ping();
            PingOptions options = new PingOptions();
            options.DontFragment = true; // prevent packet from splitting into smaller packets
            string data = "a"; // dummy data to send because the Send method requires it
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(data); // convert string to byte array
            int timeout = 120; // 120 ms timeout before the ping request is considered failed

            // Ping the server IP using the server's IP address
            PingReply reply = null;
            try
            {
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
                WorldGen.SaveAndQuit(() =>
                {
                    // set the IP AND PORT (the two necessary fields) for the server
                    Netplay.ServerIP = new System.Net.IPAddress([127, 0, 0, 1]); // localhost
                    Netplay.ListenPort = 7777; // default port

                    Main.menuMode = 10; // set menu mode to 10 (WorldSelect)
                    Netplay.StartTcpClient(); // start the TCP client which is later used to connect to the server
                });
            }
            else
            {
                Mod.Logger.Error($"Ping failed to destination server, possibly timed out: {reply.Status}");
            }
        }

        private bool IsJourneyWorld(WorldFileData world) => world.GameMode == GameModeID.Creative;
        private bool IsJourneyPlayer(PlayerFileData player) => player.Player.difficulty == GameModeID.Creative;
    }
}