using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Elements;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class ItemButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, float textSize) : BaseButton(spritesheet, buttonText, hoverText, textSize)
    {
        // Set custom animation dimensions
        protected override float Scale => 1.2f;
        protected override int FrameCount => 5;
        protected override int FrameSpeed => 12;
        protected override int FrameWidth => 40;
        protected override int FrameHeight => 40;

        /// <summary>
        /// This is needed because ItemButton is set to end animation at its last frame 5.
        /// </summary>
        /// <param name="spriteBatch"></param>
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Conf.HideCollapseButton && !Main.playerInventory)
                return;

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
            // force open inventory
            // Main.playerInventory = true;

            // Close the NPCSpawnerPanel.
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            var npcSpawnerPanel = sys?.mainState?.npcSpawnerPanel;
            if (npcSpawnerPanel != null && npcSpawnerPanel.GetActive())
            {
                npcSpawnerPanel.SetActive(false);
            }

            // Toggle the ItemsPanel.
            ItemSpawner itemSpawnerPanel = sys?.mainState?.itemSpawnerPanel;
            if (itemSpawnerPanel != null)
            {
                if (itemSpawnerPanel.GetActive())
                {
                    itemSpawnerPanel.SetActive(false);
                }
                else
                {
                    itemSpawnerPanel.SetActive(true);

                    // focus on the search bar
                    itemSpawnerPanel.GetCustomTextBox()?.Focus();
                }
            }
        }
    }
}
