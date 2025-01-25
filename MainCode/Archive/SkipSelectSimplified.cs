//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using SkipSelect.MainCode.Other;
//using Terraria;
//using Terraria.GameContent.UI.States;
//using Terraria.ID;
//using Terraria.IO;
//using Terraria.ModLoader;

//namespace SkipSelect
//{
//    public class SkipSelectSimplifiedd : Mod
//    {
//        private MethodInfo canWorldBePlayedMethod;

//        // --------------------------------------------------------------------
//        // Hooks
//        // --------------------------------------------------------------------
//        public override void Load()
//        {
//            var config = ModContent.GetInstance<Config>();

//            // Get the OnSuccessfulLoad field using reflection
//            FieldInfo onSuccessfulLoadField = typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static);

//            // Get the CanWorldBePlayed method using reflection
//            canWorldBePlayedMethod = typeof(UIWorldSelect).GetMethod("CanWorldBePlayed", BindingFlags.NonPublic | BindingFlags.Static);

//            if (onSuccessfulLoadField != null)
//            {
//                Action onSuccessfulLoad = (Action)onSuccessfulLoadField.GetValue(null);

//                if (config.EnableSingleplayer)
//                {
//                    // Hook into OnSuccessfulLoad to enter single-player world
//                    onSuccessfulLoad += EnterSingleplayerWorld;
//                }
//                else if (config.EnableMultiplayer)
//                {
//                    // Hook into OnSuccessfulLoad to enter multiplayer world
//                    onSuccessfulLoad += EnterMultiplayerWorld;
//                }

//                // Set the modified delegate back to the field
//                onSuccessfulLoadField.SetValue(null, onSuccessfulLoad);
//            }
//            else
//            {
//                Logger.Warn("Failed to access OnSuccessfulLoad field.");
//            }
//        }

//        public override void Unload()
//        {
//            // Reset the OnSuccessfulLoad hook.
//            typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static)
//                ?.SetValue(null, null);

//            // Reset the CanWorldBePlayed method.
//            canWorldBePlayedMethod = null;
//        }

//        // --------------------------------------------------------------------
//        // MULTIPLAYER
//        // --------------------------------------------------------------------
//        private void EnterMultiplayerWorld()
//        {
//            Logger.Info("EnterMultiplayerWorld() called!");

//            Main.LoadWorlds();
//            Main.LoadPlayers();
//            if (Main.PlayerList.Count == 0 || Main.WorldList.Count == 0)
//                throw new Exception("No players or worlds found.");

//            // Get the first player and world
//            var player = Main.PlayerList[0];
//            var world = Main.WorldList[0];

//            Logger.Info($"Starting game with Player: {player.Name}{player.Player.difficulty}, World: {world.Name}{world.CreationTime}");

//            Main.SelectPlayer(player);

//        }

//        // --------------------------------------------------------------------
//        // SINGLEPLAYER
//        // --------------------------------------------------------------------
//        private void EnterSingleplayerWorld()
//        {
//            Logger.Info("EnterSingleplayerWorld() called!");

//            Main.LoadWorlds();
//            Main.LoadPlayers();

//            if (Main.PlayerList.Count == 0 || Main.WorldList.Count == 0)
//                throw new Exception("No players or worlds found.");

//            // Players 
//            var journeyPlayers = Main.PlayerList.ToList().Where(IsJourneyPlayer).ToList();
//            var favoritePlayersNonJourney = Main.PlayerList.Where(p => p.IsFavorite).Except(journeyPlayers).ToList();

//            // Worlds
//            var journeyWorlds = Main.WorldList.ToList().Where(IsJourneyWorld).ToList();
//            var favoriteWorldsNonJourney = Main.WorldList.Where(p => p.IsFavorite).Except(journeyWorlds).ToList();

//            Logger.Info($"Favorite players count: {favoritePlayersNonJourney.Count}/{Main.PlayerList.Count}, " +
//                $"Favorite worlds count: {favoriteWorldsNonJourney.Count}/{Main.WorldList.Count}");

//            // Test combinations, 
//            if (TryFindCompatiblePair(favoritePlayersNonJourney, favoriteWorldsNonJourney, out var player, out var world) ||
//                TryFindCompatiblePair(favoritePlayersNonJourney, favoriteWorldsNonJourney, out player, out world))
//            {
//                StartGameWithPair(player, world);
//            }
//            else
//            {
//                string log = "Error: No compatible player-world pair found. \n" +
//                    "Please favorite a player and a world";
//                Logger.Info(log);
//                throw new Exception(log);
//            }
//        }

//        private bool TryFindCompatiblePair(IEnumerable<PlayerFileData> players, List<WorldFileData> worlds, out PlayerFileData player, out WorldFileData world)
//        {
//            if (canWorldBePlayedMethod == null)
//            {
//                Logger.Error("CanWorldBePlayed method is not cached. Exiting.");
//                player = null;
//                world = null;
//                return false;
//            }

//            foreach (var p in players)
//            {
//                Logger.Info($"Testing player: {p.Name}");
//                p.SetAsActive();

//                foreach (var w in worlds)
//                {
//                    if (w == null)
//                    {
//                        Logger.Warn("Encountered null WorldFileData. Skipping.");
//                        continue;
//                    }

//                    Logger.Info($"Testing world: {w.Name} (GameMode: {w.GameMode}, Favorite: {w.IsFavorite})");

//                    bool canBePlayed = false;

//                    try
//                    {
//                        canBePlayed = (bool)canWorldBePlayedMethod?.Invoke(null, new object[] { w });
//                    }
//                    catch (Exception ex)
//                    {
//                        Logger.Error($"Error invoking CanWorldBePlayed for world {w.Name}: {ex}");
//                        continue;
//                    }

//                    if (canBePlayed)
//                    {
//                        Logger.Info($"Compatible pair found: Player {p.Name}, World {w.Name}");
//                        player = p;
//                        world = w;
//                        return true;
//                    }
//                }
//            }

//            player = null;
//            world = null;
//            return false;
//        }

//        private void StartGameWithPair(PlayerFileData player, WorldFileData world)
//        {
//            Main.SelectPlayer(player);
//            Logger.Info($"Starting game with Player: {player.Name}, World: {world.Name}");
//            WorldGen.playWorld();
//        }

//        private bool IsJourneyWorld(WorldFileData world) => world.GameMode == GameModeID.Creative;
//        private bool IsJourneyPlayer(PlayerFileData player) => player.Player.difficulty == GameModeID.Creative;
//    }
//}
