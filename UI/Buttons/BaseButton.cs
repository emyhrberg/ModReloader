using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    /// <summary>
    /// The base class for the main buttons that are displayed.
    /// This class handles the general button logic, such as drawing the button, animations, and tooltip text.
    /// </summary>
    public abstract class BaseButton : UIImageButton
    {
        // General variables for a button
        protected Asset<Texture2D> Button;
        protected Asset<Texture2D> Spritesheet { get; set; }
        public string HoverText = "";
        public string HoverTextDescription;
        protected float opacity = 0.8f;
        public ButtonText ButtonText;
        public bool Active = true;

        // Animation frames
        protected int currFrame = 1; // the current frame
        protected int frameCounter = 0; // the counter for the frame speed

        // Animations variables to be set by child classes. virtual means it can be overridden by child classes.
        protected virtual float Scale { get; set; } = 1; // the scale of the animation.
        protected virtual int StartFrame => 1;
        protected virtual int FrameCount => 1;
        protected virtual int FrameSpeed => 0; // the speed of the animation, lower is faster
        protected abstract int FrameWidth { get; } // abstract means force child classes to implement this
        protected abstract int FrameHeight { get; } // abstract means force child classes to implement this

        // Constructor
        protected BaseButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription="", float textSize = 0.9f) : base(spritesheet)
        {
            Button = Ass.Button;
            Spritesheet = spritesheet;
            HoverText = hoverText;
            HoverTextDescription = hoverTextDescription;
            SetImage(Button);
            currFrame = StartFrame;

            // Add a UIText centered horizontally at the bottom of the button.
            // Set the scale; 70f seems to fit to 0.9f scale.
            ButtonText = new ButtonText(text: buttonText, textScale: textSize, large: false);
            Append(ButtonText);
        }

        public void UpdateHoverText()
        {
            // Based on modstoreload, make the hovertext.
            string modsToReload = string.Join(", ", ModsToReload.modsToReload);
            HoverText = $"Reload {modsToReload}";
        }

        /// <summary>
        /// Draws the button with the specified image/animation and tooltip text
        /// </summary>
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!Active || Button == null || Button.Value == null)
                return;

            // Get the button size from MainState (default to 70 if MainState is null)
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            float buttonSize = sys.mainState?.ButtonSize ?? 70f;

            // Update the scale based on the buttonsize. 70f means a scale of 1. For every 10 pixels, the scale is increased by 0.1f.

            // Get the dimensions based on the button size.
            CalculatedStyle dimensions = GetInnerDimensions();
            Rectangle drawRect = new((int)dimensions.X, (int)dimensions.Y, (int)buttonSize, (int)buttonSize);

            // Set the opacity based on mouse hover.
            opacity = IsMouseHovering ? 1f : 0.8f; // Determine opacity based on mouse hover.

            // Set UIText opacity
            ButtonText.TextColor = Color.White * opacity;

            // Draw the texture with the calculated opacity.
            spriteBatch.Draw(Button.Value, drawRect, Color.White * opacity);

            // Draw the animation texture
            if (Spritesheet != null)
            {
                if (IsMouseHovering)
                {
                    frameCounter++;
                    if (frameCounter >= FrameSpeed)
                    {
                        currFrame++;
                        if (currFrame > FrameCount)
                            currFrame = StartFrame;
                        frameCounter = 0;
                    }
                }
                else
                {
                    currFrame = StartFrame;
                    frameCounter = 0;
                }

                // Use a custom scale and offset to draw the animated overlay.
                Vector2 position = dimensions.Position();
                Vector2 size = new(dimensions.Width, dimensions.Height);
                Vector2 centeredPosition = position + (size - new Vector2(FrameWidth, FrameHeight) * Scale) / 2f;
                Rectangle sourceRectangle = new(x: 0, y: (currFrame - 1) * FrameHeight, FrameWidth, FrameHeight);
                centeredPosition.Y -= 7; // magic offset to move it up a bit

                // Draw the spritesheet.
                spriteBatch.Draw(Spritesheet.Value, centeredPosition, sourceRectangle, Color.White * opacity, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
            }

            // Update: Drawing is now done in MainState
            /// <see cref="MainState"/> 
            // Draw tooltip text if hovering and HoverText is given (see MainState).
            // if (!string.IsNullOrEmpty(HoverText) && IsMouseHovering)
            // {
            //     UICommon.TooltipMouseText(HoverText);

            //     DrawHelper.DrawTooltipPanel(this, "a", HoverText); // Draw the tooltip panel
            // }
        }

        //Disable button click if config window is open
        public override bool ContainsPoint(Vector2 point)
        {
            if (!Active) // Dont allow clicking if button is disabled.
                return false;

            try
            {
                if (Main.InGameUI != null)
                {
                    var currentStateProp = Main.InGameUI.GetType().GetProperty("CurrentState", BindingFlags.Public | BindingFlags.Instance);
                    if (currentStateProp != null)
                    {
                        var currentState = currentStateProp.GetValue(Main.InGameUI);
                        if (currentState != null)
                        {
                            string stateName = currentState.GetType().Name;
                            if (stateName.Contains("Config"))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error checking UI state in ContainsPoint: " + ex.Message);
            }

            return base.ContainsPoint(point);
        }

        // Disable item use on click
        public override void Update(GameTime gameTime)
        {
            // base update
            base.Update(gameTime);

            // disable item use if the button is hovered
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
    }
}