using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using SquidTestingMod.Reload;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class ReloadSPButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText) : BaseButton(spritesheet, buttonText, hoverText)
    {
        // Set custom animation dimensions
        protected override int MaxFrames => 5;
        protected override int FrameSpeed => 12;
        protected override int FrameWidth => 65;
        protected override int FrameHeight => 65;

        public async override void LeftClick(UIMouseEvent evt)
        {
            await ReloadUtils.ReloadEverything();
        }

        // If right click, toggle the mode and return
        public override void RightClick(UIMouseEvent evt)
        {
            Active = false;
            buttonUIText.Active = false;

            // set MP active
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            foreach (var btn in sys?.mainState?.AllButtons)
            {
                if (btn is ReloadMPButton spBtn)
                {
                    spBtn.Active = true;
                    spBtn.buttonUIText.Active = true;
                }
            }
            return;
        }
    }
}