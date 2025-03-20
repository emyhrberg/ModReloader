using System;
using System.Reflection;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;

namespace SquidTestingMod.Reload
{
    /// <summary>
    /// Automatically loads a saved player and world after mods have been reloaded.
    /// Gets data from <see cref="ReloadUtils"/>
    /// </summary>
    public class ReloadSystem : ModSystem
    {
        public override void OnModLoad()
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Mod.Logger.Info("ReloadSystem disabled in multiplayer.");
                return;
            }

            // Get the private OnSuccessfulLoad field via reflection.
            FieldInfo onSuccessfulLoadField = typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static);
            Action onSuccessfulLoad = onSuccessfulLoadField?.GetValue(null) as Action ?? (() => { });

            // Attach our callback that loads the saved player and world.
            onSuccessfulLoad += EnterSingleplayerWorld;
            onSuccessfulLoadField.SetValue(null, onSuccessfulLoad);
        }

        public override void Unload()
        {
            // Reset the OnSuccessfulLoad hook.
            typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static)?.SetValue(null, null);
        }

        /// <summary>
        /// Called after mods have successfully loaded to start the saved singleplayer world.
        /// </summary>
        private void EnterSingleplayerWorld()
        {
            // Refresh player and world lists.
            Main.LoadWorlds();
            Main.LoadPlayers();

            if (Main.PlayerList.Count == 0 || Main.WorldList.Count == 0)
                throw new Exception("No players or worlds found.");

            // Use ReloadUtils to retrieve saved indices.
            var player = Main.PlayerList[ReloadUtils.PlayerId];
            var world = Main.WorldList[ReloadUtils.WorldId];

            // Start game with pair
            Main.SelectPlayer(player);
            Log.Info($"Starting game with Player: {player.Name}, World: {world.Name}");
            Main.ActiveWorldFileData = world;

            if (string.IsNullOrEmpty(world.Path))
            {
                Mod.Logger.Error($"World {world.Name} has an invalid or null path.");
                throw new ArgumentNullException(nameof(world.Path), "World path cannot be null or empty.");
            }
            WorldGen.playWorld();
        }
    }
}
