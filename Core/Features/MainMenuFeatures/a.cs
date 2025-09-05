using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;

namespace ModReloader.Core.Features.MainMenuFeatures
{
    internal class DrawBackToMainMenuSystem : ModSystem
    {
        public override void Load()
        {
            if (!Conf.C.ShowBackToMainMenu) return;
            On_Main.DrawVersionNumber += MainDrawVersionNumber_Detour;
        }

        public override void Unload()
        {
            if (!Conf.C.ShowBackToMainMenu) return;
            On_Main.DrawVersionNumber -= MainDrawVersionNumber_Detour;
        }

        private void MainDrawVersionNumber_Detour(On_Main.orig_DrawVersionNumber orig, Color menucolor, float upbump)
        {
            orig(menucolor, upbump);

            if (!Conf.C.ShowBackToMainMenu) return;

            if (Main.menuMode != 0)
            {
                if (Main.menuMode == 888 && Main.MenuUI._currentState == Interface.modConfig)
                    DrawBackToMainMenu(menucolor);
                return;
            }
        }

        private void DrawBackToMainMenu(Color menucolor)
        {
            string text = "Back to Main Menu";

            // Measure text
            Vector2 size = FontAssets.MouseText.Value.MeasureString(text);
            float scale = 1.26f;
            Vector2 hoverSize = new(size.X * scale, size.Y * scale);

            // Start at top-right corner
            var drawPos = new Vector2(15, 35);

            DrawMainMenuText(text, drawPos);
        }

        private static void DrawMainMenuText(string text, Vector2 pos)
        {
            var font = FontAssets.MouseText.Value;
            var spriteBatch = Main.spriteBatch;

            // Text scaling
            float scale = 1.15f;
            Vector2 textSize = font.MeasureString(text) * scale;

            // Mouse hover detection
            bool hovered = Main.MouseScreen.Between(pos, pos + textSize);

            // Hover color logic
            Color textColor = hovered ? new Color(237, 246, 255) : new Color(173, 173, 198);
            float alpha = hovered ? 1f : 0.76f;

            // Draw outlined text
            DrawOutlinedStringOnMenu(
                spriteBatch,
                font,
                text,
                pos,
                textColor,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0f,
                alphaMult: alpha
            );
            // Optional: Return to main menu on click
            if (hovered && Main.mouseLeft && Main.mouseLeftRelease)
            {
                Conf.C.SaveChanges();
                Main.menuMode = 0;
                SoundEngine.PlaySound(Terraria.ID.SoundID.MenuClose);
            }
        }

        private static void DrawOutlinedStringOnMenu(SpriteBatch spriteBatch, DynamicSpriteFont font, string text,
            Vector2 position, Color drawColor, float rotation, Vector2 origin, float scale, SpriteEffects effects,
            float layerDepth, bool special = false, float alphaMult = 0.3f)
        {
            for (int i = 0; i < 5; i++)
            {
                Color color;
                if (i == 4) // draw the main text last
                {
                    color = drawColor;
                }
                else // Draw the outline first
                {
                    color = Color.Black;
                    if (special)
                    {
                        color.R = (byte)((255 + color.R) / 2);
                        color.G = (byte)((255 + color.G) / 2);
                        color.B = (byte)((255 + color.B) / 2);
                    }
                }
                // Adjust alpha
                color.A = (byte)(color.A * alphaMult);

                // Outline offsets
                int offX = 0;
                int offY = 0;
                switch (i)
                {
                    case 0: offX = -2; break;
                    case 1: offX = 2; break;
                    case 2: offY = -2; break;
                    case 3: offY = 2; break;
                }

                // Draw text
                spriteBatch.DrawString(font, text, position + new Vector2(offX, offY),
                color, rotation, origin, scale, effects, layerDepth);
            }
        }
    }
}