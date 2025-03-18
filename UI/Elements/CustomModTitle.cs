using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace SquidTestingMod.UI.Elements
{
    public class CustomModTitle : UIElement
    {
        // Text properties
        private string _text;
        private float _textScale = 1f;
        private bool _isLarge;
        private Vector2 _textSize = Vector2.Zero;

        // State properties
        public bool IsEnabled { get; set; } = true;

        // Constants for opacity levels
        private const float ENABLED_OPACITY = 1.0f;
        private const float DISABLED_OPACITY = 0.3f;

        // Colors
        private Color _baseTextColor = Color.White;
        private Color _shadowColor = Color.Black;

        public CustomModTitle(string text = "Mod Title", float textScale = 1f, bool large = false)
        {
            _text = text;
            _textScale = textScale;
            _isLarge = large;

            // Set up the element positioning
            VAlign = 0.5f;
            Left.Set(pixels: 40, 0f);

            // Calculate text dimensions
            RecalculateTextSize();
        }

        private void RecalculateTextSize()
        {
            // Get the appropriate font
            DynamicSpriteFont font = (_isLarge ? FontAssets.DeathText.Value : FontAssets.MouseText.Value);

            // Measure text size
            Vector2 textMeasurement = font.MeasureString(_text);

            // Set the size based on text dimensions 
            _textSize = new Vector2(textMeasurement.X, _isLarge ? 32f : 16f) * _textScale;

            // Update element dimensions
            MinWidth.Set(_textSize.X, 0f);
            MinHeight.Set(_textSize.Y, 0f);
        }

        public void SetText(string text)
        {
            _text = text;
            RecalculateTextSize();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Get dimensions and position
            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = dimensions.Position();

            // Adjust y-position based on font type
            if (_isLarge)
                position.Y -= 10f * _textScale;
            else
                position.Y -= 2f * _textScale;

            // Get the appropriate font
            DynamicSpriteFont font = (_isLarge ? FontAssets.DeathText.Value : FontAssets.MouseText.Value);

            // Calculate opacity based on enabled state and hover
            float opacity = IsEnabled ? ENABLED_OPACITY : DISABLED_OPACITY;
            opacity *= IsMouseHovering ? 1.2f : 1.0f;

            // Create color with appropriate opacity
            Color textColor = _baseTextColor * opacity;
            Color shadowColor = _shadowColor * (opacity * ((float)(int)textColor.A / 255f));

            // Draw the text with shadow
            Vector2 origin = Vector2.Zero;
            Vector2 scale = new Vector2(_textScale);

            // Parse the text into snippets for color codes
            TextSnippet[] snippets = ChatManager.ParseMessage(_text, textColor).ToArray();
            ChatManager.ConvertNormalSnippets(snippets);

            // Draw shadow then text
            ChatManager.DrawColorCodedStringShadow(
                spriteBatch, font, snippets, position, shadowColor,
                0f, origin, scale, -1f, 1.5f);

            ChatManager.DrawColorCodedString(
                spriteBatch, font, snippets, position,
                Color.White, 0f, origin, scale, out var _, -1f);
        }
    }
}