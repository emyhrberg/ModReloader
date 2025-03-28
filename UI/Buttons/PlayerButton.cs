using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Players;
using ModHelper.UI.Elements;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class PlayerButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set custom animation dimensions
        private float _scale = 0.9f;
        protected override float Scale => _scale;
        protected override int StartFrame => 3;
        protected override int FrameCount => 17;
        protected override int FrameSpeed => 5;
        protected override int FrameWidth => 44;
        protected override int FrameHeight => 54;

        public override void LeftClick(UIMouseEvent evt)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            PlayerPanel panel = sys?.mainState?.playerPanel;
            List<DraggablePanel> rightSidePanels = sys?.mainState?.RightSidePanels;

            // Disable all other panels
            foreach (var p in rightSidePanels.Except([panel]))
            {
                if (p != panel && p.GetActive())
                {
                    p.SetActive(false);
                }
            }

            // Toggle the player panel
            if (panel.GetActive())
            {
                panel.SetActive(false);
                ParentActive = false;
            }
            else
            {
                ParentActive = true;
                panel.SetActive(true);
            }

            // Disable World, Log, UI, Mods buttons
            foreach (var button in sys.mainState.AllButtons)
            {
                if (button is WorldButton || button is LogButton || button is UIElementButton || button is ModsButton)
                {
                    button.ParentActive = false;
                }
            }
        }

        public override void RightClick(UIMouseEvent evt)
        {
            //base.RightClick(evt);

            // Toggle super mode
            PlayerCheatManager p = Main.LocalPlayer.GetModPlayer<PlayerCheatManager>();

            p.SuperMode = !p.SuperMode;
            if (p.SuperMode)
                p.EnableSupermode();
            else
                p.DisableSupermode();
        }
    }
}
