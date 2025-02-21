using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public abstract class BaseFilterButton : UIImageButton
    {
        protected Asset<Texture2D> _Texture;
        public string TooltipText = "";
        public float RelativeLeftOffset = 0f;
        public bool Active = true;

        protected BaseFilterButton(Asset<Texture2D> texture, string hoverText) : base(texture)
        {
            _Texture = texture;
            TooltipText = hoverText;
            SetImage(_Texture);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
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
    }
}