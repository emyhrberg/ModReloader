using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;

namespace ModReloader.UI.Elements.PanelElements
{
    public class OptionIcon : UIImage
    {
        public Texture2D tex;

        public bool IsHovered => IsMouseHovering;

        public OptionIcon(Texture2D texture) : base(texture)
        {
            tex = texture;

            float size = 25f;
            MaxHeight.Set(size, 0f);
            MaxWidth.Set(size, 0f);
            Width.Set(size, 0f);
            Height.Set(size, 0f);
            VAlign = 0.5f;

            // custom top
            Top.Set(-1, 0);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw the base image
            //DrawHelper.DrawProperScale(spriteBatch, this, tex, scale: 1.0f);
        }
    }
}