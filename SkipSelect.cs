using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SkipSelect.Core.System;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;

namespace SkipSelect
{
    [Autoload(Side = ModSide.Client)]
    public class SkipSelect : Mod
    {
        private MethodInfo canWorldBePlayedMethod;

        // --------------------------------------------------------------------
        // Hooks
        // --------------------------------------------------------------------
        public override void Load()
        {
            startMPorSP();
        }

        private void startMPorSP()
        {
            // Get the OnSuccessfulLoad field using reflection
            FieldInfo onSuccessfulLoadField = typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static);

            // Get the CanWorldBePlayed method using reflection
            canWorldBePlayedMethod = typeof(UIWorldSelect).GetMethod("CanWorldBePlayed", BindingFlags.NonPublic | BindingFlags.Static);

            if (onSuccessfulLoadField != null)
            {
                Action onSuccessfulLoad = (Action)onSuccessfulLoadField.GetValue(null);
                var config = ModContent.GetInstance<Config>();
                if (config.EnableSingleplayer)
                {
                    onSuccessfulLoad += EnterSingleplayerWorld;
                }
                else if (config.EnableMultiplayer)
                {
                    //onSuccessfulLoad += EnterMultiplayerWorld;
                }
                // Set the modified delegate back to the field
                onSuccessfulLoadField.SetValue(null, onSuccessfulLoad);
            }
            else
            {
                Logger.Warn("Failed to access OnSuccessfulLoad field.");
            }
        }

        public override void Unload()
        {
            // Reset the OnSuccessfulLoad hook.
            typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static)
                ?.SetValue(null, null);

            // Reset the CanWorldBePlayed method.
            canWorldBePlayedMethod = null;
        }

        // --------------------------------------------------------------------
        // MULTIPLAYER
        // --------------------------------------------------------------------
        private async void EnterMultiplayerWorld()
        {
            Logger.Info("EnterMultiplayerWorld() called!");

            Main.LoadWorlds();
            Main.LoadPlayers();
            if (Main.PlayerList.Count == 0 || Main.WorldList.Count == 0)
                throw new Exception("No players or worlds found.");

            // Get the first player and a favorite world
            var player = Main.PlayerList[0];
            List<WorldFileData> favoriteWorldsNonJourney = Main.WorldList.Where(p => p.IsFavorite).ToList();
            if (favoriteWorldsNonJourney.Count == 0)
            {
                Logger.Error("No favorite worlds found!");
                throw new Exception("No favorite worlds found.");
            }
            var world = favoriteWorldsNonJourney[0];

            Logger.Info($"Starting game with Player: {player.Name} (Difficulty: {player.Player.difficulty}), World: {world.Name} (Created: {world.CreationTime})");

            Main.SelectPlayer(player);
            Main.ActiveWorldFileData = world;

            // Set global flags for server mode
            Main.netMode = NetmodeID.Server;
            Netplay.ServerPassword = "";

            // It is common for server mode to set the active player index to 255
            Main.myPlayer = 255;
            // And also set an appropriate menu mode (often 14 for dedicated server)
            Main.menuMode = 14;

            // Debug info before starting
            Logger.Info($"Active World: {Main.ActiveWorldFileData?.Path}");
            Logger.Info($"Main netmode: {Main.netMode}, Clients count: {Netplay.Clients.Count()}");
            Logger.Info($"TCP Listener before init: {(Netplay.TcpListener != null ? "Initialized" : "Null")}");
            Logger.Info($"Netmode: {Main.netMode}");

            // Ensure disconnect flag is false
            Netplay.Disconnect = false;

            // Call Netplay.Initialize() to set up internal buffers; this should be called early
            Logger.Info("Init begin! ");
            Netplay.Initialize();
            Logger.Info("Init success! ");

            Logger.Info($"TCP Listener after init: {(Netplay.TcpListener != null ? "Initialized" : "Null")}");
            Logger.Info($"Netmode: {Main.netMode}");

            Logger.Info("Start server in 3 seconds!");
            await Task.Delay(3000);
            Logger.Info("Start server now!");

            try
            {
                // Call StartServer(), which should call InitializeServer() internally
                Netplay.StartServer();
                Logger.Info("Netplay.StartServer() called successfully.");
            }
            catch (Exception ex)
            {
                Logger.Error("Exception during Netplay.StartServer(): " + ex);
            }
        }
        private void CallInitializeServer()
        {
            try
            {
                // Get the type of Netplay
                Type netplayType = typeof(Netplay);

                // Get the private static InitializeServer method
                MethodInfo initializeServerMethod = netplayType.GetMethod("InitializeServer", BindingFlags.NonPublic | BindingFlags.Static);

                if (initializeServerMethod != null)
                {
                    // Invoke the method
                    initializeServerMethod.Invoke(null, null); // Static method, so pass 'null' for the instance
                    Logger.Info("InitializeServer successfully invoked.");
                }
                else
                {
                    Logger.Error("Could not find InitializeServer method.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Error invoking InitializeServer: {ex.Message}\n{ex.StackTrace}");
            }
        }


        // --------------------------------------------------------------------
        // SINGLEPLAYER
        // --------------------------------------------------------------------
        private void EnterSingleplayerWorld()
        {
            Logger.Info("EnterSingleplayerWorld() called!");

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

            Logger.Info($"Favorite players count: {favoritePlayersNonJourney.Count}/{Main.PlayerList.Count}, " +
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
                Logger.Info(log);
                throw new Exception(log);
            }
        }

        private bool TryFindCompatiblePair(IEnumerable<PlayerFileData> players, List<WorldFileData> worlds, out PlayerFileData player, out WorldFileData world)
        {
            if (canWorldBePlayedMethod == null)
            {
                Logger.Error("CanWorldBePlayed method is not cached. Exiting.");
                player = null;
                world = null;
                return false;
            }

            foreach (var p in players)
            {
                Logger.Info($"Testing player: {p.Name}");
                p.SetAsActive();

                foreach (var w in worlds)
                {
                    if (w == null)
                    {
                        Logger.Warn("Encountered null WorldFileData. Skipping.");
                        continue;
                    }

                    Logger.Info($"Testing world: {w.Name} (GameMode: {w.GameMode}, Favorite: {w.IsFavorite})");

                    bool canBePlayed = false;

                    try
                    {
                        canBePlayed = (bool)canWorldBePlayedMethod?.Invoke(null, new object[] { w });
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Error invoking CanWorldBePlayed for world {w.Name}: {ex}");
                        continue;
                    }

                    if (canBePlayed)
                    {
                        Logger.Info($"Compatible pair found: Player {p.Name}, World {w.Name}");
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
            Logger.Info($"Starting game with Player: {player.Name}, World: {world.Name}");
            Main.ActiveWorldFileData = world;
            // Ensure the world's file path is valid
            if (string.IsNullOrEmpty(world.Path))
            {
                Logger.Error($"World {world.Name} has an invalid or null path.");
                throw new ArgumentNullException(nameof(world.Path), "World path cannot be null or empty.");
            }
            // Play the selected world
            WorldGen.playWorld();
        }

        private bool IsJourneyWorld(WorldFileData world) => world.GameMode == GameModeID.Creative;
        private bool IsJourneyPlayer(PlayerFileData player) => player.Player.difficulty == GameModeID.Creative;
    }
}
