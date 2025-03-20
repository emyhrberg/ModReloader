using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class NPCButton(Asset<Texture2D> image, string buttonText, string hoverText) : BaseButton(image, buttonText, hoverText)
    {
        // Set custom animation dimensions
        protected override int FrameCount => 3;
        protected override int FrameSpeed => 8;
        protected override int FrameWidth => 38;
        protected override int FrameHeight => 48;
        protected override float SpriteScale => 0.9f;

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