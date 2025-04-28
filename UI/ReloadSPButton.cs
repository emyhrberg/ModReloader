using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Common.Systems;
using ModHelper.Helpers;
using ModHelper.UI.Elements.AbstractElements;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI
{
    public class ReloadSPButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set custom animation dimensions
        private float _scale = 0.8f;
        protected override int FrameCount => 5;
        protected override int FrameSpeed => 12;
        protected override int FrameWidth => 65;
        protected override int FrameHeight => 65;
        protected override float Scale => _scale;

        public async override void LeftClick(UIMouseEvent evt)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (!sys.mainState.isClick && Conf.C.DragButtons == "Left")
            {
                return;
            }

            await ReloadUtilities.SinglePlayerReload();


        }

        public override void RightClick(UIMouseEvent evt)
        {
            if (!Conf.C.RightClickButtonToExitWorld)
            {
                return;
            }

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (!sys.mainState.isClick)
            {
                return;
            }

            if (Conf.C.SaveWorldBeforeReloading)
            {
                WorldGen.SaveAndQuit();
            }
            else
            {
                WorldGen.JustQuit();
            }
            Main.menuMode = 10001;
        }
    }
}