using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    public class ResizeButton : UIImageButton
    {
        private Asset<Texture2D> Texture;
        private bool dragging;
        private float clickOffsetY;

        // Fired every frame while dragging, passing how far we moved in Y
        public event Action<float> OnDragY;

        public ResizeButton(Asset<Texture2D> texture) : base(texture)
        {
            // Position the button inside the bottom-right corner
            HAlign = 1f;
            VAlign = 1f;
            Left.Set(12f, 0f);  // 35 is the button size
            Top.Set(12f, 0f);

            Width.Set(35, 0f);
            Height.Set(35, 0f);

            Texture = texture;
        }

        // Called when the user presses the left mouse button on this element
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);  // needed for correct event handling

            // We only start dragging if the user explicitly clicked this button
            dragging = true;
            clickOffsetY = evt.MousePosition.Y - GetDimensions().Y;
            // Main.LocalPlayer.mouseInterface = true;
        }

        // Called when the user releases the left mouse button
        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // If we are dragging, keep sending offset events
            if (dragging)
            {
                // If the mouse was released outside this UI, stop
                if (!Main.mouseLeft)
                {
                    dragging = false;
                }
                else
                {
                    float newTop = Main.MouseScreen.Y - clickOffsetY;
                    float offsetY = newTop - GetDimensions().Y;
                    OnDragY?.Invoke(offsetY);
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Texture?.Value != null)
            {
                // Get center of the UI element
                Vector2 center = GetDimensions().ToRectangle().Center.ToVector2();

                // Get center of the texture
                Vector2 origin = new(Texture.Width() / 2f, Texture.Height() / 2f);

                // Set scale
                float scale = 1f;

                // Set opacity: 0.8 when not hovering, 1 when hovering
                float opacity = IsMouseHovering ? 1f : 0.8f;

                // Draw the texture at 'center', anchored by 'origin', scaled by 'scale'
                spriteBatch.Draw(Texture.Value, center, null, Color.White * opacity, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
