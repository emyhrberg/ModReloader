using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
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
        private float _scale = 0.9f;
        protected override float Scale => _scale;

        // OLD BUTTON, DO NOT DELETE
        // protected override int FrameWidth => 55;
        // protected override int FrameHeight => 70;
        // protected override int FrameCount => 10;
        // protected override int FrameSpeed => 4;


        protected override int FrameWidth => 60;
        protected override int FrameHeight => 58;

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (!sys.mainState.isClick && Conf.C.DragButtons == "Left")
            {
                return;
            }

            List<DraggablePanel> allPanels = sys?.mainState?.AllPanels;

            // replace with THIS panel
            var panel = sys?.mainState?.modsPanel;

            // Disable all other panels
            foreach (var p in allPanels.Except([panel]))
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
                panel.searchbox.Focus();
            }

            // Disable World, Log, UI, Mods buttons
            foreach (var button in sys.mainState.AllButtons)
            {
                if (button is UIElementButton || button is OptionsButton || button is ModSourcesButton)
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

            if (Conf.C.SaveWorldBeforeReloading)
            {
                WorldGen.SaveAndQuit();
            }
            else
            {
                WorldGen.JustQuit();
            }
            Main.menuMode = 10001;
        }
    }
}