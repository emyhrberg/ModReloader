using System;
using System.Reflection;
using ModReloader.Common.Configs;
using ModReloader.Helpers;
using Terraria.ID;

namespace ModReloader.Common.Systems
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
                ClientDataJsonHelper.WriteData();

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

            // Get the OnSuccessfulLoad field using reflection
            FieldInfo onSuccessfulLoadField = typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static);

            if (onSuccessfulLoadField != null)
            {
                Action onSuccessfulLoad = (Action)onSuccessfulLoadField.GetValue(null);

                if (ClientDataJsonHelper.ClientMode == ClientMode.SinglePlayer)
                {
                    // Modify the delegate to call EnterSingleplayerWorld() when OnSuccessfulLoad is called
                    onSuccessfulLoad += EnterSingleplayerWorld;
                }
                else if (ClientDataJsonHelper.ClientMode == ClientMode.MPMajor || ClientDataJsonHelper.ClientMode == ClientMode.MPMinor)
                {
                    onSuccessfulLoad += EnterMultiplayerWorld;
                }

                // Set the modified delegate back to the field
                onSuccessfulLoadField.SetValue(null, onSuccessfulLoad);
            }
            else
            {
                Log.Warn("Failed to access OnSuccessfulLoad field.");
            }
        }

        /// <summary>
        /// Enters the singleplayer world using the ClientDataHandler.
        /// </summary>
        public static void EnterSingleplayerWorld()
        {
            Log.Info("Entering SP World");

            // Select the player and world
            SelectPlayerAndWorld();

            // Play the selected world in singleplayer
            WorldGen.playWorld();

            // Show the custom load screen
            LoadWorldState.Show(Main.ActiveWorldFileData.Name);
        }

        /// <summary>
        /// Joins the multiplayer server using the ClientDataHandler.
        /// </summary>
        public static void EnterMultiplayerWorld()
        {
            Log.Info("Entering MP World");

            // Select the player and world
            SelectPlayerAndWorld();

            // Join the localhost server (code taken from Main.instance.OnSubmitServerPassword())
            Netplay.SetRemoteIP("127.0.0.1");
            Main.autoPass = true;
            Main.statusText = Lang.menu[8].Value;
            Netplay.StartTcpClient();
            Main.menuMode = 10;
        }

        /// <summary>
        /// Hosts the multiplayer world using the ClientDataHandler.
        /// </summary>
        public static void HostMultiplayerWorld()
        {
            Log.Info("Hosting MP World");

            // Select the player and world
            SelectPlayerAndWorld();

            // Host the server
            Main.instance.OnSubmitServerPassword("");
        }

        /// <summary>
        /// Selects the player and world based on the ClientDataHandler.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private static void SelectPlayerAndWorld()
        {
            Main.LoadPlayers();
            if (Main.PlayerList == null || Main.PlayerList.Count == 0)
            {
                Log.Error("No players found after loading players.");
                return;
            }

            Main.LoadWorlds();
            if (Main.WorldList == null || Main.WorldList.Count == 0)
            {
                Log.Error("No worlds found after loading worlds.");
                return;
            }

            // Select player and world based on json
            int playerID = ClientDataJsonHelper.PlayerID;
            int worldID = ClientDataJsonHelper.WorldID;

            var player = Main.PlayerList[Conf.C.Player];
            var world = Main.WorldList[Conf.C.World];

            if (playerID == -1 || worldID == -1)
            {
                Log.Error("PlayerID or WorldID is -1. Cannot autoload.");
                // if we return here, we cause a "crash" or "stuck" in loading.
            }
            else
            {
                // all ok, continue.
                player = Main.PlayerList[ClientDataJsonHelper.PlayerID];
                world = Main.WorldList[ClientDataJsonHelper.WorldID];
            }

            // Ensure the world's file path is valid
            if (string.IsNullOrEmpty(world.Path))
            {
                Log.Error($"World {world.Name} has an invalid or null path.");
                throw new ArgumentNullException(nameof(world.Path), "World path cannot be null or empty.");
            }

            Log.Info("SelectPlayerAndWorld. Found player: " + player.Name + ", world: " + world.Name);

            // Announce that we have added the player to the world to the server (what server? we are in main menu)
            // ModNetHandler.PlayersInLocalHost.SendPlayersInLocalHost(toWho: 255);

            Main.SelectPlayer(player);
            Main.ActiveWorldFileData = world;
        }
    }
}