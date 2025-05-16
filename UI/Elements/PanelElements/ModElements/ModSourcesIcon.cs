using System;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Helpers;
using Terraria.GameContent.UI.Elements;

namespace ModReloader.UI.Elements.PanelElements.ModElements
{
    public class ModSourcesIcon : UIImage
    {
        public Texture2D tex;
        public DateTime lastModified;

        public bool IsHovered => IsMouseHovering;

        public ModSourcesIcon(Texture2D texture, DateTime lastModified = default) : base(texture)
        {
            tex = texture;

            float size = 50f;
            MaxHeight.Set(size, 0f);
            MaxWidth.Set(size, 0f);
            Width.Set(size, 0f);
            Height.Set(size, 0f);
            VAlign = 0.5f;

            // custom top
            Top.Set(-1, 0);
            this.lastModified = lastModified;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw the base image.
            DrawHelper.DrawProperScale(spriteBatch, this, tex, scale: 1.0f);

            // The hover image is drawn in the parent UIPanel class above everything else.
            // This is because the hover image is drawn on top of the icon.
            /// <see cref="OptionPanel"/> Draw() method.
        }
    }
}