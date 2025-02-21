using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Systems
{
    /// <summary>
    /// This system will automatically load a player and world every time AFTER all the mods have been reloaded.
    /// Meaning in OnModLoad. This is useful for testing mods in singleplayer.
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class AutoloadSingleplayerSystem : ModSystem
    {
        private MethodInfo canWorldBePlayedMethod;

        public override void OnModLoad()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Mod.Logger.Info("AutoloadSingleplayerSystem is disabled in multiplayer.");
                return;
            }

            // Get the OnSuccessfulLoad field using reflection
            FieldInfo onSuccessfulLoadField = typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static);

            // Get the CanWorldBePlayed method using reflection
            canWorldBePlayedMethod = typeof(UIWorldSelect).GetMethod("CanWorldBePlayed", BindingFlags.NonPublic | BindingFlags.Static);

            if (onSuccessfulLoadField != null)
            {
                Action onSuccessfulLoad = (Action)onSuccessfulLoadField.GetValue(null);

                if (ClientDataHandler.Mode == ClientMode.SinglePlayer)
                {
                    onSuccessfulLoad += EnterSingleplayerWorld;
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

        private void EnterSingleplayerWorld()
        {
            Mod.Logger.Info("EnterSingleplayerWorld() called!");

            Main.LoadWorlds();
            Main.LoadPlayers();

            if (Main.PlayerList.Count == 0 || Main.WorldList.Count == 0)
                throw new Exception("No players or worlds found.");

            var player = Main.PlayerList[ClientDataHandler.PlayerId];
            var world = Main.WorldList[ClientDataHandler.WorldId];

            StartGameWithPair(player, world);

            /*
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
            */

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
            // Play the selected world in singleplayer
            WorldGen.playWorld();
        }

        private bool IsJourneyWorld(WorldFileData world) => world.GameMode == GameModeID.Creative;
        private bool IsJourneyPlayer(PlayerFileData player) => player.Player.difficulty == GameModeID.Creative;
    }
}