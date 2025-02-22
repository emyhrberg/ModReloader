using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    public class BaseFilterButton : UIImageButton
    {
        protected Asset<Texture2D> _Texture;
        public string TooltipText = "";
        public bool Active = true;

        public BaseFilterButton(Asset<Texture2D> texture, string hoverText) : base(texture)
        {
            Width.Set(21, 0f);
            Height.Set(21, 0f);
            MaxWidth.Set(21, 0f);
            MaxHeight.Set(21, 0f);
            Top.Set(50, 0f);

            _Texture = texture;
            TooltipText = hoverText;
            SetImage(_Texture);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (_Texture != null && _Texture.Value != null)
            {
                // Get the button size.
                float buttonSize = 21f;

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
                Main.hoverItemName = TooltipText;
        }

    }
}