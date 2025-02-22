using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Panels;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class NPCSpawnerButton : BaseButton
    {
        public NPCSpawnerButton(Asset<Texture2D> _image, string hoverText) : base(_image, hoverText)
        {
            // 50% chance to switch asset to Xmas version
            if (Main.rand.NextBool())
            {
                zombie = Assets.ButtonNPCSS_XMAS;
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // Close the ItemSpawnerPanel.
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            var itemSpawnerPanel = sys?.mainState?.itemSpawnerPanel;
            if (itemSpawnerPanel != null && itemSpawnerPanel.GetActive())
            {
                itemSpawnerPanel.SetActive(false);
            }

            // Toggle the NPCSpawnerPanel.
            NPCSpawnerPanel npcSpawnerPanel = sys?.mainState?.npcSpawnerPanel;

            if (npcSpawnerPanel != null)
            {
                if (npcSpawnerPanel.GetNPCPanelActive())
                {
                    npcSpawnerPanel.SetNPCPanelActive(false);
                }
                else
                {
                    npcSpawnerPanel.SetNPCPanelActive(true);
                }
            }
        }

        // --------------------- Drawing ---------------------
        // Animation frames
        private int currFrame = 1;
        private int maxFrame = 3;
        private int frameCounter = 0;
        private int frameSpeed = 8; // lower is faster. 3 is fast, 8 is slow

        // Animation texture
        private Asset<Texture2D> zombie = Assets.ButtonNPCSS;
        private int frameWidth = 38;
        private int frameHeight = 48;
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
            float scale = 0.85f;
            Vector2 position = GetDimensions().Position();
            Vector2 size = new Vector2(GetDimensions().Width, GetDimensions().Height);
            Vector2 centeredPosition = position + (size - new Vector2(frameWidth, frameHeight) * scale) / 2f;
            centeredPosition.Y -= 7; // magic number to move up a bit

            spriteBatch.Draw(
                texture: zombie.Value,
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
            frameCounter++;
            if (frameCounter >= frameSpeed)
            {
                frameCounter = 0;
                currFrame++; // advance frame so that we show the next frame
                if (currFrame >= maxFrame)
                {
                    currFrame = 0;
                }
            }

            // uncomment this to loop the animation
            // I only want to play it once though
            // frameCounter++;
            // if (frameCounter >= frameSpeed)
            // {
            //     currFrame++;
            //     if (currFrame >= maxFrame)
            //     {
            //         currFrame = 0;
            //     }

            //     frameCounter = 0; 
            // }
        }
    }
}