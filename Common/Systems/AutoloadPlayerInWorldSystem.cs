using System;
using System.Linq;
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
            bool isPlayerAndWorldSelected = SelectPlayerAndWorld();

            if (isPlayerAndWorldSelected)
            {
                // Play the selected world in singleplayer
                WorldGen.playWorld();

                // Show the custom load screen
                LoadWorldState.Show(Main.ActiveWorldFileData.Name);
            }
            else
            {
                Log.Error("Failed to select player and world for singleplayer.");
                Main.menuMode = 0;
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
                Main.instance.OnSubmitServerPassword("");
            }
            else
            {
                Log.Error("Failed to select player and world for hosting multiplayer.");
                Main.menuMode = 0;
            }
        }

        /// <summary>
        /// Selects the player and world based on the ClientDataHandler.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private static bool SelectPlayerAndWorld(bool onlyPlayer = false)
        {
            Main.LoadPlayers();
            if (Main.PlayerList == null || Main.PlayerList.Count == 0)
            {
                Log.Error("No players found after loading players.");
                return false;
            }

            var player = Main.PlayerList.Count > Conf.C.Player ? Main.PlayerList[Conf.C.Player] : null;

            if (ClientDataJsonHelper.PlayerPath != null && ClientDataJsonHelper.ClientMode != ClientMode.FreshClient)
            {
                player = Main.PlayerList.FirstOrDefault(p => p.Path.Equals(ClientDataJsonHelper.PlayerPath), null);
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

            Main.LoadWorlds();
            if (Main.WorldList == null || Main.WorldList.Count == 0)
            {
                Log.Error("No worlds found after loading worlds.");
                return false;
            }

            var world = Main.WorldList.Count > Conf.C.World ? Main.WorldList[Conf.C.World] : null;

            if (ClientDataJsonHelper.WorldPath != null && ClientDataJsonHelper.ClientMode != ClientMode.FreshClient)
            {
                world = Main.WorldList.FirstOrDefault(p => p.Path.Equals(ClientDataJsonHelper.WorldPath), null);
            }

            if (world == null)
            {
                Log.Error("World not found. Cannot autoload world.");
                return false;
            }

            world.SetAsActive();

            Log.Info("Found player: " + player.Name + ", world: " + world.Name);
            return true;
        }
    }
}