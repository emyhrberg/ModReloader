using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public SliderOption(
            string title,
            float min,
            float max,
            float defaultValue,
            Action<float> onValueChanged = null,
            float? increment = null,
            float textSize = 1f,
            string hover = "")
            : base(title)
        {
            HoverText = hover;
            TextScale = textSize;
            Min = min;
            Max = max;
            _onValueChanged = onValueChanged;
            snapIncrement = increment;

            normalizedValue = MathHelper.Clamp((defaultValue - Min) / (max - Min), 0f, 1f);
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
