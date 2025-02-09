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
        public float progress = 0.1f; // 0 = 1x, 1 = 2x
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
                // Clamp the relative value and then snap it to 0 or 1:
                progress = MathHelper.Clamp(relative, 0f, 1f) < 0.5f ? 0f : 1f;

                // Update the time scale based on the snapped progress:
                float timeScale = MathHelper.Lerp(1f, 2f, progress);
                FastForwardSystem.speedup = (int)(timeScale - 1f);
            }
            base.Update(gameTime);
        }

        private void UpdateValue(UIMouseEvent evt)
        {
            CalculatedStyle dims = GetDimensions();
            float relative = (evt.MousePosition.X - dims.X) / dims.Width;
            // Snap the value:
            progress = MathHelper.Clamp(relative, 0f, 1f) < 0.5f ? 0f : 1f;

            float timeScale = MathHelper.Lerp(1f, 2f, progress);
            FastForwardSystem.speedup = (int)(timeScale - 1f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dims = GetDimensions();
            Rectangle sliderRect = dims.ToRectangle();

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, sliderRect, Color.DarkGray);

            // draw 2 ticks
            for (int i = 0; i < 2; i++)
            {
                int tickX = sliderRect.X + (int)(i / 1f * sliderRect.Width);
                Rectangle tickRect = new(tickX - 1, sliderRect.Y - 4, 2, sliderRect.Height + 8);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, tickRect, Color.LightGray);
            }

            // knob
            int knobWidth = 10;
            Rectangle knobRect = new((int)(sliderRect.X + progress * sliderRect.Width - knobWidth / 2), sliderRect.Y - 4, knobWidth, sliderRect.Height + 8);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, knobRect, Color.White);

            // display value
            float displayValue = MathHelper.Lerp(1f, 2f, progress);
            string valueText = $"{displayValue:0.0}x";
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(valueText);
            Vector2 textPos = new(knobRect.Center.X - textSize.X / 2, sliderRect.Y - textSize.Y - 2);
            Utils.DrawBorderString(spriteBatch, valueText, textPos, Color.White, 0.8f);
        }
    }
}
