using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace EliteTestingMod.Common.Players
{
    /// <summary>
    /// Allows the player to move through blocks and fly around the world.
    /// Hold shift to move faster by modifying the speed by 2x.
    /// </summary>
    public class Noclip : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            base.ProcessTriggers(triggersSet);

            if (PlayerCheatManager.Noclip)
            {
                Player.gravity = 0f;
                Player.controlJump = false;
                Player.noFallDmg = true;
                Player.moveSpeed = 0f;
                Player.noKnockback = true;
                Player.velocity.Y = -1E-11f; // prevent falling?
                float modifier = 1f;

                if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift))
                {
                    modifier += 3f;
                }
                if (Main.keyState.IsKeyDown(Keys.Space) || Main.keyState.IsKeyDown(Keys.Space))
                {
                    modifier += 8f;
                }

                if (Main.keyState.IsKeyDown(Keys.W))
                {
                    Player.position.Y -= 8f * modifier;
                }
                if (Main.keyState.IsKeyDown(Keys.S))
                {
                    Player.position.Y += 8f * modifier;
                }
                if (Main.keyState.IsKeyDown(Keys.A))
                {
                    Player.position.X -= 8f * modifier;
                }
                if (Main.keyState.IsKeyDown(Keys.D))
                {
                    Player.position.X += 8f * modifier;
                }
            }
        }
    }
}
