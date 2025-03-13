using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    public class SliderOption : PanelElement
    {
        // The actual slider control.
        public CustomSlider Slider;

        // Slider limits.
        private float Min;
        private float Max;

        // The slider’s current normalized value (0..1).
        public float normalizedValue;

        // Optional snap increment.
        // If set (and > 0), the slider value will snap to multiples of this.
        private float? snapIncrement;

        public Action<float> _onValueChanged;

        // Added an optional "increment" parameter.
        public SliderOption(string title, float min, float max, float defaultValue, Action<float> onValueChanged = null, float? increment = null, float textSize = 1.0f, string hover = "")
            : base(title)
        {
            // Set hover and text size.
            HoverText = hover;
            TextScale = textSize;

            Min = min;
            Max = max;
            _onValueChanged = onValueChanged;
            snapIncrement = increment;

            // Convert the default value into a normalized 0..1 value.
            normalizedValue = MathHelper.Clamp((defaultValue - Min) / (max - min), 0f, 1f);

            // Position the text element.
            textElement.HAlign = 0.05f;

            // Create the slider control.
            Slider = new CustomSlider(
                textKey: Language.GetText("UI.SliderLabel"),
                getStatus: () => normalizedValue,
                setStatusKeyboard: val =>
                {
                    // Update the normalized value.
                    normalizedValue = val;
                    // Convert normalized value to the actual value.
                    float realValue = MathHelper.Lerp(Min, Max, normalizedValue);
                    if (snapIncrement.HasValue && snapIncrement.Value > 0)
                    {
                        // Snap to nearest multiple.
                        float snapped = (float)Math.Round(realValue / snapIncrement.Value) * snapIncrement.Value;
                        snapped = MathHelper.Clamp(snapped, Min, Max);
                        normalizedValue = (snapped - Min) / (Max - Min);
                        _onValueChanged?.Invoke(snapped);
                    }
                    else
                    {
                        _onValueChanged?.Invoke(realValue);
                    }
                },
                setStatusGamepad: () => { },
                blipColorFunction: s => Color.Lerp(Color.Black, Color.White, s),
                color: Color.White
            );

            Append(Slider);
        }

        public void SetValue(float value)
        {
            normalizedValue = MathHelper.Clamp(value, 0f, 1f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            textElement.HAlign = 0.08f;

            base.Draw(spriteBatch);

            float realValue = MathHelper.Lerp(Min, Max, normalizedValue);
            if (snapIncrement.HasValue && snapIncrement.Value > 0)
            {
                float snapped = (float)Math.Round(realValue / snapIncrement.Value) * snapIncrement.Value;
                textElement.SetText($"{Title}: {snapped}");
            }
            else
            {
                int currentIntValue = (int)Math.Round(realValue);
                textElement.SetText($"{Title}: {currentIntValue}");
            }
        }
    }
}
