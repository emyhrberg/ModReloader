// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Input;
// using SquidTestingMod.Common.Configs;
// using SquidTestingMod.Helpers;
// using Terraria;
// using Terraria.DataStructures;
// using Terraria.ModLoader;

// namespace SquidTestingMod.Common.Systems
// {
//     /// <summary>
//     /// Gives player abilities for:
//     /// GodMode - Makes the player invincible
//     /// FastMode - Increases player speed
//     /// BuildMode - Infinite range, instant mining and more
//     /// NoClip - Move through blocks
//     /// </summary>
//     [Autoload(Side = ModSide.Client)]
//     public class PlayerCheats : ModPlayer
//     {
//         // Variables section
//         public static bool IsGodModeOn = false;
//         public static bool IsFastModeOn = false;
//         public static bool IsBuildModeOn = false;
//         public static bool IsNoClipOn = false;
//         public static bool IsLightModeOn = false;
//         public static bool IsTeleportModeOn = false;
//         public static bool IsNoGravityOn = false;
//         public static bool IsPlaceFasterOn = false;
//         public static bool IsMineFasterOn = false;
//         public static bool IsInvisibleToEnemiesOn = false;

//         #region Toggle values
//         private static void ToggleCheat<T>(ref T value, string name) where T : struct
//         {
//             value = value is bool b ? (T)(object)!b : value;
//             if (Conf.ShowCombatTextOnToggle)
//             {
//                 bool isOn = value is bool bVal && bVal;
//                 CombatText.NewText(
//                     Main.LocalPlayer.getRect(),
//                     isOn ? Color.Green : Color.Red,
//                     $"{name} {(isOn ? "On" : "Off")}");
//             }
//         }

//         public static void ToggleMoveFaster()
//         {
//             ToggleCheat(ref IsFastModeOn, "Fast Mode");
//             if (IsFastModeOn)
//             {
//                 var cloudJump = new CloudInABottleJump();
//                 bool playSound = true;
//                 cloudJump.OnStarted(Main.LocalPlayer, ref playSound);
//                 cloudJump.ShowVisuals(Main.LocalPlayer);
//             }
//         }

//         public static void ToggleGod() => ToggleCheat(ref IsGodModeOn, "God Mode");
//         public static void ToggleBuildMode() => ToggleCheat(ref IsBuildModeOn, "Build Mode");
//         public static void ToggleNoClip() => ToggleCheat(ref IsNoClipOn, "NoClip");
//         public static void ToggleLightMode() => ToggleCheat(ref IsLightModeOn, "Light Mode");
//         public static void ToggleTeleportMode() => ToggleCheat(ref IsTeleportModeOn, "Teleport Mode");
//         public static void ToggleNoGravity() => ToggleCheat(ref IsNoGravityOn, "No Gravity");
//         public static void TogglePlaceFaster() => ToggleCheat(ref IsPlaceFasterOn, "Place Faster");
//         public static void ToggleMineFaster() => ToggleCheat(ref IsMineFasterOn, "Mine Faster");
//         public static void ToggleInvisibleToEnemies() => ToggleCheat(ref IsInvisibleToEnemiesOn, "Invisible To Enemies");

//         #endregion

//         // Hooks start here

//         public override void Load()
//         {
//             // base.Load(); (This line is commented out and fine to leave as-is)

//             // Hook into the player's tile and wall placement events
//             // for placing tiles anywhere (not just near the player)
//             On_Player.PlaceThing_Tiles += PlaceThing_Tiles;
//             On_Player.PlaceThing_Walls += PlaceThing_Walls;
//         }

//         public override void Unload()
//         {
//             On_Player.PlaceThing_Tiles -= PlaceThing_Tiles;
//             On_Player.PlaceThing_Walls -= PlaceThing_Walls;
//         }

//         private void PlaceThing_Tiles(On_Player.orig_PlaceThing_Tiles orig, Player player)
//         {
//             // Only apply when build mode is on
//             if (IsBuildModeOn && player == Main.LocalPlayer)
//             {
//                 Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
//                 ushort wallType = tile.WallType;
//                 tile.WallType = 4; // Temporarily set wall to wood to allow placement
//                 orig(player);
//                 tile.WallType = wallType; // Restore original wall
//             }
//             else
//             {
//                 orig(player);
//             }
//         }

//         private void PlaceThing_Walls(On_Player.orig_PlaceThing_Walls orig, Player player)
//         {
//             // Only apply when build mode is on
//             if (IsBuildModeOn && player == Main.LocalPlayer)
//             {
//                 Tile tile = Framing.GetTileSafely(Player.tileTargetX - 1, Player.tileTargetY);
//                 bool hasTile = tile.HasTile;
//                 tile.HasTile = true; // Temporarily set tile to true to allow wall placement
//                 orig(player);
//                 tile.HasTile = hasTile; // Restore original tile state
//             }
//             else
//             {
//                 orig(player);
//             }
//         }

//         #region GodMode
//         public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
//         {
//             if (IsGodModeOn)
//                 return true; // Immune to all damage
//             return false;
//         }

//         public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
//         {
//             if (IsGodModeOn)
//             {
//                 // Don't kill the player
//                 Player.statLife = Player.statLifeMax2;
//                 return false;
//             }
//             return true;
//         }
//         #endregion

//         #region All cheats in postupdate
//         public override void PostUpdate()
//         {
//             Log.Info("PlayerCheats PostUpdate");

//             if (IsGodModeOn)
//                 Player.statLife = Player.statLifeMax2; // Keep player at max health

//             if (IsNoGravityOn)
//             {
//                 Player.gravity = 0f;
//                 Player.controlJump = false;
//                 Player.noFallDmg = true;
//                 Player.noKnockback = true;
//                 Player.velocity.Y = -1E-11f;

//                 float modifier = 1f;
//                 if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift))
//                 {
//                     modifier = 2f;
//                 }
//                 if (Main.keyState.IsKeyDown(Keys.LeftControl) || Main.keyState.IsKeyDown(Keys.RightControl))
//                 {
//                     modifier = 0.5f;
//                 }
//                 if (Main.keyState.IsKeyDown(Keys.W))
//                 {
//                     Player.position.Y -= 8f * modifier;
//                 }
//                 if (Main.keyState.IsKeyDown(Keys.S))
//                 {
//                     Player.position.Y += 8f * modifier;
//                 }
//                 if (Main.keyState.IsKeyDown(Keys.A))
//                 {
//                     Player.position.X -= 8f * modifier;
//                 }
//                 if (Main.keyState.IsKeyDown(Keys.D))
//                 {
//                     Player.position.X += 8f * modifier;
//                 }
//             }

//             if (IsFastModeOn)
//             {
//                 // Increase player speed and acceleration
//                 // default is:
//                 // maxRunSpeed = 3f, runAcceleration = 0.08f, runSlowdown = 0.2f, 
//                 // moveSpeed = 1f where e.g += 0.25f is swiftness potion
//                 // Log.Info($"Fast: maxRunSpeed: {Player.maxRunSpeed}, runAcceleration: {Player.runAcceleration}, moveSpeed: {Player.moveSpeed}");

//                 Player.moveSpeed += 2.25f;
//                 Player.maxRunSpeed = 50f; // 50f is very fast, 15-20f is kinda fast
//                 Player.runAcceleration *= 3.5f;
//                 Player.runSlowdown *= 0.5f;

//                 // Instant acceleration for running.
//                 // Set this to around 5f for fast or 10f for instant (10f is too fast imo)
//                 if (Player.controlLeft)
//                     Player.velocity.X = -Player.maxRunSpeed * 0.33f;
//                 else if (Player.controlRight)
//                     Player.velocity.X = Player.maxRunSpeed * 0.33f;

//                 // When holding jump, make the player fly
//                 if (Player.controlJump)
//                     Player.velocity.Y -= 0.5f;
//             }

//             if (IsNoClipOn)
//             {
//                 Vector2 desiredPos = Player.Center;

//                 // Handle movement input
//                 if (Player.controlLeft)
//                     desiredPos.X -= 15;
//                 if (Player.controlRight)
//                     desiredPos.X += 15;
//                 if (Player.controlUp)
//                     desiredPos.Y -= 15;
//                 if (Player.controlDown)
//                     desiredPos.Y += 15;

//                 // Apply position and disable velocity
//                 Player.Center = desiredPos;
//                 Player.velocity = Vector2.Zero;
//                 Player.gfxOffY = 0;
//             }

//             if (IsTeleportModeOn && Main.mouseRight)
//             {
//                 Main.LocalPlayer.Teleport(Main.MouseWorld);
//             }
//         }
//         #endregion

//         #region Build mode
//         public override void UpdateEquips()
//         {
//             if (IsBuildModeOn)
//             {
//                 // Disable smart cursor
//                 if (Main.SmartCursorWanted)
//                 {
//                     Main.SmartCursorWanted_Mouse = false;
//                     Main.SmartCursorWanted_GamePad = false;
//                     Main.NewText("Smart Cursor Disabled in Build Mode");
//                 }

//                 // Set infinite range
//                 Player.tileRangeX = int.MaxValue;
//                 Player.tileRangeY = int.MaxValue;
//                 Player.blockRange = int.MaxValue; // Ensures unrestricted tile placement
//             }
//         }

//         public override float UseTimeMultiplier(Item item)
//         {
//             // Fast speed for tools
//             if (IsBuildModeOn && (item.createTile != -1 || item.createWall != -1 || item.pick > 0 || item.axe > 0 || item.hammer > 0))
//                 return 0.01f; // Near-instant speed

//             // Default speed
//             return 1;
//         }

//         public override float UseAnimationMultiplier(Item item)
//         {
//             if (!IsBuildModeOn)
//                 return 1;

//             // If the item is a tile, wall, or tool, make it near-instant
//             if (item.createTile != -1 || item.createWall != -1 || item.pick > 0 || item.axe > 0 || item.hammer > 0)
//                 return 0.01f; // Near-instant speed

//             return 1;
//         }
//         #endregion
//     }
// }