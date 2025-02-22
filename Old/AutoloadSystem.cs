// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Reflection;
// using SquidTestingMod.Common.Configs;
// using SquidTestingMod.Helpers;
// using Terraria;
// using Terraria.GameContent.UI.States;
// using Terraria.ID;
// using Terraria.IO;
// using Terraria.ModLoader;

// namespace SquidTestingMod.Common.Systems
// {
//     /// <summary>
//     /// This system will automatically load a player and world every time AFTER all the mods have been reloaded.
//     /// Meaning in OnModLoad. This is useful for testing mods in singleplayer.
//     /// </summary>
//     [Autoload(Side = ModSide.Client)]
//     public class AutoloadSingleplayerSystem : ModSystem
//     {
//         private MethodInfo canWorldBePlayedMethod;

//         public override void OnModLoad()
//         {
//             if (Main.netMode == NetmodeID.MultiplayerClient)
//             {
//                 Mod.Logger.Info("AutoloadSingleplayerSystem is disabled in multiplayer.");
//                 return;
//             }

//             // Get the OnSuccessfulLoad field using reflection
//             FieldInfo onSuccessfulLoadField = typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static);

//             // Get the CanWorldBePlayed method using reflection
//             // TODO ACTUALLY USE THIS? CURRENTLY UNUSED
//             canWorldBePlayedMethod = typeof(UIWorldSelect).GetMethod("CanWorldBePlayed", BindingFlags.NonPublic | BindingFlags.Static);

//             if (onSuccessfulLoadField != null)
//             {
//                 Action onSuccessfulLoad = (Action)onSuccessfulLoadField.GetValue(null);
//                 Config c = ModContent.GetInstance<Config>();

//                 if (true)
//                 // if (ClientDataHandler.Mode == ClientMode.SinglePlayer)
//                 {
//                     onSuccessfulLoad += EnterSingleplayerWorld;
//                 }
//                 else
//                 {
//                     onSuccessfulLoad += EnterMultiplayerWorld;
//                 }
//                 // Set the modified delegate back to the field
//                 onSuccessfulLoadField.SetValue(null, onSuccessfulLoad);
//             }
//             else
//             {
//                 Mod.Logger.Warn("Failed to access OnSuccessfulLoad field.");
//             }
//         }

//         public override void Unload() // reset some hooks
//         {
//             typeof(ModLoader).GetField("OnSuccessfulLoad", BindingFlags.NonPublic | BindingFlags.Static)?.SetValue(null, null);
//             canWorldBePlayedMethod = null;
//         }

//         private void EnterSingleplayerWorld()
//         {
//             Mod.Logger.Info("EnterSingleplayerWorld() called!");

//             Main.LoadWorlds();
//             Main.LoadPlayers();

//             if (Main.PlayerList.Count == 0 || Main.WorldList.Count == 0)
//                 throw new Exception("No players or worlds found.");

//             // var player = Main.PlayerList[ClientDataHandler.PlayerId];
//             // var world = Main.WorldList[ClientDataHandler.WorldId];

//             var player = Main.PlayerList.FirstOrDefault();
//             var world = Main.WorldList.FirstOrDefault();

//             Main.SelectPlayer(player);
//             Mod.Logger.Info($"Starting game with Player: {player.Name}, World: {world.Name}");
//             Main.ActiveWorldFileData = world;

//             // Ensure the world's file path is valid
//             if (string.IsNullOrEmpty(world.Path))
//             {
//                 Mod.Logger.Error($"World {world.Name} has an invalid or null path.");
//                 throw new ArgumentNullException(nameof(world.Path), "World path cannot be null or empty.");
//             }
//             // Play the selected world in singleplayer
//             WorldGen.playWorld();
//         }

//         private void EnterMultiplayerWorld()
//         {
//             // TODO
//         }
//     }
// }