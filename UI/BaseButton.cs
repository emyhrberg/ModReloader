using log4net.Repository.Hierarchy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public abstract class BaseButton(Asset<Texture2D> texture, string hoverText) : UIImageButton(texture)
    {
        public string HoverText { get; private set; } = hoverText;

        public bool Visible { get; private set; }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText(HoverText);
            }
        }

        // Abstract = force children to implement this method
        public abstract void HandleClick();

        #region dragging
        private bool dragging;
        private Vector2 dragOffset;
        public override void RightMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);
            dragging = true;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
            Main.LocalPlayer.mouseInterface = true;
        }

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
            if (dragging)
            {
                Left.Set(Main.mouseX - dragOffset.X, 0f);
                Top.Set(Main.mouseY - dragOffset.Y, 0f);
                Recalculate();
            }
        }
        #endregion
    }
}