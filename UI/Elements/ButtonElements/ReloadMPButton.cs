using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Systems;
using ReLogic.Content;
using Terraria.UI;

namespace ModReloader.UI.Elements.ButtonElements
{
    public class ReloadMPButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set custom animation dimensions
        protected override float BaseAnimScale => 1.15f;
        protected override int FrameCount => 5;
        protected override int FrameSpeed => 12;
        protected override int FrameWidth => 65;
        protected override int FrameHeight => 65;
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