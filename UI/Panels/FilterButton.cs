using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    public class FilterButton : UIImageButton
    {
        // Variables
        private readonly Asset<Texture2D> icon;
        private readonly string HoverText = "";
        public static FilterButton ActiveButton { get; set; }

        public FilterButton(Asset<Texture2D> texture, string hoverText) : base(texture)
        {
            // size and position
            Width.Set(21, 0f);
            Height.Set(21, 0f);
            MaxWidth.Set(21, 0f);
            MaxHeight.Set(21, 0f);
            Top.Set(50, 0f);

            // init stuff
            icon = texture;
            HoverText = hoverText;
            SetImage(icon);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (icon != null && icon.Value != null)
            {
                // Get rectangle dimensions
                Rectangle drawRect = GetDimensions().ToRectangle();
                float opacity = IsMouseHovering ? 1f : 0.4f;

                // If active, draw the active background
                bool isActive = this == ActiveButton;
                if (isActive)
                    spriteBatch.Draw(Assets.FilterBGActive.Value, drawRect, Color.White);
                else
                    spriteBatch.Draw(Assets.FilterBG.Value, drawRect, Color.White);

                // Always draw the icon on top with full opacity if active,
                // otherwise draw with the opacity value.
                if (isActive)
                    spriteBatch.Draw(icon.Value, drawRect, Color.White);
                else
                    spriteBatch.Draw(icon.Value, drawRect, Color.White * opacity);

                // Draw tooltip text if hovering.
                if (IsMouseHovering)
                    Main.hoverItemName = HoverText;
            }

        }
    }
}