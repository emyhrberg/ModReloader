using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    public class ResizeButton : UIImageButton
    {
        private Asset<Texture2D> Texture;
        public bool draggingResize;
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
            draggingResize = true;
            clickOffsetY = evt.MousePosition.Y - GetDimensions().Y;
            // Main.LocalPlayer.mouseInterface = true;
        }

        // Called when the user releases the left mouse button
        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            draggingResize = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // If we are dragging, keep sending offset events
            if (draggingResize)
            {
                // If the mouse was released outside this UI, stop
                if (!Main.mouseLeft)
                {
                    draggingResize = false;
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
            DrawHelper.DrawProperScale(spriteBatch, element: this, tex: Texture.Value, scale: 0.7f);
        }
    }
}
