using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements; // For UIColoredSlider if it's in scope
using Terraria.Localization;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    public class SliderOption : PanelElement
    {
        // The actual slider
        public CustomSlider Slider;

        // Store the slider limits
        private float Min;
        private float Max;

        // Store the slider’s current value in [0..1]
        public float normalizedValue;

        public Action<float> _onValueChanged;

        public SliderOption(string title, float min, float max, float defaultValue, Action<float> onValueChanged = null)
            : base(title)
        {
            Min = min;
            Max = max;
            _onValueChanged = onValueChanged;

            // Convert default integer value to normalized 0..1
            normalizedValue = MathHelper.Clamp(
                (defaultValue - Min) / (max - min),
                0f, 1f
            );

            // Position the text slightly to the left
            textElement.HAlign = 0.1f;

            // Create the colored slider
            // NOTE: Adjust or remove parameters as needed to match your actual UIColoredSlider constructor!
            Slider = new CustomSlider(
                textKey: Language.GetText("UI.SliderLabel"), // or a custom LocalizedText
                getStatus: () => normalizedValue,
                setStatusKeyboard: val =>
                {
                    // val is the new normalized slider value (0..1)
                    normalizedValue = val;
                    // You could do something else here, like apply it to config

                    // onvalue changed
                    _onValueChanged?.Invoke(MathHelper.Lerp(Min, Max, normalizedValue));
                },
                setStatusGamepad: () => { }, // optional gamepad stuff
                blipColorFunction: s => Color.Lerp(Color.Black, Color.White, s),
                color: Color.White
            );

            // Append the slider so it will draw and respond to input
            Append(Slider);
        }

        public void SetValue(float value)
        {
            normalizedValue = MathHelper.Clamp(value, 0f, 1f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Convert the normalized slider value back to an integer
            int currentIntValue = (int)Math.Round(
                MathHelper.Lerp(Min, Max, normalizedValue)
            );

            // Update text
            textElement.SetText($"{Title}: {currentIntValue}");
        }
    }
}
