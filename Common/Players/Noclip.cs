using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SquidTestingMod.Common.Configs;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    /// <summary>
    /// Allows the player to move through blocks and fly around the world.
    /// Hold shift to move faster by modifying the speed by 2x.
    /// Holding shift for 5 seconds will increase the speed by 2x again.
    /// </summary>
    public class Noclip : ModPlayer
    {
        private int startTime = 0;
        private bool speedIncreased = false;

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            base.ProcessTriggers(triggersSet);

            if (PlayerCheatManager.Noclip)
            {
                if (startTime == 0)
                {
                    startTime = (int)Main.GameUpdateCount;
                }

                Player.gravity = 0f;
                Player.controlJump = false;
                Player.noFallDmg = true;
                Player.moveSpeed = 0f;
                Player.noKnockback = true;
                Player.velocity.Y = -1E-11f;
                float modifier = 1f;

                if (Main.keyState.IsKeyDown((Keys)160) || Main.keyState.IsKeyDown((Keys)161))
                {
                    modifier = 2f;
                }
                if (Main.keyState.IsKeyDown((Keys)162) || Main.keyState.IsKeyDown((Keys)163))
                {
                    modifier = 0.5f;
                }
                if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift))
                {
                    modifier *= 2f;
                }

                if ((Main.GameUpdateCount - startTime) >= 300 && !speedIncreased) // 300 frames = 5 seconds
                {
                    modifier *= 2f;
                    speedIncreased = true;
                }

                if (Main.keyState.IsKeyDown((Keys)87))
                {
                    Player.position.Y -= 8f * modifier;
                }
                if (Main.keyState.IsKeyDown((Keys)83))
                {
                    Player.position.Y += 8f * modifier;
                }
                if (Main.keyState.IsKeyDown((Keys)65))
                {
                    Player.position.X -= 8f * modifier;
                }
                if (Main.keyState.IsKeyDown((Keys)68))
                {
                    Player.position.X += 8f * modifier;
                }
            }
            else
            {
                startTime = 0;
                speedIncreased = false;
            }
        }
    }
}
