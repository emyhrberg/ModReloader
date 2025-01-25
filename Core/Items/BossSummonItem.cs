// using Terraria;
// using Terraria.Audio;
// using Terraria.ID;
// using Terraria.ModLoader;

// namespace SkipSelect.Core.System
// {
//     public class BossSummonItem : ModItem
//     {
//         public override void SetStaticDefaults()
//         {
//             Item.ResearchUnlockCount = 3;
//             ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
//         }

//         public override void SetDefaults()
//         {
//             Item.width = 20;
//             Item.height = 20;
//             Item.maxStack = 20;
//             Item.rare = ItemRarityID.Blue;
//             Item.useAnimation = 30;
//             Item.useTime = 30;
//             Item.consumable = true;
//             Item.useStyle = ItemUseStyleID.HoldUp;
//             Item.value = Item.buyPrice(gold: 1);
//         }

//         // Only allow use if King Slime isn't already present.
//         public override bool CanUseItem(Player player) =>
//             !NPC.AnyNPCs(NPCID.KingSlime);

//         public override bool? UseItem(Player player)
//         {
//             if (player.whoAmI != Main.myPlayer)
//                 return false;

//             // Play sound at player's position.
//             SoundEngine.PlaySound(SoundID.Roar, player.position);

//             // In single player spawn directly at player's center.
//             if (Main.netMode != NetmodeID.MultiplayerClient)
//             {
//                 NPC.NewNPC(player.GetSource_ItemUse(Item), (int)player.Center.X, (int)player.Center.Y, NPCID.KingSlime);
//             }
//             else // In multiplayer, send a message to spawn the boss.
//             {
//                 NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, number: player.whoAmI, number2: NPCID.KingSlime);
//             }
//             return true;
//         }
//     }
// }
