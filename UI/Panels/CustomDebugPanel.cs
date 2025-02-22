using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    public class CustomDebugPanel : UIPanel
    {
        private bool dragging;
        private Vector2 dragOffset;

        public CustomDebugPanel(int width, int height)
        {
            Width.Set(width, 0f);
            Height.Set(height, 0f);
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);
            dragging = true;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (dragging)
            {
                Left.Set(Main.mouseX - dragOffset.X, 0f);
                Top.Set(Main.mouseY - dragOffset.Y, 0f);
                Recalculate();
            }
        }
    }
}
