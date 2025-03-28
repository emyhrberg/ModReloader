using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using ModHelper.UI.Elements;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class ModsButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set button image size
        private float _scale = 0.6f;
        protected override float Scale => _scale;
        protected override int FrameWidth => 55;
        protected override int FrameHeight => 70;

        // Set button image animation
        protected override int FrameCount => 10;
        protected override int FrameSpeed => 4;

        public override void LeftClick(UIMouseEvent evt)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            List<DraggablePanel> rightSidePanels = sys?.mainState?.RightSidePanels;

            // replace with THIS panel
            var panel = sys?.mainState?.modsPanel;

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
                if (button is PlayerButton || button is WorldButton || button is UIElementButton || button is LogButton)
                {
                    button.ParentActive = false;
                }
            }
        }

        public override void RightClick(UIMouseEvent evt)
        {
            WorldGen.JustQuit();
            Main.menuMode = 10001;
        }
    }
}