using System;
using System.Linq;
using System.Reflection;
using ModHelper.Common.Configs;
using ModHelper.Common.Systems.Menus;
using ModHelper.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModHelper.Common.Systems
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
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        private void EnterSingleplayerWorld()
        {
            Log.Info("EnterSingleplayerWorld() called!");

            // Loading lists of Players and Worlds
            Main.LoadWorlds();
            Main.LoadPlayers();

            // Check if there are any players or worlds available
            if (Main.PlayerList.Count == 0 || Main.WorldList.Count == 0)
                throw new Exception("No players or worlds found.");

            // Getting playerID and worldID and print
            Log.Info("PlayerID: " + ClientDataJsonHelper.PlayerID + ", WorldID: " + ClientDataJsonHelper.WorldID);

            int playerID = ClientDataJsonHelper.PlayerID;
            int worldID = ClientDataJsonHelper.WorldID;

            // Select default player and world if IDs are invalid
            var player = Main.PlayerList.FirstOrDefault();
            var world = Main.WorldList.FirstOrDefault();

            if (playerID == -1 || worldID == -1)
            {
                Log.Error("PlayerID or WorldID is -1. Cannot autoload.");
            }
            else
            {
                // Set the player and world based on the IDs
                player = Main.PlayerList[ClientDataJsonHelper.PlayerID];
                world = Main.WorldList[ClientDataJsonHelper.WorldID];
            }

            Log.Info($"Autoload using ClientDataHandler. Starting game with Player: {player.Name}, World: {world.Name}");

            // Select the player and world
            Main.SelectPlayer(player);
            Main.ActiveWorldFileData = world;

            // Ensure the world's file path is valid
            if (string.IsNullOrEmpty(world.Path))
            {
                Log.Error($"World {world.Name} has an invalid or null path.");
                throw new ArgumentNullException(nameof(world.Path), "World path cannot be null or empty.");
            }

            // Play the selected world in singleplayer
            WorldGen.playWorld();

            // Show the custom load screen
            CustomLoadWorld.Show(world.Name);
        }

        /// <summary>
        /// Joins the multiplayer server using the ClientDataHandler.
        /// </summary>
        /// <exception cref="Exception"></exception>
        private void EnterMultiplayerWorld()
        {
            Log.Info("EnterMultiplayerWorld() called!");

            // Loading lists of Players
            Main.LoadPlayers();

            // Check if there are any players available
            if (Main.PlayerList.Count == 0)
                throw new Exception("No players found.");

            // Getting playerID and print
            Log.Info("PlayerID: " + ClientDataJsonHelper.PlayerID);

            int playerID = ClientDataJsonHelper.PlayerID;

            // Select default player if ID is invalid
            var player = Main.PlayerList.FirstOrDefault();

            if (playerID == -1)
            {
                Log.Error("PlayerID is -1. Cannot autoload.");
            }
            else
            {
                // Set the player based on the ID
                player = Main.PlayerList[ClientDataJsonHelper.PlayerID];
            }

            Log.Info($"Autoload using ClientDataHandler. Starting game with Player: {player.Name}");

            // Select the player
            Main.SelectPlayer(player);

            // Join the server (code taken from Main.instance.OnSubmitServerPassword())
            Netplay.SetRemoteIP("127.0.0.1");
            Main.autoPass = true;
            Main.statusText = Lang.menu[8].Value;
            Netplay.StartTcpClient();
            Main.menuMode = 10;
        }
    }
}