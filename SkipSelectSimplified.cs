//using System;
//using System.Linq;
//using System.Reflection;
//using Steamworks;
//using Terraria;
//using Terraria.GameContent.UI.States;
//using Terraria.ModLoader;

//namespace SkipSelect
//{
//	public class SkipSelectSimplified : Mod
//	{

//        public override void Load()
//        {
            
//            base.Load();

//            // Hook into OnSuccessfulLoad
//            // This is called after all mods are loaded and the game is about to start
//            // This works by using reflection.
//            // More specifically by setting the value of a private field in the ModLoader class
//            typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static)
//                ?.SetValue(null, (Action)SkipSelectFunction);
//        }

//        private void SkipSelectFunction()
//        {
//            Logger.Info("SkipSelectFunction called!");

//            // load worlds and players
//            WorldGen.clearWorld();
//            Main.LoadWorlds();
//            Main.LoadPlayers();

//            // select first player and first world
//            var firstPlayer = Main.PlayerList.FirstOrDefault();
//            var firstWorld = Main.WorldList.FirstOrDefault();
//            Main.SelectPlayer(firstPlayer);
//            firstPlayer.SetAsActive();
//            firstWorld.SetAsActive();

//            // play world
//            //WorldGen.playWorld();
//        }
//    }
//}
