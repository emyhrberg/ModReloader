using System;
using System.Linq;
using System.Reflection;
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

        public override void OnModLoad()
        {
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
                Mod.Logger.Warn("Failed to access OnSuccessfulLoad field.");
            }
        }

        public override void Unload()
        {
            // Reset some hooks
            typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static)?.SetValue(null, null);
        }

        private void EnterSingleplayerWorld()
        {
            Mod.Logger.Info("EnterSingleplayerWorld() called!");

            // Loading lists of Players and Worlds
            Main.LoadWorlds();
            Main.LoadPlayers();

            if (Main.PlayerList.Count == 0 || Main.WorldList.Count == 0)
                throw new Exception("No players or worlds found.");

            // Getting Player and World from ClientDataHandler
            // var player = Main.PlayerList[ClientDataHandler.PlayerID];
            // var world = Main.WorldList[ClientDataHandler.WorldID];
            var player = Main.PlayerList.FirstOrDefault();
            var world = Main.WorldList.FirstOrDefault();

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
    }
}