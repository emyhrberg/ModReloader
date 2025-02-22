using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class WorldButton : BaseButton
    {
        public WorldButton(Asset<Texture2D> image, string hoverText) : base(image, hoverText)
        {
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // Toggle world panel
            var sys = ModContent.GetInstance<MainSystem>();
            var worldPanel = sys?.mainState?.worldPanel;
            var playerPanel = sys?.mainState?.playerPanel;
            var debugPanel = sys?.mainState?.debugPanel;

            // Close other panels
            if (playerPanel != null && playerPanel.GetActive())
            {
                playerPanel.SetActive(false);
            }
            if (debugPanel != null && debugPanel.GetActive())
            {
                debugPanel.SetActive(false);
            }

            // Toggle world panel
            if (worldPanel != null)
            {
                if (worldPanel.GetActive())
                {
                    worldPanel.SetActive(false);
                }
                else
                {
                    worldPanel.SetActive(true);
                }
            }
        }

        // --------------------- Drawing ---------------------
        // Animation frames
        private int currFrame = 1;
        private int maxFrame = 8;
        private int frameCounter = 0;
        private int frameSpeed = 7; // lower is faster. 3 is fast, 8 is slow

        // Animation texture
        private Asset<Texture2D> player = Assets.ButtonWorldSS;
        private int frameWidth = 48;
        private int frameHeight = 68;
        private bool wasHovering = false;

        public override void Draw(SpriteBatch spriteBatch)
        {
            // draw base button
            base.Draw(spriteBatch);

            // draw sprite sheet animation if hovering, otherwise draw first frame

            // set source rectangle
            Rectangle sourceRectangle = new Rectangle(
                x: 0,
                y: currFrame * frameHeight,
                width: frameWidth,
                height: frameHeight
            );

            // if not hovering, draw first frame
            if (!IsMouseHovering)
            {
                sourceRectangle = new Rectangle(
                    x: 0,
                    y: 0,
                    width: frameWidth,
                    height: frameHeight
                );
            }
            else
            {
                if (!wasHovering)
                {
                    currFrame = 0; // reset frame to 0 if hovering
                }
            }
            wasHovering = IsMouseHovering;

            // calculate position to center the sprite
            float scale = 0.6f;
            Vector2 position = GetDimensions().Position();
            Vector2 size = new Vector2(GetDimensions().Width, GetDimensions().Height);
            Vector2 centeredPosition = position + (size - new Vector2(frameWidth, frameHeight) * scale) / 2f;
            centeredPosition.Y -= 7; // magic number to move up a bit

            spriteBatch.Draw(
                texture: player.Value,
                position: centeredPosition,
                sourceRectangle: sourceRectangle,
                color: Color.White * opacity,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: scale,
                effects: SpriteEffects.None,
                layerDepth: 0f
            );

            // update currFrame
            // frameCounter++;
            // if (frameCounter >= frameSpeed)
            // {
            //     frameCounter = 0;
            //     if (currFrame < maxFrame - 1)
            //     {
            //         currFrame++;
            //     }
            // }

            // uncomment this to loop the animation
            frameCounter++;
            if (frameCounter >= frameSpeed)
            {
                currFrame++;
                if (currFrame >= maxFrame)
                {
                    currFrame = 0;
                }

                frameCounter = 0;
            }
        }
    }
}
