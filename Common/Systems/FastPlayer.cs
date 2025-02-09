using SquidTestingMod.Helpers;
using SquidTestingMod.UI;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Systems
{
    /// <summary>
    /// Good code:
    /// https://github.com/ScalarVector1/DragonLens/blob/1b2ca47a5a4d770b256fdffd5dc68c0b4d32d3b2/Content/Tools/Gameplay/InfiniteReach.cs#L58
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class FastPlayer : ModPlayer
    {
        private bool IsFastModeEnabled()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            // Use a null-safe check if needed.
            return sys?.mainState?.godButton?.IsFastMode ?? false;
        }

        // 1) Infinite Flight:
        // In ResetEffects (which runs every tick), reset the wing time.
        public override void ResetEffects()
        {
            if (!IsFastModeEnabled())
                return;

            // By constantly setting wingTime to a huge value, you effectively allow infinite flight.
            Player.wingTime = int.MaxValue;
            Player.wingTimeMax = int.MaxValue;
        }

        private float flightTimer = 0f;
        // 2), 3), 4) and 5) in PostUpdate:
        public override void PostUpdate()
        {
            if (!IsFastModeEnabled())
                return;

            // 3) Set running speed to a fixed value.
            // (This value sets the maximum running speed.)
            Player.maxRunSpeed = 5f;  // Adjust this value for your desired ground speed.

            // 2) Instant acceleration for running.
            if (Player.controlLeft)
                Player.velocity.X = -Player.maxRunSpeed;
            else if (Player.controlRight)
                Player.velocity.X = Player.maxRunSpeed;

            // 4) Increase flight speed by 2x
            if (Player.wingTime > 0 && (Player.controlLeft || Player.controlRight))
            {
                Player.velocity.X *= 2f;
            }

            // Vertical acceleration.
            if (Player.wingTime > 0 && Player.controlUp)
            {
                Player.velocity.Y -= 0.5f;
            }

            // Downward acceleration.
            if (Player.controlDown)
            {
                // Add to Y velocity to accelerate downward faster.
                Player.velocity.Y += 0.5f; // Adjust value for desired acceleration
            }

            // 5) Enable hover when holding down.
            if (Player.controlDown && Player.controlJump)
            {
                Player.velocity.Y = 0f;
            }
            else if (Player.controlDown)
            {
                Player.velocity.Y += 3f; // Move down
            }

            if (Player.controlJump && Player.wingTime > 0)
            {
                // Increment flight timer.
                // (Assume 60 FPS; alternatively, use your own time delta if available.)
                flightTimer += 1f / 60f;

                // If flying for 2 seconds or more, boost horizontal speed.
                if (flightTimer >= 2f)
                {
                    Player.velocity.X *= 5f;
                }
                else if (flightTimer >= 1f)
                {
                    Player.velocity.X *= 1.5f;
                }
            }
            else
            {
                // Reset timer if not flying.
                flightTimer = 0f;
            }
        }

        public override void UpdateEquips()
        {
            if (!IsFastModeEnabled())
                return;

            // Disable smart cursor
            if (Main.SmartCursorWanted)
            {
                Main.SmartCursorWanted_Mouse = false;
                Main.SmartCursorWanted_GamePad = false;
                Main.NewText("Smart cursor is disabled with fast building");
            }

            // Set infinite range
            Player.tileRangeX = int.MaxValue;
            Player.tileRangeY = int.MaxValue;
            Player.blockRange = int.MaxValue; // Ensures unrestricted tile placement
        }

        public override float UseTimeMultiplier(Item item)
        {
            if (!IsFastModeEnabled())
                return 1;

            // Fast speed for tools
            if (item.createTile != -1 || item.createWall != -1 || item.pick > 0 || item.axe > 0 || item.hammer > 0)
                return 0.01f; // Near-instant speed

            return 1;
        }

        public override float UseAnimationMultiplier(Item item)
        {
            if (!IsFastModeEnabled())
                return 1;

            // If the item is a tile, wall, or tool, make it near-instant
            if (item.createTile != -1 || item.createWall != -1 || item.pick > 0 || item.axe > 0 || item.hammer > 0)
                return 0.01f; // Near-instant speed

            return 1;
        }
    }
}
