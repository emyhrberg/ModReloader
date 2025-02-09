using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class DrawUIState : UIState
    {
        private List<Color> rainbowColors;

        public override void OnInitialize()
        {
            GenerateRainbowColors(20);
        }

        public void DrawHitbox(UIElement element, SpriteBatch spriteBatch)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (!sys.mainState.uiDebugButton.IsUIDebugDrawing) return; // Toggle check

            if (element is DrawUIState || element is MainState) return; // Skip full-screen elements

            Rectangle hitbox = element.GetOuterDimensions().ToRectangle();

            // Get a color from the rainbow
            int colorIndex = element.UniqueId % rainbowColors.Count;
            Color hitboxColor = rainbowColors[colorIndex] * 0.5f;

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, hitbox, hitboxColor);
            DrawOutline(spriteBatch, hitbox);
        }

        private void DrawOutline(SpriteBatch spriteBatch, Rectangle hitbox)
        {
            hitbox.Inflate(1, 1);
            Texture2D t = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(t, new Rectangle(hitbox.X, hitbox.Y, hitbox.Width, 2), Color.White);
            spriteBatch.Draw(t, new Rectangle(hitbox.X, hitbox.Y, 2, hitbox.Height), Color.White);
            spriteBatch.Draw(t, new Rectangle(hitbox.X + hitbox.Width - 2, hitbox.Y, 2, hitbox.Height), Color.White);
            spriteBatch.Draw(t, new Rectangle(hitbox.X, hitbox.Y + hitbox.Height - 2, hitbox.Width, 2), Color.White);
        }

        private void GenerateRainbowColors(int count)
        {
            rainbowColors = new List<Color>();
            for (int i = 0; i < count; i++)
            {
                float hue = (float)i / count;
                rainbowColors.Add(Main.hslToRgb(hue, 1f, 0.5f));
            }
        }
    }
}
