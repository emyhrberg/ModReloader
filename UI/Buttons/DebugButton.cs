using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class DebugButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, float textSize) : BaseButton(spritesheet, buttonText, hoverText, textSize)
    {
        // Set custom animation dimensions
        protected override float Scale => 0.55f;
        protected override int FrameCount => 16;
        protected override int FrameSpeed => 4;
        protected override int FrameWidth => 74;
        protected override int FrameHeight => 78;

        public override void LeftClick(UIMouseEvent evt)
        {
            // Get panels
            var sys = ModContent.GetInstance<MainSystem>();
            var allPanels = sys?.mainState?.RightSidePanels;
            var debugPanel = sys?.mainState?.debugPanel;

            // Close other panels
            foreach (var panel in allPanels.Except([debugPanel]))
            {
                if (panel.GetActive())
                {
                    panel.SetActive(false);
                }
            }

            // Toggle this panel
            if (debugPanel.GetActive())
                debugPanel.SetActive(false);
            else
                debugPanel.SetActive(true);
        }
    }
}