using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class UIButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText) : BaseButton(spritesheet, buttonText, hoverText)
    {
        protected override float SpriteScale => 0.1f;
        protected override int FrameWidth => 512;
        protected override int FrameHeight => 512;

        public override void LeftClick(UIMouseEvent evt)
        {
            // Toggle uiPanel
            var sys = ModContent.GetInstance<MainSystem>();
            var allPanels = sys?.mainState?.RightSidePanels;
            var uiPanel = sys?.mainState?.uiPanel;

            // Close other panels
            foreach (var panel in allPanels.Except([uiPanel]))
            {
                if (panel.GetActive())
                {
                    panel.SetActive(false);
                }
            }

            // Toggle uiPanel
            if (uiPanel.GetActive())
                uiPanel.SetActive(false);
            else
                uiPanel.SetActive(true);
        }
    }
}