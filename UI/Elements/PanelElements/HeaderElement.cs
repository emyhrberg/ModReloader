namespace ModReloader.UI.Elements.PanelElements
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Terraria.GameContent;
    using Terraria.ModLoader.UI;
    using Terraria.UI;
    using Terraria.UI.Chat;

    public class HeaderElement : UIElement
    {
        public readonly string header;
        private readonly string hoverText;

        public HeaderElement(string header, string hover)
        {
            this.header = header;
            Vector2 stringSize = ChatManager.GetStringSize(FontAssets.ItemStack.Value, this.header, Vector2.One, 532f);
            Width.Set(0f, 1f);
            Height.Set(stringSize.Y + 6f, 0f);
            hoverText = hover;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = GetDimensions();
            float num = dimensions.Width + 1f;
            Vector2 position = new Vector2(dimensions.X, dimensions.Y) + new Vector2(8f);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)dimensions.X + 10, (int)dimensions.Y + (int)dimensions.Height - 2, (int)dimensions.Width - 20, 1), Color.LightGray);
            ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, header, position, Color.White, 0f, Vector2.Zero, new Vector2(1f), num - 20f);

            if (hoverText != "" && IsMouseHovering)
            {
                UICommon.TooltipMouseText(hoverText);
            }
        }
    }
}