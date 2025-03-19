using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    /// <summary>
    /// A minimal slider UI element that can be locked (dragged) by the mouse.
    /// </summary>
    public class CustomSlider : UIElement
    {
        // Which slider is currently locked/dragged
        private static CustomSlider _currentLocked;

        // Which slider is currently hovered
        private static CustomSlider _currentAimed;

        /// <summary>
        /// Clears any locked/hovered references (e.g., on UI close).
        /// </summary>
        public static void EscapeElements()
        {
            _currentLocked = null;
            _currentAimed = null;
        }

        // How we get the current slider value (0..1).
        private readonly Func<float> _getValue;

        // How we set the slider value (0..1) when dragging.
        private readonly Action<float> _setValue;

        // Returns a color for each portion of the bar, given a [0..1] progress.
        private readonly Func<float, Color> _barColorFunc;

        /// <summary>
        /// Creates a minimal custom slider.
        /// </summary>
        /// <param name="getValue">Return current slider value [0..1].</param>
        /// <param name="setValue">Set new slider value [0..1] when dragged.</param>
        /// <param name="barColorFunc">
        /// Function to color each portion [0..1] of the bar.
        /// If null, defaults to white.
        /// </param>
        public CustomSlider(Func<float> getValue, Action<float> setValue, Func<float, Color> barColorFunc = null)
        {
            _getValue = getValue ?? (() => 0f);
            _setValue = setValue ?? (_ => { });
            _barColorFunc = barColorFunc ?? (_ => Color.White);

            // Example size and position (customize as needed).
            Width.Set(160f, 0f);
            Height.Set(20f, 0f);
            Left.Set(50f, 0f);
            Top.Set(50f, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            // If mouse is released, no slider is locked anymore
            if (!Main.mouseLeft)
            {
                _currentLocked = null;
            }

            // Determine if this slider is locked, or blocked by another locked slider
            bool isLockedByThis = (_currentLocked == this);
            bool anotherLocked = (_currentLocked != null && !isLockedByThis);

            // If another slider is locked, we ignore our hover
            bool canHover = !anotherLocked && IsMouseHovering;

            // If we are locked by ourselves, we always consider ourselves "hovered"
            if (isLockedByThis)
            {
                canHover = true;
            }

            // Draw the bar and get potential new slider value
            bool overBar;
            float newVal = DrawBar(spriteBatch, _getValue(), out overBar);

            // If we're hovered or locked, we are the aimed slider
            if (overBar || isLockedByThis)
            {
                _currentAimed = this;

                // If the mouse is held down on us, we lock ourselves
                if (PlayerInput.Triggers.Current.MouseLeft && !PlayerInput.UsingGamepad)
                {
                    // If not already locked, lock ourselves
                    if (_currentLocked == null)
                        _currentLocked = this;

                    // If locked, we can set the new value
                    if (_currentLocked == this)
                    {
                        _setValue(newVal);
                    }
                }
            }
        }

        /// <summary>
        /// Draws the background bar, the color gradient, and the knob.
        /// Returns what the new [0..1] slider value *would be* if we're hovered.
        /// </summary>
        private float DrawBar(SpriteBatch spriteBatch, float currentVal, out bool overBar)
        {
            // Basic textures from Terraria
            Texture2D barTex = TextureAssets.ColorBar.Value;
            Texture2D blipTex = TextureAssets.ColorBlip.Value;
            Texture2D knobTex = TextureAssets.ColorSlider.Value;
            Texture2D highlight = TextureAssets.ColorHighlight.Value; // for hover effect

            // Position for the bar in world space
            CalculatedStyle dims = GetDimensions();
            Vector2 barPos = new(dims.X, dims.Y);
            float scale = 1f;

            // Draw the basic bar
            spriteBatch.Draw(barTex, barPos, Color.White);

            // The actual bar is 167 px wide (for the gradient).
            // We'll start drawing the gradient a few px inside the bar.
            float gradientStartX = barPos.X + 5f;
            float gradientY = barPos.Y + 4f; // shift slightly down

            // Draw the color gradient
            for (int i = 0; i < 167; i++)
            {
                float progress = i / 167f;
                spriteBatch.Draw(
                    blipTex,
                    new Vector2(gradientStartX + i * scale, gradientY),
                    null,
                    _barColorFunc(progress),
                    0f,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }

            // Interactive rectangle for hover checks
            Rectangle interactRect = new(
                (int)gradientStartX,
                (int)gradientY,
                167,
                barTex.Height - 8 // reduce top/bottom padding
            );

            // Check if we're hovering inside that rectangle
            overBar = interactRect.Contains(Main.mouseX, Main.mouseY) && !IgnoresMouseInteraction;

            // If hovered, draw a highlight over the entire bar
            if (overBar)
            {
                Rectangle highlightRect = new((int)barPos.X, (int)barPos.Y, barTex.Width, barTex.Height);
                spriteBatch.Draw(highlight, highlightRect, Main.OurFavoriteColor);
            }

            // Draw the knob
            float knobX = gradientStartX + 167f * currentVal;
            float knobY = gradientY + 4f;
            Vector2 knobOrigin = new(knobTex.Width * 0.5f, knobTex.Height * 0.5f);
            spriteBatch.Draw(knobTex, new Vector2(knobX, knobY), null, Color.White,
                0f, knobOrigin, scale, SpriteEffects.None, 0f);

            // If hovered, compute new slider value based on mouse position
            if (overBar)
            {
                float mouseOffset = Main.mouseX - interactRect.X;
                float newVal = MathHelper.Clamp(mouseOffset / interactRect.Width, 0f, 1f);
                return newVal;
            }

            // Otherwise, just return the current value
            return currentVal;
        }
    }
}
