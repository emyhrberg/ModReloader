using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using ModHelper.UI.AbstractElements;
using ReLogic.Content;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    public class ReloadMPButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        public async override void LeftClick(UIMouseEvent evt)
        {
            await ReloadUtilities.MultiPlayerMainReload();
        }
    }
}