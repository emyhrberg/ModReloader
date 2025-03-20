using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class ModsButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText) : BaseButton(spritesheet, buttonText, hoverText)
    {
        // Set button image size
        protected override float SpriteScale => 0.7f; // 250 * 0.1 = 25f (reasonable, button is 70f)
        protected override int FrameWidth => 100;
        protected override int FrameHeight => 100;

        public override void LeftClick(UIMouseEvent evt)
        {
            // Perform actions on leftclick here
            // Retrieve the MainSystem instance and ensure it and its mainState are not null.
            var sys = ModContent.GetInstance<MainSystem>();
            if (sys?.mainState == null)
                return;

            // Retrieve the panels and check for null.
            var allPanels = sys.mainState.RightSidePanels;
            var modsPanel = sys.mainState.modsPanel;
            if (allPanels == null || modsPanel == null)
            {
                Log.Error("ReloadSPButton.RightClick: allPanels or modsPanel is null.");
                return;
            }

            // Close all panels except the modsPanel.
            foreach (var panel in allPanels.Except([modsPanel]))
            {
                if (panel != null && panel.GetActive())
                {
                    panel.SetActive(false);
                }
            }

            // Toggle the modsPanel's active state.
            if (modsPanel.GetActive())
                modsPanel.SetActive(false);
            else
                modsPanel.SetActive(true);
        }

        public override void RightClick(UIMouseEvent evt)
        {
            // Perform actions on rightclick here
        }
    }
}