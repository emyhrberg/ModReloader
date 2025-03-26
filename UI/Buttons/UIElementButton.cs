using System.Linq;
using EliteTestingMod.Common.Configs;
using EliteTestingMod.Common.Systems;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI;

namespace EliteTestingMod.UI.Buttons
{
    public class UIElementButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, float textSize) : BaseButton(spritesheet, buttonText, hoverText, textSize)
    {
        // Sprite size
        private float _scale = 1.3f;
        protected override float Scale => _scale;
        protected override int FrameWidth => 28;
        protected override int FrameHeight => 24;

        // Sprite animation
        protected override int FrameCount => 4;
        protected override int FrameSpeed => 10;

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

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);

            // Toggle all elements
            UIElementSystem elementSystem = ModContent.GetInstance<UIElementSystem>();
            UIElementState elementState = elementSystem.debugState;
            elementState.ToggleShowAll();
        }
    }
}
