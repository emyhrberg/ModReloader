using Microsoft.Xna.Framework.Graphics;
using ModHelper.UI.Elements;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class NPCButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set custom animation dimensions
        private float _scale = 0.9f;
        protected override float Scale => _scale;
        protected override int FrameCount => 3;
        protected override int FrameSpeed => 8;
        protected override int FrameWidth => 38;
        protected override int FrameHeight => 48;

        public override void LeftClick(UIMouseEvent evt)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            // Toggle the NPCPanel.
            NPCSpawner panel = sys?.mainState?.npcSpawnerPanel;
            if (panel != null)
            {
                // Close the Item spawner if open
                var itemPanel = sys.mainState.itemSpawnerPanel;
                if (itemPanel != null && itemPanel.GetActive())
                {
                    itemPanel.SetActive(false);
                    var itemButton = sys.mainState.AllButtons.Find(x => x is ItemButton);
                    if (itemButton != null)
                        itemButton.ParentActive = false;
                }

                // Use the helper to ensure other left-side panels are closed
                sys.mainState.TogglePanel(panel);
                ParentActive = panel.GetActive();

                // Focus on textbox if panel is active
                if (panel.GetActive())
                {
                    panel.GetCustomTextBox()?.Focus();
                }
            }
        }
    }
}