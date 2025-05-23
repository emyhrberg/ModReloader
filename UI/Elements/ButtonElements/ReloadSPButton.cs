using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Systems;
using ReLogic.Content;
using Terraria.UI;

namespace ModReloader.UI.Elements.ButtonElements
{
    public class ReloadSPButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        protected override int FrameCount => 5;
        protected override int FrameSpeed => 12;
        protected override int FrameWidth => 65;
        protected override int FrameHeight => 65;
        protected override float BaseAnimScale => 0.8f;
        public async override void LeftClick(UIMouseEvent evt)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (!sys.mainState.AreButtonsShowing) return;
            await ReloadUtilities.SinglePlayerReload();
        }

        public override async void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);

            if (!Conf.C.RightClickToolOptions) return;

            ReloadUtilities.forceJustReload = true;
            await ReloadUtilities.SinglePlayerReload();
        }
    }
}