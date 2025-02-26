using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // 1) Detect start of dragging
            if (ContainsPoint(Main.MouseScreen))
            {
                // If left mouse pressed while hovering, start dragging
                if (Main.mouseLeft && !dragging)
                {
                    dragging = true;
                    // Remember how far down from the top of the button the user clicked
                    clickOffsetY = Main.MouseScreen.Y - GetDimensions().Y;
                }
            }

            // 2) If mouse released, stop dragging
            if (!Main.mouseLeft)
                dragging = false;

            // 3) If still dragging, calculate vertical offset
            if (dragging)
            {
                // Where the button’s top is now
                float newTop = Main.MouseScreen.Y - clickOffsetY;
                float offsetY = newTop - GetDimensions().Y;
                // Fire event so the parent panel can respond
                OnDragY?.Invoke(offsetY);
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
                float opacity = IsMouseHovering ? 1f : 0.6f;

                // Draw the texture at 'center', anchored by 'origin', scaled by 'scale'
                spriteBatch.Draw(Texture.Value, center, null, Color.White * opacity, 0f, origin, scale, SpriteEffects.None, 0f);
            }
        }
    }
}
