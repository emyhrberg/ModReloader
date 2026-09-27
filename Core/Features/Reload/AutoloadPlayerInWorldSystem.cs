using System;
using System.Diagnostics;
using System.Linq;
using Humanizer;
using ModReloader.Core.Features.MainMenuFeatures;
using ReLogic.OS;
using Terraria.Audio;
using Terraria.ID;
using Terraria.IO;
using Terraria.Localization;
using Terraria.ModLoader.Core;
using Terraria.Social;

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

            // tML installs its network-reload continuation before unloading mods.
            // Remove only our callbacks so the existing connection can finish joining.
            ModLoader.OnSuccessfulLoad -= EnterSingleplayerWorld;
            ModLoader.OnSuccessfulLoad -= EnterMultiplayerWorld;
        }

        public override void OnModLoad()
        {
            // A server-required reload already has a live client loop and a tML
            // continuation. Auto-joining here would start a second connection.
            if (ModNet.NetReloadActive)
                return;

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
            EnterSingleplayerWorld(useStoredPaths: true);
        }

        public static void EnterSingleplayerWorldFromConfig()
        {
            ClientDataMemoryStorage.ClearData();
            EnterSingleplayerWorld(useStoredPaths: false);
        }

        private static void EnterSingleplayerWorld(bool useStoredPaths)
        {
            Log.Info("Entering SP World");

            // Select the player and world
            bool ok = SelectPlayerAndWorld(useStoredPaths: useStoredPaths);

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
                if (!TryMoveToRejectionMenuIfNeeded())
                    Main.menuMode = 0;
            }
            
        }

        /// <summary>
        /// Joins the multiplayer server using the ClientDataHandler.
        /// </summary>
        public static void EnterMultiplayerWorld()
        {
            EnterMultiplayerWorld(useStoredPaths: true);
        }

        public static void EnterMultiplayerWorldFromConfig()
        {
            ClientDataMemoryStorage.ClearData();
            EnterMultiplayerWorld(useStoredPaths: false);
        }

        private static void EnterMultiplayerWorld(bool useStoredPaths)
        {
            Log.Info("Entering MP World");

            // Select the player and world
            bool isPlayerSelected = SelectPlayerAndWorld(onlyPlayer: true, useStoredPaths: useStoredPaths);

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
            ClientDataMemoryStorage.ClearData();

            // Select the player and world
            bool isPlayerAndWorldSelected = SelectPlayerAndWorld(useStoredPaths: false);

            if (isPlayerAndWorldSelected)
            {
                // Host the server
                Main.showServerConsole = true;
                StartLocalServerWithConfiguredArguments("");
            }
            else
            {
                Log.Error("Failed to select player and world for hosting multiplayer.");
                if (TryMoveToRejectionMenuIfNeeded())
                    return;
                Main.menuMode = 0;
            }
        }

        private static void StartLocalServerWithConfiguredArguments(string password)
        {
            Netplay.ServerPassword = password;

            string arguments = "-autoshutdown -password \"" + Main.ConvertToSafeArgument(password) + "\" -lang " + Language.ActiveCulture.LegacyId;
            if (Platform.IsLinux)
                arguments += nint.Size != 8 ? " -x86" : " -x64";

            arguments += !Main.ActiveWorldFileData.IsCloudSave
                ? Main.instance.SanitizePathArgument("world", Main.worldPathName)
                : Main.instance.SanitizePathArgument("cloudworld", Main.worldPathName);
            arguments += " -worldrollbackstokeep " + Main.WorldRollingBackupsCountToKeep;
            arguments += $" -modpath \"{ModOrganizer.modPath}\"";

            if (Program.LaunchParameters.TryGetValue("-tmlsavedirectory", out string tmlSaveDirectory))
                arguments += $" -tmlsavedirectory \"{tmlSaveDirectory}\"";
            else if (Program.LaunchParameters.TryGetValue("-savedirectory", out string saveDirectory))
                arguments += $" -savedirectory \"{saveDirectory}\"";

            if (Main.showServerConsole)
                arguments += " -showserverconsole";

            string configuredArguments = MainMenuActions.GetStartServerArgumentsText();
            if (!string.IsNullOrEmpty(configuredArguments))
                arguments += " " + configuredArguments;

            Main.tServer = new Process();
            Main.tServer.StartInfo.FileName = Environment.ProcessPath;
            Main.tServer.StartInfo.Arguments = "tModLoader.dll -server " + arguments;
            if (Main.libPath != "")
                Main.tServer.StartInfo.Arguments += " -loadlib " + Main.libPath;

            Main.tServer.StartInfo.UseShellExecute = true;
            if (!Main.showServerConsole)
                Main.tServer.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            if (SocialAPI.Network != null)
                SocialAPI.Network.LaunchLocalServer(Main.tServer, Main.MenuServerMode);
            else
                Main.tServer.Start();

            Netplay.SetRemoteIP("127.0.0.1");
            Main.autoPass = true;
            Main.statusText = Lang.menu[8].Value;
            Netplay.StartTcpClient();
            Main.menuMode = 10;
        }

        /// <summary>
        /// Selects the player and world based on the ClientDataHandler.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private static bool SelectPlayerAndWorld(bool onlyPlayer = false, bool useStoredPaths = true)
        {
            LoadPlayerAndWorldLists();

            if (Main.PlayerList == null || Main.PlayerList.Count == 0)
            {
                Log.Error("No players found after loading players.");
                return false;
            }
            int playerId = Conf.C.Player?.Type ?? 0;
            if (playerId < 0 || playerId >= Main.PlayerList.Count)
            {
                Log.Error($"Invalid player index {playerId}. Cannot autoload player.");
                return false;
            }
            var player = Main.PlayerList[playerId];

            if (useStoredPaths && !string.IsNullOrEmpty(ClientDataMemoryStorage.PlayerPath) && ClientDataMemoryStorage.ClientMode != ClientMode.FreshClient)
            {
                PlayerFileData storedPlayer = Main.PlayerList.FirstOrDefault(p => string.Equals(p.Path, ClientDataMemoryStorage.PlayerPath, StringComparison.OrdinalIgnoreCase), null);
                if (storedPlayer != null)
                    player = storedPlayer;
                else
                    Log.Warn($"Stored player path no longer exists: {ClientDataMemoryStorage.PlayerPath}. Falling back to configured player.");
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

            int worldId = Conf.C.World?.Type ?? 0;
            if (worldId < 0 || worldId >= Main.WorldList.Count)
            {
                Log.Error($"Invalid world index {worldId}. Cannot autoload world.");
                return false;
            }
            var world = Main.WorldList[worldId];

            if (useStoredPaths && !string.IsNullOrEmpty(ClientDataMemoryStorage.WorldPath) && ClientDataMemoryStorage.ClientMode != ClientMode.FreshClient)
            {
                WorldFileData storedWorld = Main.WorldList.FirstOrDefault(p => string.Equals(p.Path, ClientDataMemoryStorage.WorldPath, StringComparison.OrdinalIgnoreCase), null);
                if (storedWorld != null)
                    world = storedWorld;
                else
                    Log.Warn($"Stored world path no longer exists: {ClientDataMemoryStorage.WorldPath}. Falling back to configured world.");
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
            if (Main.PlayerList == null || Main.WorldList == null || Conf.C.Player == null || Conf.C.World == null)
                return false;

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
