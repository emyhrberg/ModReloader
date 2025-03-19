using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    public class SliderOption : PanelElement
    {
        private float Min;
        private float Max;
        public float normalizedValue;
        private float? snapIncrement;
        private System.Action<float> _onValueChanged;

        // The minimal slider
        public CustomSlider Slider;

        public SliderOption(string title, float min, float max, float defaultValue,
            System.Action<float> onValueChanged = null, float? increment = null,
            float textSize = 1.0f, string hover = "")
            : base(title)
        {
            HoverText = hover;
            TextScale = textSize;

            Min = min;
            Max = max;
            snapIncrement = increment;
            _onValueChanged = onValueChanged;

            // Convert the default value to normalized [0..1]
            normalizedValue = MathHelper.Clamp((defaultValue - Min) / (Max - Min), 0f, 1f);

            // Position the text label (optional, from your PanelElement base)
            textElement.HAlign = 0.05f;

            // Create our minimal CustomSlider
            Slider = new CustomSlider(
                // getValue: return our normalizedValue
                getValue: () => normalizedValue,

                // setValue: update the slider when dragged
                setValue: val =>
                {
                    // Update normalized value
                    normalizedValue = val;

                    // Convert to actual [Min..Max]
                    float realValue = MathHelper.Lerp(Min, Max, normalizedValue);

                    // If we have a snap increment, snap to the nearest multiple
                    if (snapIncrement.HasValue && snapIncrement.Value > 0)
                    {
                        float snapped = (float)System.Math.Round(realValue / snapIncrement.Value) * snapIncrement.Value;
                        snapped = MathHelper.Clamp(snapped, Min, Max);
                        normalizedValue = (snapped - Min) / (Max - Min);
                        _onValueChanged?.Invoke(snapped);
                    }
                    else
                    {
                        _onValueChanged?.Invoke(realValue);
                    }
                },

                // barColorFunc: color gradient from black to white
                barColorFunc: s => Color.Lerp(Color.Black, Color.White, s)
            );

            Append(Slider);
        }

        public void SetValue(float newValue)
        {
            // Make sure it's within [Min..Max]
            newValue = MathHelper.Clamp(newValue, Min, Max);

            // Convert to normalized [0..1]
            normalizedValue = (newValue - Min) / (Max - Min);

            // If you want to apply snapping here too, do it:
            if (snapIncrement.HasValue && snapIncrement.Value > 0)
            {
                float snapped = (float)Math.Round(newValue / snapIncrement.Value) * snapIncrement.Value;
                snapped = MathHelper.Clamp(snapped, Min, Max);
                normalizedValue = (snapped - Min) / (Max - Min);
                // Fire callback if needed
                _onValueChanged?.Invoke(snapped);
            }
            else
            {
                _onValueChanged?.Invoke(newValue);
            }
        }
    }
}
