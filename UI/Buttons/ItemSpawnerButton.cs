using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.UI.Panels;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class ItemSpawnerButton : BaseButton
    {

        public ItemSpawnerButton(Asset<Texture2D> image, string hoverText, bool animating)
            : base(image, hoverText, animating)
        {
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // force open inventory
            Main.playerInventory = true;

            // Close the NPCSpawnerPanel.
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            var npcSpawnerPanel = sys?.mainState?.npcSpawnerPanel;
            if (npcSpawnerPanel != null && npcSpawnerPanel.GetActive())
            {
                npcSpawnerPanel.SetActive(false);
            }

            // Toggle the ItemsPanel.
            ItemSpawnerPanel itemSpawnerPanel = sys?.mainState?.itemSpawnerPanel;
            if (itemSpawnerPanel != null)
            {
                if (itemSpawnerPanel.GetActive())
                {
                    itemSpawnerPanel.SetActive(false);
                }
                else
                {
                    itemSpawnerPanel.SetActive(true);

                    // focus on the search bar
                    itemSpawnerPanel.GetCustomTextBox()?.Focus();
                }
            }
        }
    }
}
