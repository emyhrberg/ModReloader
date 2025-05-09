using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Systems;
using ModHelper.Helpers;
using ReLogic.Content;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    public class ReloadMPButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        public async override void LeftClick(UIMouseEvent evt)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (!sys.mainState.AreButtonsShowing)
            {
                return;
            }

            await ReloadUtilities.MultiPlayerMainReload();
        }
    }
}