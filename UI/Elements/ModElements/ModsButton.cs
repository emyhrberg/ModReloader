using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Common.Systems;
using ModHelper.UI.Elements.AbstractElements;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI.Elements.ModElements
{
    public class ModsButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set button image size
        private float _scale = 0.9f;
        protected override float Scale => _scale;

        // OLD BUTTON, DO NOT DELETE
        // protected override int FrameWidth => 55;
        // protected override int FrameHeight => 70;
        // protected override int FrameCount => 10;
        // protected override int FrameSpeed => 4;

        protected override int FrameWidth => 60;
        protected override int FrameHeight => 58;

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