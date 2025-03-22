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
    public class HitboxSystem : ModSystem
    {
        private static bool drawPlayerHitbox = false;
        private static bool drawNPCHitbox = false;
        private static bool drawProjAndMeleeHitbox = false;

        public static void ToggleAllHitboxes()
        {
            drawPlayerHitbox = !drawPlayerHitbox;
            drawNPCHitbox = !drawNPCHitbox;
            drawProjAndMeleeHitbox = !drawProjAndMeleeHitbox;
        }

        public override void PostDrawInterface(SpriteBatch sb)
        {
            base.PostDrawInterface(sb);
            if (!drawPlayerHitbox && !drawNPCHitbox && !drawProjAndMeleeHitbox) return;

            RestartSB(sb);
            if (drawPlayerHitbox) DrawPlayerHitbox(sb);
            if (drawNPCHitbox) DrawNPCHitboxes(sb);
            if (drawProjAndMeleeHitbox)
            {
                DrawProjectileHitboxes(sb);
                DrawPlayerMeleeHitboxes(sb);
            }
        }

        private void RestartSB(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
        }

        private void DrawPlayerHitbox(SpriteBatch spriteBatch)
        {
            Player p = Main.LocalPlayer;
            Rectangle hitbox = p.Hitbox;
            hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
            Color lightOrange = new(255, 113, 69);
            DrawHitbox(spriteBatch, hitbox, lightOrange);
            DrawOutlineHitbox(spriteBatch, hitbox, lightOrange);
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
                    Color red = new(226, 57, 39);
                    DrawHitbox(spriteBatch, hitbox, red);
                    DrawOutlineHitbox(spriteBatch, hitbox, red);
                }
            }
        }

        private void DrawProjectileHitboxes(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || proj.type == ProjectileID.None) continue;

                Rectangle hitbox = proj.getRect();
                hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
                Color pink = new(255, 42, 156);
                DrawHitbox(spriteBatch, hitbox, pink);
                DrawOutlineHitbox(spriteBatch, hitbox, pink);
            }
        }

        private void DrawPlayerMeleeHitboxes(SpriteBatch sb)
        {
            for (int i = 0; i < 256; i++)
            {
                if (HitboxesGlobalItem.meleeHitbox[i].HasValue)
                {
                    Rectangle hitbox = HitboxesGlobalItem.meleeHitbox[i].Value;
                    hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
                    hitbox = Main.ReverseGravitySupport(hitbox);
                    Color yellow = Color.Yellow;
                    DrawHitbox(sb, hitbox, yellow);
                    DrawOutlineHitbox(sb, hitbox, yellow);
                    HitboxesGlobalItem.meleeHitbox[i] = null;
                }
            }
        }

        private void DrawHitbox(SpriteBatch spriteBatch, Rectangle hitbox, Color color)
        {
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, color * 0.3f);
        }

        private void DrawOutlineHitbox(SpriteBatch spriteBatch, Rectangle hitbox, Color color)
        {
            hitbox.Inflate(2, 2);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X, hitbox.Y, hitbox.Width, 2), color);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X, hitbox.Y, 2, hitbox.Height), color);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X + hitbox.Width - 2, hitbox.Y, 2, hitbox.Height), color);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X, hitbox.Y + hitbox.Height - 2, hitbox.Width, 2), color);
        }

        internal class HitboxesGlobalItem : GlobalItem
        {
            internal static Rectangle?[] meleeHitbox = new Rectangle?[256];

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
