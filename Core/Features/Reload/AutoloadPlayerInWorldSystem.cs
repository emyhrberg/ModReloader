using System;
using System.Linq;
using System.Reflection;
using Humanizer;
using ModReloader.Core.Features.MainMenuFeatures;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace ModReloader.Core.Features.Reload
{
    /// <summary>
    /// This system handles the automatic loading of players and worlds in singleplayer and multiplayer modes.
    /// </summary>
    public class AutoloadPlayerInWorldSystem : ModSystem
    {
        // Useful hooks:
        // OnModLoad, OnLoad.
        // TODO: Investigate differences between the above methods.
        // Also, NetReceive, NetSend, , CanWorldBePlayed, OnWorldLoad, etc.

        public override void Unload()
        {
            if (Main.netMode != NetmodeID.Server)
                ClientDataMemoryStorage.WriteData();

            // Reset some hooks
            typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static)?.SetValue(null, null);
        }

        public override void OnModLoad()
        {
            if (!Conf.C.AutoJoinWorld)
            {
                Log.Info("AutoJoinWorld is disabled. Skipping EnterSingleplayerWorld() hook.");
                return;
            }

            if (ClientDataMemoryStorage.ClientMode == ClientMode.SinglePlayer)
            {
                // Modify the delegate to call EnterSingleplayerWorld() when OnSuccessfulLoad is called
                ModLoader.OnSuccessfulLoad += EnterSingleplayerWorld;
            }
            else if (ClientDataMemoryStorage.ClientMode == ClientMode.MPMajor || ClientDataMemoryStorage.ClientMode == ClientMode.MPMinor)
            {
                ModLoader.OnSuccessfulLoad += EnterMultiplayerWorld;
            }
        }

        public static void LoadPlayerAndWorldLists()
        {
            Log.Info("Loading player and world lists...");
            Main.LoadPlayers();
            Main.LoadWorlds();
            Log.Info($"Loaded {Main.PlayerList?.Count ?? 0} players and {Main.WorldList?.Count ?? 0} worlds.");
        }

        /// <summary>
        /// Enters the singleplayer world using the ClientDataHandler.
        /// </summary>
        public static void EnterSingleplayerWorld()
        {
            Log.Info("Entering SP World");

            // Select the player and world
            bool ok = SelectPlayerAndWorld();

            if (ok)
            {
                // Play the selected world in singleplayer
                WorldGen.playWorld();

                // Show the custom load screen
                LoadWorldState.Show(Main.ActiveWorldFileData.Name, Main.ActivePlayerFileData.Name);
            }
            else
            {
                Log.Error("Failed to select player and world for singleplayer.");
                if (TryMoveToRejectionMenuIfNeeded())
                    return;
            }
        }

        /// <summary>
        /// Joins the multiplayer server using the ClientDataHandler.
        /// </summary>
        public static void EnterMultiplayerWorld()
        {
            Log.Info("Entering MP World");

            // Select the player and world
            bool isPlayerSelected = SelectPlayerAndWorld(onlyPlayer: true);

            if (isPlayerSelected)
            {
                // Join the localhost server (code taken from Main.instance.OnSubmitServerPassword())
                Netplay.SetRemoteIP("127.0.0.1");
                Main.autoPass = true;
                Main.statusText = Lang.menu[8].Value;
                Netplay.StartTcpClient();
                Main.menuMode = 10;
            }
            else
            {
                Log.Error("Failed to select player for multiplayer world.");
                Main.menuMode = 0;
            }
        }

        /// <summary>
        /// Hosts the multiplayer world using the ClientDataHandler.
        /// </summary>
        public static void HostMultiplayerWorld()
        {
            Log.Info("Hosting MP World");

            // Select the player and world
            bool isPlayerAndWorldSelected = SelectPlayerAndWorld();

            if (isPlayerAndWorldSelected)
            {
                // Host the server
                Main.showServerConsole = true;
                Main.instance.OnSubmitServerPassword("");
            }
            else
            {
                Log.Error("Failed to select player and world for hosting multiplayer.");
                if (TryMoveToRejectionMenuIfNeeded())
                    return;
                Main.menuMode = 0;
            }
        }

        /// <summary>
        /// Selects the player and world based on the ClientDataHandler.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private static bool SelectPlayerAndWorld(bool onlyPlayer = false)
        {
            LoadPlayerAndWorldLists();

            if (Main.PlayerList == null || Main.PlayerList.Count == 0)
            {
                Log.Error("No players found after loading players.");
                return false;
            }
            int playerId = Conf.C.Player.Type;
            if (playerId < 0 || playerId >= Main.PlayerList.Count)
            {
                Log.Error($"Invalid player index {playerId}. Cannot autoload player.");
                return false;
            }
            var player = Main.PlayerList[playerId];

            if (ClientDataMemoryStorage.PlayerPath != null && ClientDataMemoryStorage.ClientMode != ClientMode.FreshClient)
            {
                player = Main.PlayerList.FirstOrDefault(p => p.Path.Equals(ClientDataMemoryStorage.PlayerPath), null);
            }

            if (player == null)
            {
                Log.Error("Player not found. Cannot autoload player.");
                return false;
            }
            Main.SelectPlayer(player);

            if (onlyPlayer)
            {
                Log.Info("Found player: " + player.Name);
                return true;
            }

            if (Main.WorldList == null || Main.WorldList.Count == 0)
            {
                Log.Error("No worlds found after loading worlds.");
                return false;
            }

            int worldId = Conf.C.World.Type;
            if (worldId < 0 || worldId >= Main.WorldList.Count)
            {
                Log.Error($"Invalid world index {worldId}. Cannot autoload world.");
                return false;
            }
            var world = Main.WorldList[worldId];

            if (ClientDataMemoryStorage.WorldPath != null && ClientDataMemoryStorage.ClientMode != ClientMode.FreshClient)
            {
                world = Main.WorldList.FirstOrDefault(p => p.Path.Equals(ClientDataMemoryStorage.WorldPath), null);
            }

            if (world == null)
            {
                Log.Error("World not found. Cannot autoload world.");
                return false;
            }

            if ((world.GameMode == GameModeID.Creative) != (player._player.difficulty == PlayerDifficultyID.Creative))
            {
                return false;
            }

            world.SetAsActive();

            Log.Info("Found player: " + player.Name + ", world: " + world.Name);
            return true;
        }

        #region Rejection
        private static bool TryMoveToRejectionMenuIfNeeded()
        {
            // Resolve player from config
            int playerId = Conf.C.Player.Type;
            if (playerId < 0 || playerId >= Main.PlayerList.Count)
                return false;
            var playerFile = Main.PlayerList[playerId];

            // Resolve world from config
            int worldId = Conf.C.World.Type;
            if (worldId < 0 || worldId >= Main.WorldList.Count)
                return false;
            var worldFile = Main.WorldList[worldId];

            if (playerFile?.Player == null || worldFile == null)
                return false;

            // Validate world game mode is registered
            if (!Main.RegisteredGameModes.TryGetValue(worldFile.GameMode, out var modeData))
            {
                SoundEngine.PlaySound(10);
                ShowRejectionUI(Language.GetTextValue("UI.WorldCannotBeLoadedBecauseItHasAnInvalidGameMode"));
                return true;
            }

            // Journey mismatch
            bool playerIsJourney = playerFile.Player.difficulty == PlayerDifficultyID.Creative;
            bool worldIsJourney = worldFile.GameMode == GameModeID.Creative;

            if (playerIsJourney && !worldIsJourney)
            {
                SoundEngine.PlaySound(10);
                string msg = Language.GetTextValue("UI.PlayerIsCreativeAndWorldIsNotCreative");
                msg += $"\nPlayer: [c/ffff00:{playerFile.Player.name}] (Journey)";
                msg += $"\nWorld: [c/ffff00:{worldFile.GetWorldName()}] (Non-Journey)";
                ShowRejectionUI(msg);
                return true;
            }
            if (!playerIsJourney && worldIsJourney)
            {
                SoundEngine.PlaySound(10);
                string msg = Language.GetTextValue("UI.PlayerIsNotCreativeAndWorldIsCreative");
                msg += $"\nPlayer: [c/ffff00:{playerFile.Player.name}] (Non-Journey)";
                msg += $"\nWorld: [c/ffff00:{worldFile.GetWorldName()}] (Journey)";
                ShowRejectionUI(msg);
                return true;
            }

            // Other rejections
            if (!SystemLoader.CanWorldBePlayed(playerFile, worldFile, out var rejector))
            {
                SoundEngine.PlaySound(10);
                ShowRejectionUI(rejector.WorldCanBePlayedRejectionMessage(playerFile, worldFile));
                return true;
            }

            return false;
        }

        private static void ShowRejectionUI(string message)
        {
            // Update the status text (optional)
            Main.statusText = message;

            // Swap to our custom rejection state
            Main.MenuUI.SetState(new RejectionState(message));

            Main.menuMode = 888;
        }
        #endregion
    }
}