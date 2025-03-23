using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class WorldButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, float textSize) : BaseButton(spritesheet, buttonText, hoverText, textSize)
    {
        // Spinning globe pixel art
        // protected override float Scale => 0.08f;
        // protected override int FrameWidth => 1080;
        // protected override int FrameHeight => 576;
        // protected override int FrameCount => 11;
        // protected override int FrameSpeed => 7;

        // Old pylon texture, do not delete
        private float _scale = 0.58f;
        protected override float Scale => _scale;
        protected override int FrameCount => 8;
        protected override int FrameSpeed => 7;
        protected override int FrameWidth => 48;
        protected override int FrameHeight => 68;

        public override void LeftClick(UIMouseEvent evt)
        {
            // Get panels
            var sys = ModContent.GetInstance<MainSystem>();
            var allPanels = sys?.mainState?.RightSidePanels;
            var worldPanel = sys?.mainState?.worldPanel;

            // Close other panels
            foreach (var panel in allPanels.Except([worldPanel]))
            {
                if (panel.GetActive())
                {
                    panel.SetActive(false);
                }
            }

            // Toggle this panel
            if (worldPanel.GetActive())
                worldPanel.SetActive(false);
            else
                worldPanel.SetActive(true);
        }
    }
}
