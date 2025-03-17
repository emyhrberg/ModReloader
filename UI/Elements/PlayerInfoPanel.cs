using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    public class PlayerInfoPanel : UIPanel
    {
        // Dragging
        private bool dragging;
        private Vector2 dragOffset;

        public bool Active { get; set; }
        UIText pos = new UIText("Position: (0, 0)");
        protected Color darkBlueLowAlpha = new(73, 85, 186, 100);

        public PlayerInfoPanel()
        {
            Active = true;
            VAlign = 0.5f; // top aligned
            HAlign = 0.5f;
            Top.Set(190, 0f); // move to below player
            Height.Set(250, 0f);
            Width.Set(250, 0f);
            // BackgroundColor = darkBlueLowAlpha;

            // add position info
            pos.Top.Set(0f, 0f);
            Append(pos);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
                return;

            base.Draw(spriteBatch);
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
            // Disable item use if the mouse is over the panel
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (!Active)
                return;

            base.Update(gameTime);

            // update position info
            pos.SetText($"(x, y): ({(int)Main.LocalPlayer.position.X}, {(int)Main.LocalPlayer.position.Y})");

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