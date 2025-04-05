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
    /// 
    /// </summary>
    public class AutoloadPlayerInWorldSystem : ModSystem
    {
        // Useful overrides:
        // OnModLoad, OnLoad.
        // TODO: Investigate differences between the above methods.
        // Also, NetReceive, NetSend, , CanWorldBePlayed, OnWorldLoad, etc.

        public override void Load()
        {
            if (Main.netMode != NetmodeID.Server)
                ClientDataHandler.ReadData();
        }

        public override void Unload()
        {
            if (Main.netMode != NetmodeID.Server)
                ClientDataHandler.WriteData();

            // Reset some hooks
            typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static)?.SetValue(null, null);
        }

        public override void OnModLoad()
        {
            if (!Conf.C.AutoJoinWorld)
            {
                Log.Info("AutoJoinWorldAfterUnload is disabled. Skipping EnterSingleplayerWorld() hook.");
                return;
            }

            // Get the OnSuccessfulLoad field using reflection
            FieldInfo onSuccessfulLoadField = typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static);

            if (onSuccessfulLoadField != null)
            {
                Action onSuccessfulLoad = (Action)onSuccessfulLoadField.GetValue(null);

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    // Modify the delegate to call EnterSingleplayerWorld() when OnSuccessfulLoad is called
                    onSuccessfulLoad += EnterSingleplayerWorld;
                }

                // TODO multiplayer here.

                // Set the modified delegate back to the field
                onSuccessfulLoadField.SetValue(null, onSuccessfulLoad);
            }
            else
            {
                Log.Warn("Failed to access OnSuccessfulLoad field.");
            }
        }

        private void EnterSingleplayerWorld()
        {
            Log.Info("EnterSingleplayerWorld() called!");

            // Loading lists of Players and Worlds
            Main.LoadWorlds();
            Main.LoadPlayers();

            if (Main.PlayerList.Count == 0 || Main.WorldList.Count == 0)
                throw new Exception("No players or worlds found.");

            // getting playerID and worldID and print
            Log.Info("PlayerID: " + ClientDataHandler.PlayerID + ", WorldID: " + ClientDataHandler.WorldID);

            int playerID = ClientDataHandler.PlayerID;
            int worldID = ClientDataHandler.WorldID;

            var player = Main.PlayerList.FirstOrDefault();
            var world = Main.WorldList.FirstOrDefault();

            if (playerID == -1 || worldID == -1)
            {
                Log.Error("PlayerID or WorldID is -1. Cannot autoload.");
                // if we return here, we cause a "crash" or "stuck" in loading.
            }
            else
            {
                // all ok, continue.
                player = Main.PlayerList[ClientDataHandler.PlayerID];
                world = Main.WorldList[ClientDataHandler.WorldID];
            }

            Main.SelectPlayer(player);
            Log.Info($"Autoload using ClientDataHandler. Starting game with Player: {player.Name}, World: {world.Name}");
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
            // }
        }
    }
}