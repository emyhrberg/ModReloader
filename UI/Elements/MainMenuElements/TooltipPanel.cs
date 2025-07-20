using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;

namespace ModReloader.UI.Elements.MainMenuElements
{
    public class TooltipPanel : UIPanel
    {
        public bool Hidden = true;
        public UIText TextElement;
        public string Text
        {
            get => TextElement.Text;
            set => TextElement.SetText(value);
        }
        public TooltipPanel()
        {
            BorderColor = new Color(33, 43, 79) * 0.8f;
            BackgroundColor = new Color(73, 94, 171);
            Left.Set(0, 0f); // -3 panel padding on each side is 6/2 = 3
            Top.Set(10, 0f);

            // arbitrary size
            Width.Set(310f, 0f);
            Height.Set(68f, 0f);

            // text
            TextElement = new(string.Empty, 0.9f);
            TextElement.Left.Set(0f, 0f);
            TextElement.TextOriginX = 0;
            TextElement.TextOriginY = 0;
            Append(TextElement);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Hidden) return;
            base.Draw(spriteBatch);
        }
    }
}