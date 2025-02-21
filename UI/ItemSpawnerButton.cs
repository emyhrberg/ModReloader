using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.UI.Spawners;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ItemSpawnerButton(Asset<Texture2D> _image, string hoverText) : BaseButton(_image, hoverText)
    {
        public override void LeftClick(UIMouseEvent evt)
        {
            // force open inventory
            Main.playerInventory = true;

            // Toggle the ItemsPanel.
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            ItemSpawnerPanel itemSpawnerPanel = sys?.mainState?.itemSpawnerPanel;
            if (itemSpawnerPanel != null)
            {
                // toggle Active flag if its true to false vice versa
                itemSpawnerPanel.SetActive(!itemSpawnerPanel.GetActive());

                // focus on the text box
                itemSpawnerPanel.GetCustomTextBox().Focus();
            }
        }
    }
}
