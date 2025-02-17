using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public abstract class BaseButton : UIImageButton
    {
        protected Asset<Texture2D> _Texture;
        public string TooltipText = "";
        public float RelativeLeftOffset = 0f;
        public bool Active = true;

        protected BaseButton(Asset<Texture2D> texture, string hoverText) : base(texture)
        {
            _Texture = texture;
            TooltipText = hoverText;
            SetImage(_Texture);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!Active)
                return;

            if (_Texture != null && _Texture.Value != null)
            {
                // Get the forced button size from MainState (default to 70 if not set)
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                float buttonSize = sys?.mainState?.ButtonSize ?? 70f;

                // Determine opacity based on mouse hover.
                float opacity = IsMouseHovering ? 1f : 0.4f;

                // Get the dimensions based on the button size.
                CalculatedStyle dimensions = GetInnerDimensions();
                Rectangle drawRect = new((int)dimensions.X, (int)dimensions.Y, (int)buttonSize, (int)buttonSize);

                // Draw the texture with the calculated opacity.
                spriteBatch.Draw(_Texture.Value, drawRect, Color.White * opacity);
            }

            // Draw tooltip text if hovering.
            if (IsMouseHovering)
                UICommon.TooltipMouseText(TooltipText);
        }

        public virtual void UpdateTexture()
        {
            SetImage(_Texture);
        }

        // Dont allow clicking if button is disabled.
        public override bool ContainsPoint(Vector2 point)
        {
            if (!Active)
                return false;

            return base.ContainsPoint(point);
        }

        // Disable use on button click
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            Main.LocalPlayer.mouseInterface = true;
            base.LeftMouseDown(evt);
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            Main.LocalPlayer.mouseInterface = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
    }
}