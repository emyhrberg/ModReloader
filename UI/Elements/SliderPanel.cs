using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;

namespace ModHelper.UI.Elements
{
    public class SliderPanel : UIPanel
    {
        // Text
        public OptionTitleText optionTitle;
        public string Title;
        public float TextScale;
        public string HoverText { get; set; }

        // Slider
        public CustomSlider Slider;
        private float Min;
        private float Max;
        public float normalizedValue;
        private float? snapIncrement;
        public Action<float> _onValueChanged;
        private Func<float, string> _valueFormatter; // formats the value to string

        public void UpdateSliderMax(float newMax) => Max = newMax;

        // Constructor
        public SliderPanel(
            string title,
            float min,
            float max,
            float defaultValue,
            Action<float> onValueChanged = null,
            float? increment = null,
            float textSize = 1f,
            string hover = "",
            Action leftClickText = null,
            Action rightClickText = null,
            Func<float, string> valueFormatter = null
            )
        {
            // Size
            Width.Set(-35f, 1f);
            Height.Set(30, 0);
            Left.Set(5, 0);

            // Slider
            Min = min;
            Max = max;
            _onValueChanged = onValueChanged;
            snapIncrement = increment;
            normalizedValue = MathHelper.Clamp((defaultValue - Min) / (max - Min), 0f, 1f);
            _valueFormatter = valueFormatter;

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


            // Create the text element with the proper TextScale value
            TextScale = textSize;
            Title = title;
            HoverText = hover;
            optionTitle = new OptionTitleText(text: Title, hover: HoverText, textSize: TextScale, leftClick: leftClickText, rightClick: rightClickText);
            optionTitle.VAlign = 0.5f;
            optionTitle.TextColor = Color.White;
            // textElement.Left.Set(30, 0);
            optionTitle.Left.Set(0, 0);

            Append(optionTitle);
        }

        public void SetValue(float value)
        {
            if (Max <= Min)
            {
                Max = Min + 1f; // Ensure a valid range
            }

            // Normalize the input value based on Min and Max range
            float normalizedInputValue = (value - Min) / (Max - Min);
            normalizedValue = MathHelper.Clamp(normalizedInputValue, 0f, 1f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            float realValue = MathHelper.Lerp(Min, Max, normalizedValue);

            // Check first if we have a custom formatter
            if (_valueFormatter != null)
            {
                optionTitle.SetText($"{Title}: {_valueFormatter(realValue)}");
                return;
            }

            if (snapIncrement.HasValue && snapIncrement.Value > 0)
            {
                float snapped = (float)Math.Round(realValue / snapIncrement.Value) * snapIncrement.Value;

                // Check known increments, and format accordingly:
                if (snapIncrement.Value == 1f)
                {
                    // Round to integer
                    int currentIntValue = (int)Math.Round(snapped);
                    optionTitle.SetText($"{Title}: {currentIntValue}");
                }
                else if (snapIncrement.Value == 0.1f)
                {
                    // Round to 1 decimal place
                    optionTitle.SetText($"{Title}: {snapped:F1}");
                }
                else if (snapIncrement.Value == 0.01f)
                {
                    // Round to 2 decimal places
                    optionTitle.SetText($"{Title}: {snapped:F2}");
                }
                else
                {
                    // user-provided dynamic title (e.g TimeSlider)
                    optionTitle.SetText(Title);
                }
            }
            else
            {
                // No snap increment => title only
                optionTitle.SetText(Title);
            }
        }

        public void UpdateText(string newText)
        {
            Title = newText;
        }
    }
}