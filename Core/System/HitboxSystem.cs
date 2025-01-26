using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SkipSelect.Core.System
{
    public class HitboxSystem : ModSystem
    {
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (!ModContent.GetInstance<Config>().ShowHitboxes)
                return;

            drawPlayerHitbox(spriteBatch);
            drawNPCs(spriteBatch);
        }

        private void drawNPCs(SpriteBatch spriteBatch)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active) continue; // Skip inactive NPCs

                // Get NPC hitbox
                int w = npc.Hitbox.Width;
                int h = npc.Hitbox.Height;
                int x = (int)(npc.Hitbox.X - Main.screenPosition.X);
                int y = (int)(npc.Hitbox.Y - Main.screenPosition.Y);
                x -= w * 4 + w / 3 + 1;
                y -= h + h / 4;

                // Draw red rectangle (main hitbox)
                Rectangle redRect = new Rectangle(x, y, w, h);
                Color fillColor = Color.Red * 0.3f;
                spriteBatch.Draw(pixel, redRect, fillColor);

                // Draw black border
                spriteBatch.Draw(pixel, new Rectangle(x - 1, y - 1, w + 2, 1), Color.Black); // Top
                spriteBatch.Draw(pixel, new Rectangle(x - 1, y - 1, 1, h + 2), Color.Black); // Left
                spriteBatch.Draw(pixel, new Rectangle(x - 1, y + h, w + 2, 1), Color.Black); // Bottom
                spriteBatch.Draw(pixel, new Rectangle(x + w, y - 1, 1, h + 2), Color.Black); // Right
            }
        }

        private void drawPlayerHitbox(SpriteBatch spriteBatch)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            int w = Main.LocalPlayer.Hitbox.Width;
            int h = Main.LocalPlayer.Hitbox.Height;
            int x = (int)(Main.LocalPlayer.Hitbox.X - Main.screenPosition.X);
            int y = (int)(Main.LocalPlayer.Hitbox.Y - Main.screenPosition.Y);
            x -= w * 4 + w / 3 + 1;
            y -= h + h / 4;
            Rectangle pos = new(x, y, w, h);
            Color color = Color.Red * 0.3f;
            spriteBatch.Draw(pixel, pos, color);

            // draw black
            spriteBatch.Draw(pixel, new Rectangle(x - 1, y - 1, w + 2, 1), Color.Black);
            spriteBatch.Draw(pixel, new Rectangle(x - 1, y - 1, 1, h + 2), Color.Black);
            spriteBatch.Draw(pixel, new Rectangle(x - 1, y + h, w + 2, 1), Color.Black);
            spriteBatch.Draw(pixel, new Rectangle(x + w, y - 1, 1, h + 2), Color.Black);
        }
    }
}