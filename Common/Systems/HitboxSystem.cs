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
        private bool drawPlayerHitbox = false;
        private bool drawNPCHitbox = false;
        private bool drawProjAndMeleeHitbox = false;

        public void TogglePlayerHitboxes() => ToggleHitbox(ref drawPlayerHitbox, "Player Hitboxes");
        public void ToggleNPCHitboxes() => ToggleHitbox(ref drawNPCHitbox, "NPC Hitboxes");
        public void ToggleProjAndMeleeHitboxes() => ToggleHitbox(ref drawProjAndMeleeHitbox, "Projectile and Melee Hitboxes");

        private void ToggleHitbox(ref bool flag, string hitboxType)
        {
            flag = !flag;
            if (Conf.ShowCombatTextOnToggle)
                CombatText.NewText(Main.LocalPlayer.getRect(), flag ? Color.Green : Color.Red, flag ? $"{hitboxType} ON" : $"{hitboxType} OFF");
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

                Rectangle hitbox = proj.getRect();
                hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
                DrawHitbox(spriteBatch, hitbox, Color.Green * 0.5f);
                DrawOutlineHitbox(spriteBatch, hitbox);
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
                    DrawHitbox(sb, hitbox, Color.Yellow * 0.5f);
                    DrawOutlineHitbox(sb, hitbox);
                    HitboxesGlobalItem.meleeHitbox[i] = null;
                }
            }
        }

        private void DrawHitbox(SpriteBatch spriteBatch, Rectangle hitbox, Color color)
        {
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, color * 0.5f);
        }

        private void DrawOutlineHitbox(SpriteBatch spriteBatch, Rectangle hitbox)
        {
            hitbox.Inflate(2, 2);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X, hitbox.Y, hitbox.Width, 2), Color.Black);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X, hitbox.Y, 2, hitbox.Height), Color.Black);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X + hitbox.Width - 2, hitbox.Y, 2, hitbox.Height), Color.Black);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(hitbox.X, hitbox.Y + hitbox.Height - 2, hitbox.Width, 2), Color.Black);
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
