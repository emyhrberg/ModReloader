using System;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Helpers;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModReloader.UI.Elements.PanelElements
{
    public class ResizeIcon : UIImageButton
    {
        private Asset<Texture2D> Texture;
        public bool draggingResize;
        private float clickOffsetX, clickOffsetY;

        // Fired every frame while dragging, passing how far we moved in Y
        public event Action<float> OnDragX;                 // <─ add X-callback
        public event Action<float> OnDragY;

        public ResizeIcon(Asset<Texture2D> texture) : base(texture)
        {
            // Position the button inside the bottom-right corner
            HAlign = 1f;
            VAlign = 1f;
            Left.Set(18, 0);
            Top.Set(16, 0);

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
            clickOffsetX = evt.MousePosition.X - GetDimensions().X;   // <─ new
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
                    // Y-offset
                    float newTop = Main.MouseScreen.Y - clickOffsetY;
                    float offsetY = newTop - GetDimensions().Y;
                    OnDragY?.Invoke(offsetY);

                    // X-offset
                    float newLeft = Main.MouseScreen.X - clickOffsetX; // <─ new
                    float offsetX = newLeft - GetDimensions().X;       // <─ new
                    OnDragX?.Invoke(offsetX);                          // <─ fire X
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            DrawHelper.DrawProperScale(spriteBatch, element: this, tex: Texture.Value, scale: 0.7f);
        }
    }
}