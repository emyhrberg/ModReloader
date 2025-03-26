using System.Linq;
using ErkysModdingUtilities.Common.Systems;
using ErkysModdingUtilities.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ErkysModdingUtilities.UI.Buttons
{
    public class PlayerButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, float textSize) : BaseButton(spritesheet, buttonText, hoverText, textSize)
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
            // Toggle player panel
            var sys = ModContent.GetInstance<MainSystem>();
            var allPanels = sys?.mainState?.RightSidePanels;
            var playerPanel = sys?.mainState?.playerPanel;

            // Close other panels
            foreach (var panel in allPanels.Except([playerPanel]))
            {
                if (panel.GetActive())
                {
                    panel.SetActive(false);
                }
            }

            // Toggle player panel
            if (playerPanel.GetActive())
                playerPanel.SetActive(false);
            else
                playerPanel.SetActive(true);
        }
    }
}
