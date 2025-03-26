using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace ModHelper.Common.Systems
{
    public class HitboxSystem : ModSystem
    {
        // Boilerplate code to handle hitbox toggling
        #region Hitbox Type

        public record Hitbox(string Name, string HoverTooltipText, Func<bool> GetValue, Action<bool> SetValue)
        {
            public void Toggle() => SetValue(!GetValue());
        }

        public static bool AreAllActive => Hitboxes.All(h => h.GetValue());

        // Booleans for each hitbox
        public static bool ShowPlayerHitbox = false;
        public static bool ShowEnemiesHitboxes = false;
        public static bool ShowTownNPCHitboxes = false;
        public static bool ShowCrittersHitboxes = false;
        public static bool ShowProjectileHitboxes = false;
        public static bool ShowMeleeHitboxes = false;

        // And here
        public static readonly Hitbox[] Hitboxes =
        [
            new("Enemies", "Show enemies hitboxes", () => ShowEnemiesHitboxes, value => ShowEnemiesHitboxes = value),
            new("Town NPCs", "Show town NPCs hitboxes", () => ShowTownNPCHitboxes, value => ShowTownNPCHitboxes = value),
            new("Critters", "Show critters hitboxes", () => ShowCrittersHitboxes, value => ShowCrittersHitboxes = value),
            new("Projectiles", "Show projectiles hitboxes", () => ShowProjectileHitboxes, value => ShowProjectileHitboxes = value),
            new("Melee", "Show melee hitboxes", () => ShowMeleeHitboxes, value => ShowMeleeHitboxes = value),
            new("Player", "Show player hitbox", () => ShowPlayerHitbox, value => ShowPlayerHitbox = value),
        ];

        // Called by “Toggle All”
        public static void SetAllHitboxes(bool value)
        {
            foreach (var hitbox in Hitboxes)
                hitbox.SetValue(value);
        }

        #endregion

        // Make a list that maps names to colors
        public static readonly Dictionary<string, Color> HitboxColors = new()
        {
            ["Player"] = new(255, 113, 69),
            ["Enemies"] = new(226, 57, 39),
            ["Town NPCs"] = new(26, 50, 255),
            ["Critters"] = new(0, 255, 0),
            ["Projectiles"] = new(255, 42, 156),
            ["Melee"] = Color.Yellow
        };

        public override void PostDrawInterface(SpriteBatch sb)
        {
            base.PostDrawInterface(sb);

            RestartSB(sb); // necessary to draw hitboxes correctly

            // draw hitboxes if toggled. pass the colors from the dictionary
            // draw hitboxes if toggled, using the colors from the dictionary
            if (ShowPlayerHitbox) DrawPlayerHitbox(sb, HitboxColors["Player"]);
            if (ShowEnemiesHitboxes) DrawEnemiesHitboxes(sb, HitboxColors["Enemies"]);
            if (ShowTownNPCHitboxes) DrawTownNPCHitboxes(sb, HitboxColors["Town NPCs"]);
            if (ShowProjectileHitboxes) DrawProjectileHitboxes(sb, HitboxColors["Projectiles"]);
            if (ShowMeleeHitboxes) DrawPlayerMeleeHitboxes(sb, HitboxColors["Melee"]);
            if (ShowCrittersHitboxes) DrawCrittersHitboxes(sb, HitboxColors["Critters"]);
        }

        private void RestartSB(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
        }

        private void DrawCrittersHitboxes(SpriteBatch spriteBatch, Color color)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.catchItem > 0)
                {
                    Rectangle hitbox = npc.getRect();
                    hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
                    hitbox = Main.ReverseGravitySupport(hitbox);
                    DrawHitbox(spriteBatch, hitbox, color);
                    DrawOutlineHitbox(spriteBatch, hitbox, color);
                }
            }
        }

        private void DrawPlayerHitbox(SpriteBatch spriteBatch, Color color)
        {
            Rectangle hitbox = Main.LocalPlayer.getRect();
            hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
            hitbox = Main.ReverseGravitySupport(hitbox);
            DrawHitbox(spriteBatch, hitbox, color);
            DrawOutlineHitbox(spriteBatch, hitbox, color);
        }

        private void DrawEnemiesHitboxes(SpriteBatch spriteBatch, Color color)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.townNPC && !npc.friendly)
                {
                    Rectangle hitbox = npc.getRect();
                    hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
                    hitbox = Main.ReverseGravitySupport(hitbox);
                    DrawHitbox(spriteBatch, hitbox, color);
                    DrawOutlineHitbox(spriteBatch, hitbox, color);
                }
            }
        }

        private void DrawTownNPCHitboxes(SpriteBatch spriteBatch, Color color)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.townNPC)
                {
                    Rectangle hitbox = npc.getRect();
                    hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
                    hitbox = Main.ReverseGravitySupport(hitbox);
                    DrawHitbox(spriteBatch, hitbox, color);
                    DrawOutlineHitbox(spriteBatch, hitbox, color);
                }
            }
        }

        private void DrawProjectileHitboxes(SpriteBatch spriteBatch, Color color)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (projectile.active)
                {
                    Rectangle hitbox = projectile.getRect();
                    hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
                    hitbox = Main.ReverseGravitySupport(hitbox);
                    DrawHitbox(spriteBatch, hitbox, color);
                    DrawOutlineHitbox(spriteBatch, hitbox, color);
                }
            }
        }

        private void DrawPlayerMeleeHitboxes(SpriteBatch sb, Color color)
        {
            for (int i = 0; i < 256; i++)
            {
                if (HitboxesGlobalItem.meleeHitbox[i].HasValue)
                {
                    Rectangle hitbox = HitboxesGlobalItem.meleeHitbox[i].Value;
                    hitbox.Offset((int)-Main.screenPosition.X, (int)-Main.screenPosition.Y);
                    hitbox = Main.ReverseGravitySupport(hitbox);
                    DrawHitbox(sb, hitbox, color);
                    DrawOutlineHitbox(sb, hitbox, color);
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
                // If the melee animation is still active, store this frame's hitbox
                if (player.itemAnimation > 0)
                {
                    meleeHitbox[player.whoAmI] = hitbox;
                }
                else
                {
                    // Once the animation ends, clear the hitbox so it stops drawing
                    meleeHitbox[player.whoAmI] = null;
                }
            }

            public override void PostUpdate(Item item)
            {
                base.PostUpdate(item);
            }
        }
    }
}
