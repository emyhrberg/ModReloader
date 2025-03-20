using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class PlayerButton(Asset<Texture2D> image, string buttonText, string hoverText) : BaseButton(image, buttonText, hoverText)
    {
        // Set custom animation dimensions
        protected override float SpriteScale => 0.95f;
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
