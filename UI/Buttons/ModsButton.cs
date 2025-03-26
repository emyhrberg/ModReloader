using System.Linq;
using EliteTestingMod.Common.Configs;
using EliteTestingMod.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace EliteTestingMod.UI.Buttons
{
    public class ModsButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, float textSize) : BaseButton(spritesheet, buttonText, hoverText, textSize)
    {
        // Set button image size
        private float _scale = 0.6f;
        protected override float Scale => _scale;
        protected override int FrameWidth => 55;
        protected override int FrameHeight => 70;

        // Set button image animation
        protected override int FrameCount => 10;
        protected override int FrameSpeed => 4;

        /// <summary>
        /// This is needed because ItemButton is set to end animation at its last frame 5.
        /// </summary>
        /// <param name="spriteBatch"></param>
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!Active || Button == null || Button.Value == null)
                return;

            // Get the button size from MainState
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            float buttonSize = sys?.mainState?.ButtonSize ?? 70f;

            // Get the dimensions based on the button size.
            CalculatedStyle dimensions = GetInnerDimensions();
            Rectangle drawRect = new((int)dimensions.X, (int)dimensions.Y, (int)buttonSize, (int)buttonSize);
            opacity = IsMouseHovering ? 1f : 0.7f; // Determine opacity based on mouse hover.

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
                        if (currFrame < FrameCount) // only increment if not at last frame
                        {
                            currFrame++;
                        }
                        // Otherwise, if currFrame is already MaxFrames, do nothing.
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

            // Draw tooltip text if hovering and HoverText is given (see MainState).
            if (!string.IsNullOrEmpty(HoverText) && IsMouseHovering)
            {
                UICommon.TooltipMouseText(HoverText);
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // Perform actions on leftclick here
            // Retrieve the MainSystem instance and ensure it and its mainState are not null.
            var sys = ModContent.GetInstance<MainSystem>();
            if (sys?.mainState == null)
                return;

            // Retrieve the panels and check for null.
            var allPanels = sys.mainState.RightSidePanels;
            var modsPanel = sys.mainState.modsPanel;
            if (allPanels == null || modsPanel == null)
            {
                Log.Error("ReloadSPButton.RightClick: allPanels or modsPanel is null.");
                return;
            }

            // Close all panels except the modsPanel.
            foreach (var panel in allPanels.Except([modsPanel]))
            {
                if (panel != null && panel.GetActive())
                {
                    panel.SetActive(false);
                }
            }

            // Toggle the modsPanel's active state.
            if (modsPanel.GetActive())
                modsPanel.SetActive(false);
            else
                modsPanel.SetActive(true);
        }

        public override void RightClick(UIMouseEvent evt)
        {
            // Perform actions on rightclick here
        }
    }
}