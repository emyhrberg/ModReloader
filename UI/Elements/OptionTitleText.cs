using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace SquidTestingMod.UI.Elements
{
    public class OptionTitleText : UIText
    {
        private string hover = "";

        public OptionTitleText(string text, string hover = "") : base(text)
        {
            this.hover = hover;
            Left.Set(30, 0);
            VAlign = 0.5f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (!string.IsNullOrEmpty(hover) && IsMouseHovering)
            {
                UICommon.TooltipMouseText(hover);
            }
        }
    }
}