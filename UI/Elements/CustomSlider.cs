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
        public static bool IsDraggingSlider = false;

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

            Width.Set(175f, 0f);
            Height.Set(15f, 0f);
            Left.Set(130f, 0f);
            Top.Set(5f, 0f);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            // Indicate that the slider is being dragged
            IsDraggingSlider = true;
            Log.Info("Slider dragging started");

            // Call the base class behavior for mouse over
            base.MouseOver(evt);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            // Call the base class behavior for mouse out
            base.MouseOut(evt);

            // Reset the dragging flag
            Log.Info("Slider dragging stopped");
            IsDraggingSlider = false;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            CurrentAimedSlider = null;
            if (!Main.mouseLeft)
                CurrentLockedSlider = null;

            // Use the enum-based property from CustomSliderBase
            var usageLevel = UsageLevel;
            CalculatedStyle dimensions = GetDimensions();

            float offsetX = dimensions.X + dimensions.Width + 22f;
            float offsetY = dimensions.Y + 8f;

            bool wasInBar;
            float newValue = DrawValueBar(spriteBatch, new Vector2(offsetX, offsetY), 1f, _getStatus(), usageLevel, out wasInBar, _blipFunc);

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
            Vector2 barSize = new Vector2(barTex.Width, barTex.Height) * scale;
            pos.X -= (int)barSize.X;

            Rectangle barRect = new((int)pos.X, (int)pos.Y - (int)barSize.Y / 2, (int)barSize.X, (int)barSize.Y);
            sb.Draw(barTex, barRect, Color.White);

            float innerX = barRect.X + 5f * scale;
            float innerY = barRect.Y + 4f * scale;

            for (float i = 0; i < 167f; i++)
            {
                float t = i / 167f;
                Color c = colorFunc(t);
                sb.Draw(TextureAssets.ColorBlip.Value, new Vector2(innerX + i * scale, innerY), null, c, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

            Rectangle clickRect = new((int)innerX - 2, (int)innerY, barRect.Width - 4, barRect.Height - 8);
            bool hovered = clickRect.Contains(Main.mouseX, Main.mouseY);
            if (usageLevel == SliderUsageLevel.OtherElementIsLocked)
                hovered = false;

            if (hovered || usageLevel == SliderUsageLevel.SelectedAndLocked)
                sb.Draw(TextureAssets.ColorHighlight.Value, barRect, Main.OurFavoriteColor);

            sb.Draw(TextureAssets.ColorSlider.Value,
                    new Vector2(innerX + 167 * scale * sliderPos, innerY + 4f * scale),
                    null, Color.White, 0f,
                    new Vector2(TextureAssets.ColorSlider.Value.Width * 0.5f, TextureAssets.ColorSlider.Value.Height * 0.5f),
                    scale, SpriteEffects.None, 0f);

            // --- Changed Code Here ---
            float ratio = (Main.mouseX - clickRect.X) / (float)clickRect.Width;
            ratio = MathHelper.Clamp(ratio, 0f, 1f);
            wasInBar = clickRect.Contains(Main.mouseX, Main.mouseY) && !IgnoresMouseInteraction;
            return ratio;
        }
    }
}