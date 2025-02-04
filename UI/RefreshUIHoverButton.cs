using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace SquidTestingMod.UI
{
    internal class RefreshUIHoverButton : UIImageButton
    {
        private readonly string hoverText;

        public RefreshUIHoverButton(Asset<Texture2D> texture, string hoverText) : base(texture)
        {
            this.hoverText = hoverText;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText(hoverText);
            }
        }
    }
}
