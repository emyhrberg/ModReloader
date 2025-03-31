using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ModHelper.UI.Buttons;
using ModHelper.UI.Elements;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace ModHelper.UI
{
    public class MainState : UIState
    {
        // Buttons
        public readonly List<BaseButton> AllButtons = [];
        public float ButtonSize;
        private float ButtonOffset = 0; // offset for the button position

        // MainState Constructor. This is where we create all the buttons and set up their positions.
        public MainState() => AddEverything();

        public void AddEverything()
        {
            // Set the button size
            ButtonOffset = 0; // reset the offset
            ButtonSize = (Conf.C == null) ? 70f : Conf.C.ButtonSize;

            // Add buttons
            AddButton<ReloadSPButton>(Ass.ButtonReloadSP, "Reload", "Singleplayer Reload", "Reloads the selected mod");
            AddButton<ReloadMPButton>(Ass.ButtonReloadMP, "Reload", "Multiplayer Reload", "Reloads the selected mod");
            AddButton<ConfigButton>(Ass.ButtonConfig, "Config", "Open config", "This button is only here for testing purposes");
            AddButton<TestButton>(Ass.CollapseUp, "Test", "Testing", "This button is only here for testing purposes");

            // Add a little text in the corner
            FocusText focusText = new("Keep Game Running: Disabled");
            Append(focusText);
        }

        private T AddButton<T>(Asset<Texture2D> spritesheet = null, string buttonText = null, string hoverText = null, string hoverTextDescription = "") where T : BaseButton
        {
            // Create a new button using reflection
            T button = (T)Activator.CreateInstance(typeof(T), spritesheet, buttonText, hoverText, hoverTextDescription);

            // Button dimensions
            float size = ButtonSize;
            button.Width.Set(size, 0f);
            button.Height.Set(size, 0f);

            // Set x pos with offset
            button.Left.Set(pixels: ButtonOffset, precent: 0f);
            ButtonOffset += ButtonSize;
            button.ButtonIndex = AllButtons.Count;

            // Set VAlign and HAlign based on config
            button.VAlign = Conf.C.ButtonPosition.Y;
            button.HAlign = Conf.C.ButtonPosition.X;

            button.Recalculate();

            // Add the button to the list of all buttons and append it to the MainState
            AllButtons.Add(button);
            Append(button);
            return button;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw everything in the main state
            base.Draw(spriteBatch);

            // Draw tooltips for all buttons
            foreach (var button in AllButtons)
            {
                if (button.IsMouseHovering && button.HoverText != null)
                {
                    // Draw the tooltip
                    DrawHelper.DrawTooltipPanel(button, button.HoverText, button.HoverTextDescription); // Draw the tooltip panel
                }
            }

            // For debugging - draw the button area rect
            // var buttonArea = GetButtonAreaRectangle();
            // spriteBatch.Draw(TextureAssets.MagicPixel.Value, buttonArea, Color.Red * 0.5f);
        }

        #region Right  dragging

        public bool dragging; // whether we are dragging or not
        private Vector2 dragStartPosition; // where the drag started

        // Helper to get the rectangle that encompasses all buttons
        private Rectangle GetButtonAreaRectangle()
        {
            // Find min and max positions of all buttons
            float minX = float.MaxValue;
            float minY = float.MaxValue;
            float maxX = float.MinValue;
            float maxY = float.MinValue;

            foreach (var button in AllButtons)
            {
                CalculatedStyle dimensions = button.GetDimensions();
                minX = Math.Min(minX, dimensions.X);
                minY = Math.Min(minY, dimensions.Y);
                maxX = Math.Max(maxX, dimensions.X + dimensions.Width);
                maxY = Math.Max(maxY, dimensions.Y + dimensions.Height);
            }

            return new Rectangle(
                (int)minX,
                (int)minY,
                (int)(maxX - minX),
                (int)(maxY - minY)
            );
        }

        // Check if the mouse is within the button area
        private bool IsMouseInButtonArea()
        {
            Rectangle buttonArea = GetButtonAreaRectangle();
            return buttonArea.Contains(Main.mouseX, Main.mouseY);
        }

        public override void RightMouseDown(UIMouseEvent evt)
        {
            base.RightMouseDown(evt);

            // Only start dragging if mouse is within button area
            if (IsMouseInButtonArea())
            {
                dragStartPosition = new Vector2(Main.mouseX, Main.mouseY);
                dragging = true;

                // Store initial alignments and offsets of all buttons
                foreach (var button in AllButtons)
                {
                    // Store tuple of: HAlign, VAlign, Left offset
                    button.Tag = new Tuple<float, float, float>(
                        button.HAlign,
                        button.VAlign,
                        button.Left.Pixels
                    );
                }
            }
        }

        // used to separate clicking from dragging
        public bool rightClicking = false;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (dragging)
            {
                // First check if we dragged far enough to consider it a drag
                float dragDistance = Vector2.Distance(new Vector2(Main.mouseX, Main.mouseY), dragStartPosition);
                if (dragDistance < 10f) // 10 pixels threshold
                {
                    rightClicking = true;
                    return;
                }
                rightClicking = false;

                // Calculate the drag delta as percentage of screen
                Vector2 mouseDelta = new Vector2(Main.mouseX, Main.mouseY) - dragStartPosition;
                Vector2 alignmentDelta = new Vector2(
                    mouseDelta.X / Main.screenWidth,
                    mouseDelta.Y / Main.screenHeight
                );

                // Move all buttons by adjusting alignment but preserve horizontal spacing
                foreach (var button in AllButtons)
                {
                    if (button.Tag is Tuple<float, float, float> initialData)
                    {
                        // Unpack the tuple
                        float initialHAlign = initialData.Item1;
                        float initialVAlign = initialData.Item2;
                        float initialLeftOffset = initialData.Item3;

                        // Calculate new alignment values with clamping to keep on screen
                        float newHAlign = Math.Clamp(initialHAlign + alignmentDelta.X, 0.01f, 0.99f);
                        float newVAlign = Math.Clamp(initialVAlign + alignmentDelta.Y, 0.01f, 0.99f);

                        // Apply the new alignment
                        button.HAlign = newHAlign;
                        button.VAlign = newVAlign;

                        // Keep the original horizontal offset
                        button.Left.Set(initialLeftOffset, 0);

                        button.Recalculate();
                    }
                }

                // Prevent using items while dragging
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public override void RightMouseUp(UIMouseEvent evt)
        {
            base.RightMouseUp(evt);

            if (dragging)
            {
                dragging = false;

                // Save the new button position to config
                // Use the first button's alignment values
                Conf.C.ButtonPosition = new Vector2(AllButtons[0].HAlign, AllButtons[0].VAlign);
                Conf.Save();
            }
        }

        #endregion
    }
}