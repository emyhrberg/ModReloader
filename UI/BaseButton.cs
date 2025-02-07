using log4net.Repository.Hierarchy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public abstract class BaseButton : UIImageButton
    {
        private readonly Asset<Texture2D> _buttonImgText;
        private readonly Asset<Texture2D> _buttonImgNoText;

        public string HoverText { get; private set; }

        public BaseButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText)
            : base(buttonImgText) // Default to the text version
        {
            _buttonImgText = buttonImgText;
            _buttonImgNoText = buttonImgNoText;
            HoverText = hoverText;

            // Ensure the correct initial texture is set
            UpdateTexture();
        }

        public virtual void UpdateTexture()
        {
            Config config = ModContent.GetInstance<Config>();
            bool showText = config?.ShowButtonText ?? true; // Default to true if config is null
            SetImage(showText ? _buttonImgText : _buttonImgNoText);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // base draws all buttons with hover effect
            base.DrawSelf(spriteBatch);

            if (IsMouseHovering)
            {
                Config c = ModContent.GetInstance<Config>();
                if (c.ShowTooltips)
                    UICommon.TooltipMouseText(HoverText);
            }
        }

        public override bool ContainsPoint(Vector2 point)
        {
            // Define a custom clickable area, e.g., make it 20 pixels bigger in all directions
            float extraSize = 20f;

            Rectangle customArea = new Rectangle(
                (int)(GetDimensions().X - extraSize),
                (int)(GetDimensions().Y - extraSize),
                (int)(Width.Pixels + extraSize * 2),
                (int)(Height.Pixels + extraSize * 2)
            );

            return customArea.Contains(point.ToPoint());
        }

        // Abstract HandleClick = force children (all buttons) to implement this method
        public abstract void HandleClick();

        #region dragging
        private bool DRAG_ENABLED = false;

        private bool dragging;
        private Vector2 dragOffset;
        public override void RightMouseDown(UIMouseEvent evt)
        {
            if (!DRAG_ENABLED)
                return;

            base.LeftMouseDown(evt);
            dragging = true;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
            Main.LocalPlayer.mouseInterface = true;
        }

        public override void RightMouseUp(UIMouseEvent evt)
        {
            if (!DRAG_ENABLED)
                return;
            base.LeftMouseUp(evt);
            dragging = false;
            Main.LocalPlayer.mouseInterface = false;
            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (!DRAG_ENABLED)
                return;
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