// EnemiesSlider.cs
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Systems;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class TimeSlider : UIElement
    {
        public float progress = 0.1f; // 0 = 1x, 1 = 4x
        private bool dragging;

        public TimeSlider()
        {
            Width.Set(60f, 0f);
            Height.Set(20f, 0f);
            // set leftoffset to half of width
            Left.Set(-Width.Pixels / 2, 0.5f);
            Top.Set(10, 0);
            Recalculate();
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            dragging = true;
            UpdateValue(evt);
            // Removed base call to prevent event propagation
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            dragging = false;
            // Removed base call to prevent event propagation
        }

        public override void Update(GameTime gameTime)
        {
            if (dragging)
            {
                CalculatedStyle dims = GetDimensions();
                float relative = (Main.MouseScreen.X - dims.X) / dims.Width;
                progress = MathHelper.Clamp(relative, 0f, 1f);

                // update some value based on progress here

            }
            base.Update(gameTime);
        }

        private void UpdateValue(UIMouseEvent evt)
        {
            CalculatedStyle dims = GetDimensions();
            float relative = (evt.MousePosition.X - dims.X) / dims.Width;
            progress = MathHelper.Clamp(relative, 0f, 1f);

            // update some value based on progress here

            float timeScale = MathHelper.Lerp(1f, 4f, progress);
            FastForwardSystem.speedup = (int)(timeScale - 1f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dims = GetDimensions();
            Rectangle sliderRect = dims.ToRectangle();

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, sliderRect, Color.DarkGray);

            for (int i = 0; i <= 3; i++)
            {
                float fraction = i / 3f;
                int tickX = sliderRect.X + (int)(fraction * sliderRect.Width);
                Rectangle tickRect = new(tickX - 1, sliderRect.Y - 4, 2, sliderRect.Height + 8);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, tickRect, Color.LightGray);
            }

            int knobWidth = 10;
            int knobX = sliderRect.X + (int)(progress * (sliderRect.Width - knobWidth));
            Rectangle knobRect = new(knobX, sliderRect.Y, knobWidth, sliderRect.Height);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, knobRect, Color.Red);

            float displayValue = MathHelper.Lerp(1f, 4f, progress);
            string valueText = $"{displayValue:0.0}x";
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(valueText);
            Vector2 textPos = new(knobRect.Center.X - textSize.X / 2, sliderRect.Y - textSize.Y - 2);
            Utils.DrawBorderString(spriteBatch, valueText, textPos, Color.White, 0.8f);
        }
    }
}
