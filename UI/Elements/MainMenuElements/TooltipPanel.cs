using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;

namespace ModReloader.UI.Elements.MainMenuElements
{
    public class TooltipPanel : UIPanel
    {
        public bool Hidden = true;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Hidden) return;
            base.Draw(spriteBatch);
        }
    }
}