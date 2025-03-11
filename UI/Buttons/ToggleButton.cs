using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class ToggleButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText) : BaseButton(spritesheet, buttonText, hoverText)
    {
        // Custom button logic
        protected override int FrameWidth => 100;
        protected override int FrameHeight => 50;
        protected override int MaxFrames => 2;

        public override void Draw(SpriteBatch spriteBatch)
        {
            // get mainsystem and toggle state
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            bool areButtonsShowing = sys?.mainState.AreButtonsShowing ?? true;

            // draw base button
            if (areButtonsShowing)
            {
                // THIS ENTIRE IF STATEMENT CAN BE COMMENTED OUT
                // IF WE PREFER THE LOOK OF NO BUTTON BESIDE THE TOGGLE
                if (!Active || Button == null || Button.Value == null)
                    return;

                // Get the button size from MainState
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
            }

            // set opacity
            opacity = IsMouseHovering ? 1f : 0.4f; // Determine opacity based on mouse hover.

            // determine source rectangle based on the toggle state
            Rectangle source = new Rectangle(0, 0, FrameWidth, FrameHeight);
            if (!areButtonsShowing)
            {
                source.Y = FrameHeight; // use bottom half for "off" state
            }

            // get destination rectangle from the button dimensions
            Rectangle dest = GetDimensions().ToRectangle();

            // set scale factor for the toggle texture
            float scale = 0.7f;

            // Calculate drawn size after scaling
            Vector2 drawnSize = new Vector2(FrameWidth * scale, FrameHeight * scale);

            // Calculate the position so the texture is centered within dest
            Vector2 centerPosition = new Vector2(
                dest.X + (dest.Width - drawnSize.X) / 2,
                dest.Y + (dest.Height - drawnSize.Y) / 2);

            // draw the toggle texture at the computed centered position
            spriteBatch.Draw(Spritesheet.Value, centerPosition, source, Color.White * opacity, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // Draw tooltip text if hovering.
            if (IsMouseHovering)
                UICommon.TooltipMouseText(HoverText);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            sys?.mainState.ToggleCollapse();
        }

        #region Dragging
        private bool dragging;
        private Vector2 dragOffset;
        private Vector2 mouseDownPos;
        private bool isDrag;
        private const float DragThreshold = 10f; // you can tweak the threshold
        public Vector2 anchorPos;

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            dragging = true;
            isDrag = false;
            mouseDownPos = evt.MousePosition; // store mouse down location
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false;
            Recalculate();

            if (isDrag)
            {
                LeftClick(evt);
            }
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (dragging)
            {
                // Check if we moved the mouse more than the threshold
                var currentPos = new Vector2(Main.mouseX, Main.mouseY);
                if (Vector2.Distance(currentPos, mouseDownPos) > DragThreshold)
                {
                    isDrag = true;
                }

                // get anchor pos of this button based on what we have dragOffset it with
                anchorPos = new(Main.mouseX - dragOffset.X, Main.mouseY - dragOffset.Y);

                // update the position of all the buttons
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                // sys?.mainState.UpdateButtonsPositions(anchorPos);

                Recalculate();
            }
        }
        #endregion
    }
}