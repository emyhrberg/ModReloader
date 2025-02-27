using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
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
        protected string HoverText = "";
        protected float opacity = 0.4f;
        public ButtonText buttonUIText;
        public float RelativeLeftOffset = 0f;
        public bool Active = true;

        // Animation frames
        protected int currFrame = 1; // the current frame
        protected int frameCounter = 0; // the counter for the frame speed

        // Animations variables to be set by child classes
        protected virtual float SpriteScale => 1;
        protected virtual int StartFrame => 1;
        protected virtual int MaxFrames => 3;
        protected virtual int FrameSpeed => 8;
        protected virtual int FrameWidth => 38;
        protected virtual int FrameHeight => 48;

        protected BaseButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText) : base(spritesheet)
        {
            Button = Assets.Button;
            Spritesheet = spritesheet;
            HoverText = hoverText;
            SetImage(Button);
            currFrame = StartFrame;

            // Add a UIText centered horizontally at the bottom of the button.
            buttonUIText = new ButtonText(buttonText);
            Append(buttonUIText);
        }

        /// <summary>
        /// Draws the button with the specified image/animation and tooltip text
        /// </summary>
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!Active || Button == null || Button.Value == null)
                return;

            // Get the button size from MainState
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            float buttonSize = sys?.mainState?.ButtonSize ?? 70f; // default to 70 if MainState is null

            // Get the dimensions based on the button size.
            CalculatedStyle dimensions = GetInnerDimensions();
            Rectangle drawRect = new((int)dimensions.X, (int)dimensions.Y, (int)buttonSize, (int)buttonSize);
            opacity = IsMouseHovering ? 1f : 0.4f; // Determine opacity based on mouse hover.

            if (!Conf.AnimateButtons)
            {
                opacity = 1f;
            }

            // Set UIText opacity
            buttonUIText.TextColor = Color.White * opacity;

            // Draw the texture with the calculated opacity.
            spriteBatch.Draw(Button.Value, drawRect, Color.White * opacity);

            // Draw the animation texture
            if (Spritesheet != null)
            {
                if (IsMouseHovering && Conf.AnimateButtons)
                {
                    frameCounter++;
                    if (frameCounter >= FrameSpeed)
                    {
                        currFrame++;
                        if (currFrame > MaxFrames)
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
                Vector2 centeredPosition = position + (size - new Vector2(FrameWidth, FrameHeight) * SpriteScale) / 2f;
                Rectangle sourceRectangle = new(x: 0, y: (currFrame - 1) * FrameHeight, FrameWidth, FrameHeight);
                centeredPosition.Y -= 7; // magic offset to move it up a bit

                // Draw the spritesheet.
                spriteBatch.Draw(Spritesheet.Value, centeredPosition, sourceRectangle, Color.White * opacity, 0f, Vector2.Zero, SpriteScale, SpriteEffects.None, 0f);
            }

            // Draw tooltip text if hovering.
            if (IsMouseHovering)
                UICommon.TooltipMouseText(HoverText);
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
                                return false;
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

        public override void MouseOver(UIMouseEvent evt)
        {
            if (Conf.AnimateButtons)
            {
                base.MouseOver(evt);
            }
            // Otherwise, do nothing. The UIElement's IsMouseHovering property will still be set,
            // so your DrawSelf code will properly adjust the opacity.
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