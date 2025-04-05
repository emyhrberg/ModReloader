using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Common.Systems;
using ModHelper.UI.Elements;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class UIElementButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
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
            base.LeftClick(evt);

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (!sys.mainState.isClick && Conf.C.DragButtons == "Left")
            {
                return;
            }

            List<DraggablePanel> rightSidePanels = sys?.mainState?.AllPanels;

            // replace with THIS panel
            var panel = sys?.mainState?.uiPanel;

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
                if (button is OptionsButton || button is ModsButton)
                {
                    button.ParentActive = false;
                }
            }
        }

        public override void RightClick(UIMouseEvent evt)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (!sys.mainState.isClick)
            {
                return;
            }

            base.RightClick(evt);

            // Toggle all elements
            UIElementSystem elementSystem = ModContent.GetInstance<UIElementSystem>();
            UIElementState elementState = elementSystem.debugState;
            elementState.ToggleShowAll();
        }
    }
}
