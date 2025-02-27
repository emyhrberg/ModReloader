using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.UI;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Systems
{
    /// <summary>
    /// Good code:
    /// https://github.com/JavidPack/ModdersToolkit/blob/1.4.4/Tools/Hitboxes/HitboxesTool.cs#L137
    /// </summary>
    public class HitboxSystem : ModSystem
    {
        private bool drawHitboxFlag = false;

        public void ToggleHitboxes()
        {
            drawHitboxFlag = !drawHitboxFlag;

            if (Conf.ShowCombatTextOnToggle)
                CombatText.NewText(Main.LocalPlayer.getRect(), drawHitboxFlag ? Color.Green : Color.Red, drawHitboxFlag ? "Hitboxes ON" : "Hitboxes OFF");
        }

        public override void PostDrawInterface(SpriteBatch sb)
        {
            base.PostDrawInterface(sb);

            if (!drawHitboxFlag)
                return;

            RestartSB(sb);
            DrawPlayerHitbox(sb);
            DrawNPCHitboxes(sb);
            DrawProjectileHitboxes(sb);
            DrawPlayerMeleeHitboxes(sb);
        }

        private void RestartSB(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix); ;
        }

        private void DrawPlayerHitbox(SpriteBatch spriteBatch)
        {
            // Get hitbox of player
            Player p = Main.LocalPlayer;
            Rectangle hitbox = p.Hitbox;
            hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);

            // Draw hitbox
            DrawHitbox(spriteBatch, hitbox, Color.Blue);
            DrawOutlineHitbox(spriteBatch, hitbox);
        }

        private void DrawNPCHitboxes(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active)
                {
                    Rectangle hitbox = npc.getRect();
                    hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
                    hitbox = Main.ReverseGravitySupport(hitbox);

                    // Draw NPC hitbox (semi-transparent red)
                    DrawHitbox(spriteBatch, hitbox, Color.Red * 0.5f);
                    DrawOutlineHitbox(spriteBatch, hitbox);
                }
            }
        }

        private void DrawProjectileHitboxes(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || proj.type == ProjectileID.None) continue;

                // Get projectile hitbox and offset it
                Rectangle hitbox = proj.getRect();
                hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);

                // Draw projectile hitbox (semi-transparent green)
                DrawHitbox(spriteBatch, hitbox, Color.Green * 0.5f);
                DrawOutlineHitbox(spriteBatch, hitbox);
            }
        }


        private void DrawHitbox(SpriteBatch spriteBatch, Rectangle hitbox, Color color)
        {
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, color * 0.5f);
        }

        private void DrawOutlineHitbox(SpriteBatch spriteBatch, Rectangle hitbox)
        {
            // draw a outline 2 pixels thick around the hitbox
            hitbox.Inflate(2, 2);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X, hitbox.Y, hitbox.Width, 2), Color.Black);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X, hitbox.Y, 2, hitbox.Height), Color.Black);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X + hitbox.Width - 2, hitbox.Y, 2, hitbox.Height), Color.Black);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X, hitbox.Y + hitbox.Height - 2, hitbox.Width, 2), Color.Black);
        }

        private void DrawPlayerMeleeHitboxes(SpriteBatch sb)
        {
            for (int i = 0; i < 256; i++)
            {
                //if (hitboxesGlobalItem.meleeHitbox[i].HasValue)
                if (HitboxesGlobalItem.meleeHitbox[i].HasValue)
                {
                    Rectangle hitbox = HitboxesGlobalItem.meleeHitbox[i].Value;
                    hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
                    hitbox = Main.ReverseGravitySupport(hitbox);
                    DrawHitbox(sb, hitbox, Color.Yellow * 0.5f);
                    DrawOutlineHitbox(sb, hitbox);
                    HitboxesGlobalItem.meleeHitbox[i] = null;
                }
            }
        }

        // thanks jovidpack
        internal class HitboxesGlobalItem : GlobalItem
        {
            internal static Rectangle?[] meleeHitbox = new Rectangle?[256];
            // Is this ok to load in server?
            public override void UseItemHitbox(Item item, Player player, ref Rectangle hitbox, ref bool noHitbox)
            {
                meleeHitbox[player.whoAmI] = hitbox;
            }

            public override void PostUpdate(Item item)
            {
                base.PostUpdate(item);
            }
        }
    }
}
