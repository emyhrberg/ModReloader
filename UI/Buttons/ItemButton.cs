using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Panels;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class ItemButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText) : BaseButton(spritesheet, buttonText, hoverText)
    {
        // Set custom animation dimensions
        protected override float SpriteScale => 1.2f;
        protected override int MaxFrames => 5;
        protected override int FrameSpeed => 12;
        protected override int FrameWidth => 40;
        protected override int FrameHeight => 40;

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
                        if (currFrame < MaxFrames) // only increment if not at last frame
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

        public override void LeftClick(UIMouseEvent evt)
        {
            // force open inventory
            Main.playerInventory = true;

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
