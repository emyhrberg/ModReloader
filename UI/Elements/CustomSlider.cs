using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    public class CustomSlider : CustomSliderBase
    {
        private readonly Func<float> _getStatus;
        private readonly Action<float> _slideKeyboard;
        private readonly Func<float, Color> _blipFunc;

        public CustomSlider(
            Func<float> getStatus,
            Action<float> setStatusKeyboard,
            Func<float, Color> blipColorFunction)
        {
            _getStatus = getStatus ?? (() => 0f);
            _slideKeyboard = setStatusKeyboard ?? (_ => { });
            _blipFunc = blipColorFunction ?? (s => Color.Lerp(Color.Black, Color.White, s));

            Height.Set(15, 0);
            Width.Set(175, 0f);
            Left.Set(155, 0);
            VAlign = 0.0f;
            HAlign = 0.0f;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Height.Set(15, 0);
            Width.Set(175, 0f);
            Left.Set(155, 0);
            VAlign = 0.0f;
            HAlign = 0.0f;

            CurrentAimedSlider = null;
            if (!Main.mouseLeft)
                CurrentLockedSlider = null;

            // Use the enum-based property from CustomSliderBase
            var usageLevel = UsageLevel;
            CalculatedStyle dimensions = GetDimensions();

            float offsetX = dimensions.X + dimensions.Width + 22f;
            float offsetY = dimensions.Y + 8f;

            bool wasInBar;
            float newValue = DrawValueBar(spriteBatch, new Vector2(offsetX, offsetY), 0.95f, _getStatus(), usageLevel, out wasInBar, _blipFunc);

            if (CurrentLockedSlider == this || wasInBar)
            {
                CurrentAimedSlider = this;
                if (PlayerInput.Triggers.Current.MouseLeft && CurrentLockedSlider == this)
                    _slideKeyboard(newValue);
            }

            if (CurrentAimedSlider != null && CurrentLockedSlider == null)
                CurrentLockedSlider = CurrentAimedSlider;
        }

        private float DrawValueBar(
    SpriteBatch sb,
    Vector2 pos,
    float scale,
    float sliderPos,
    SliderUsageLevel usageLevel,
    out bool wasInBar,
    Func<float, Color> colorFunc)
        {
            Texture2D barTex = TextureAssets.ColorBar.Value;
            int width = barTex.Width;
            int height = barTex.Height;

            Vector2 barSize = new Vector2(width, height) * scale;
            pos.X -= (int)barSize.X;

            // extra offset
            pos.X -= 55f;
            pos.Y -= 6f;

            Rectangle barRect = new((int)pos.X, (int)pos.Y - (int)barSize.Y / 2, (int)barSize.X, (int)barSize.Y);
            sb.Draw(barTex, barRect, Color.White);

            float innerX = barRect.X + 5f * scale;
            float innerY = barRect.Y + 4f * scale;

            // Draw the blips along the slider track
            for (float i = 0; i < 167; i++)
            {
                float t = i / 167;
                Color c = colorFunc(t);
                sb.Draw(TextureAssets.ColorBlip.Value, new Vector2(innerX + i * scale, innerY), null, c, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

            // This is the original rectangle for value calculation:
            Rectangle valueRect = new((int)innerX, (int)innerY, barRect.Width - 10, barRect.Height - 8);

            // Create an extended hitbox rectangle (for example, 10 pixels larger on each side):
            int padding = 9;
            Rectangle extendedClickRect = new(
                (int)innerX - padding,
                (int)innerY - padding,
                (int)(barRect.Width - 10) + 2 * padding,
                (int)barRect.Height - 8 + 2 * padding);

            // Use the extended hitbox for detecting mouse hover:
            bool hovered = extendedClickRect.Contains(Main.mouseX, Main.mouseY);
            if (usageLevel == SliderUsageLevel.OtherElementIsLocked)
                hovered = false;

            // Draw highlight if hovered or active:
            if (hovered || usageLevel == SliderUsageLevel.SelectedAndLocked)
                sb.Draw(TextureAssets.ColorHighlight.Value, barRect, Main.OurFavoriteColor);

            // Draw the slider "blip"
            sb.Draw(TextureAssets.ColorSlider.Value,
                    new Vector2(innerX + width * scale * sliderPos, innerY + 4f * scale),
                    null, Color.White, 0f,
                    new Vector2(TextureAssets.ColorSlider.Value.Width * 0.5f, TextureAssets.ColorSlider.Value.Height * 0.5f),
                    scale, SpriteEffects.None, 0f);

            // Compute slider ratio based on the original slider area (valueRect)
            float ratio = (Main.mouseX - valueRect.X) / (float)valueRect.Width;
            ratio = MathHelper.Clamp(ratio, 0f, 1f);
            wasInBar = extendedClickRect.Contains(Main.mouseX, Main.mouseY) && !IgnoresMouseInteraction;
            return ratio;
        }
    }
}