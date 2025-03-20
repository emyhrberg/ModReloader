using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    public class SliderOption : PanelElement
    {
        public CustomSlider Slider;
        private float Min;
        private float Max;
        public float normalizedValue;
        private float? snapIncrement;
        public Action<float> _onValueChanged;

        // Constructor
        public SliderOption(
            string title,
            float min,
            float max,
            float defaultValue,
            Action<float> onValueChanged = null,
            float? increment = null,
            float textSize = 1f,
            string hover = "",
            Action onClickText = null
            ) : base(title)
        {
            HoverText = hover;
            TextScale = textSize;
            Min = min;
            Max = max;
            _onValueChanged = onValueChanged;
            snapIncrement = increment;

            normalizedValue = MathHelper.Clamp((defaultValue - Min) / (max - Min), 0f, 1f);

            // text element
            textElement.OnLeftClick += (evt, element) => onClickText?.Invoke();
            textElement.HAlign = 0.08f;

            Slider = new CustomSlider(
                () => normalizedValue,
                val =>
                {
                    normalizedValue = val;
                    float realValue = MathHelper.Lerp(Min, Max, normalizedValue);
                    if (snapIncrement.HasValue && snapIncrement.Value > 0)
                    {
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
                s => Color.Lerp(Color.Black, Color.White, s)
            );
            Append(Slider);
        }

        public void SetValue(float value)
        {
            normalizedValue = MathHelper.Clamp(value, 0f, 1f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            float realValue = MathHelper.Lerp(Min, Max, normalizedValue);

            if (snapIncrement.HasValue && snapIncrement.Value > 0)
            {
                float snapped = (float)Math.Round(realValue / snapIncrement.Value) * snapIncrement.Value;

                // Check known increments, and format accordingly:
                if (snapIncrement.Value == 1f)
                {
                    // Round to integer
                    int currentIntValue = (int)Math.Round(snapped);
                    textElement.SetText($"{Title}: {currentIntValue}");
                }
                else if (snapIncrement.Value == 0.1f)
                {
                    // Round to 1 decimal place
                    textElement.SetText($"{Title}: {snapped:F1}");
                }
                else if (snapIncrement.Value == 0.01f)
                {
                    // Round to 2 decimal places
                    textElement.SetText($"{Title}: {snapped:F2}");
                }
                else
                {
                    // Fallback to showing the raw snapped value if not one of the above
                    textElement.SetText($"{Title}: {snapped}");
                }
            }
            else
            {
                // No snap increment => treat as an integer
                int currentIntValue = (int)Math.Round(realValue);
                textElement.SetText($"{Title}: {currentIntValue}");
            }
        }
    }
}