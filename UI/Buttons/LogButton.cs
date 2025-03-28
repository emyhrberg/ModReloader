using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using ModHelper.UI.Elements;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class LogButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set custom animation dimensions
        private float _scale = 0.5f;
        protected override float Scale => _scale;
        protected override int FrameCount => 16;
        protected override int FrameSpeed => 4;
        protected override int FrameWidth => 74;
        protected override int FrameHeight => 78;

        public override void LeftClick(UIMouseEvent evt)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            List<DraggablePanel> rightSidePanels = sys?.mainState?.RightSidePanels;

            // replace with THIS panel
            var panel = sys?.mainState?.logPanel;

            // Disable all other panels
            foreach (var p in rightSidePanels.Except([panel]))
            {
                if (p != panel && p.GetActive())
                {
                    p.SetActive(false);
                }
            }

            // Toggle the log panel
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
                if (button is PlayerButton || button is WorldButton || button is UIElementButton || button is ModsButton)
                {
                    button.ParentActive = false;
                }
            }
        }

        public override void RightClick(UIMouseEvent evt)
        {
            // Open client log instantly
            Log.OpenClientLog();
        }
    }
}