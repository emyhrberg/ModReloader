using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ExampleMod.Systems
{
    public class ProjectileHitboxSystem : ModSystem
    {
        public override void Load()
        {
            // Hook into the game's interface drawing.
            Terraria.On_Main.DrawInterface += DrawProjectileHitboxes;
        }

        public override void Unload()
        {
            // Unhook when unloading to avoid potential issues.
            Terraria.On_Main.DrawInterface -= DrawProjectileHitboxes;
        }

        private void DrawProjectileHitboxes(Terraria.On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            // Begin a new sprite batch with the world transformation matrix.
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            // Iterate over all projectiles and draw their hitboxes if active.
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active)
                {
                    // Use the projectile's Hitbox, which is in world coordinates.
                    Rectangle hitbox = proj.Hitbox;
                    Log.Info("Drawing hitbox for projectile " + proj.type);
                    // Draw the hitbox with a semi-transparent red color.
                    Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, Color.Red * 0.5f);
                }
            }

            Main.spriteBatch.End();

            // Continue with the original interface drawing.
            orig(self, gameTime);
        }
    }
}
