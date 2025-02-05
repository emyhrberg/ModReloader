using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    internal class RefreshHoverButton : UIImageButton
    {
        private readonly string hoverText;

        // Variables to track dragging.
        private bool dragging;
        private Vector2 dragOffset;

        public RefreshHoverButton(Asset<Texture2D> texture, string hoverText) : base(texture)
        {
            this.hoverText = hoverText;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            // Show tooltip when mouse hovers over the button.
            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText(hoverText);
            }
        }

        // Begin dragging on left mouse button down.
        public override void RightMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            // Set dragging flag and compute the offset between the button's top‐left and the mouse.
            dragging = true;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);

            // This flag prevents the game from also acting on your click.
            Main.LocalPlayer.mouseInterface = true;
        }

        // End dragging on left mouse button up.
        public override void RightMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false;
            Main.LocalPlayer.mouseInterface = false;
            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // If dragging, update the position based on the current mouse position.
            if (dragging)
            {
                Left.Set(Main.mouseX - dragOffset.X, 0f);
                Top.Set(Main.mouseY - dragOffset.Y, 0f);
                Recalculate();
            }
        }
    }
}
