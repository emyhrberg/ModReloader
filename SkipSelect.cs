using System;
using System.Linq;
using System.Reflection;
using Steamworks;
using Terraria;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.ModLoader;

namespace SkipSelect
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class SkipSelect : Mod
	{

        // variables
        private MethodInfo CanWorldBePlayedMethod;

        public override void Load()
        {
            
            base.Load();

            // Hook into OnSuccessfulLoad
            // This is called after all mods are loaded and the game is about to start
            // This works by using reflection.
            // More specifically by setting the value of a private field in the ModLoader class
            typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static)
                ?.SetValue(null, (Action)SkipSelectFunction);

            // Hook into CanWorldBePlayed
            CanWorldBePlayedMethod = typeof(UIWorldSelect).GetMethod("CanWorldBePlayed", BindingFlags.NonPublic | BindingFlags.Static);
        }

        private void SkipSelectFunction()
        {
            Logger.Info("SkipSelectFunction called!");

            // load worlds and players
            Main.LoadWorlds();
            Main.LoadPlayers();

            // select first player
            var selectedPlayer = Main.PlayerList.FirstOrDefault();
            selectedPlayer.SetAsActive();

            // select first world
            var selectedWorld = Main.WorldList.FirstOrDefault();
            selectedWorld.SetAsActive();

            // print entire playerlist and worldlist
            Logger.Info("PlayerList count: " + Main.PlayerList.Count);
            foreach (var player in Main.PlayerList)
            {
                if (player.IsFavorite)
                {
                    Logger.Info("Favorite player: " + player.Name);
                    selectedPlayer = player;
                    player.SetAsActive();
                }
            }

            Logger.Info("WorldList count: " + Main.WorldList.Count);
            foreach (var world in Main.WorldList)
            {
                if (world.IsFavorite)
                {
                    Logger.Info("Favorite world: " + world.Name);
                    selectedWorld = world;
                    world.SetAsActive();
                }
                Logger.Info("World: " + world.Name);
            }

            Main.SelectPlayer(selectedPlayer);

            bool canBePlayed = (bool)CanWorldBePlayedMethod?.Invoke(null, new object[] { selectedWorld });
            Logger.Info("CanWorldBePlayed: " + canBePlayed);

            if (canBePlayed)
            {
                Logger.Info($"Playing world with Selected player: '{selectedPlayer.Name}' and Selected world: '{selectedWorld.Name}'");

                //Main.menuMode = 10;
                WorldGen.playWorld();
            }
            else
            {

                string log = "\n" +
                    $"Error: The first player '{selectedPlayer.Name}' and first world '{selectedWorld.Name}' are incompatible.\n" +
                    "Please change your favorite player or world.";

                Logger.Info(log);
                throw new Exception(log);
            }
        }

    }
}
