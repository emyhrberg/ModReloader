using ErkysModdingUtilities.UI.Elements;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI;

namespace ErkysModdingUtilities.UI.Buttons
{
    public class NPCButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, float textSize) : BaseButton(spritesheet, buttonText, hoverText, textSize)
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
            // Close the ItemSpawnerPanel.
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            var itemSpawnerPanel = sys?.mainState?.itemSpawnerPanel;
            if (itemSpawnerPanel != null && itemSpawnerPanel.GetActive())
            {
                itemSpawnerPanel.SetActive(false);
            }

            // Toggle the NPCSpawnerPanel.
            NPCSpawner npcSpawnerPanel = sys?.mainState?.npcSpawnerPanel;

            if (npcSpawnerPanel != null)
            {
                if (npcSpawnerPanel.GetActive())
                {
                    npcSpawnerPanel.SetActive(false);
                }
                else
                {
                    npcSpawnerPanel.SetActive(true);

                    // focus on textbox
                    npcSpawnerPanel.GetCustomTextBox()?.Focus();
                }
            }
        }
    }
}