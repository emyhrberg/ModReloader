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
        // 6 random items
        private Item[] randomItems = new Item[6];

        public ItemSpawnerButton(Asset<Texture2D> _image, string hoverText) : base(_image, hoverText)
        {
            // set the 6 random items
            for (int i = 0; i < randomItems.Length; i++)
            {
                randomItems[i] = new Item();
                randomItems[i].SetDefaults(Main.rand.Next(1, 4000));
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // force open inventory
            Main.playerInventory = true;

            // Close the NPCSpawnerPanel.
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            var npcSpawnerPanel = sys?.mainState?.npcSpawnerPanel;
            if (npcSpawnerPanel != null && npcSpawnerPanel.GetNPCPanelActive())
            {
                npcSpawnerPanel.SetNPCPanelActive(false);
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

        // public override void Draw(SpriteBatch spriteBatch)
        // {
        //     // draw base button
        //     base.Draw(spriteBatch);

        //     // draw 6 random items, 3 per row
        //     for (int i = 0; i < randomItems.Length; i++)
        //     {
        //         var dims = GetDimensions().ToRectangle();
        //         int x = dims.X + (i % 3) * 35 + 5;
        //         int y = dims.Y + (i / 3) * 35 + 5;

        //         Item item = randomItems[i];
        //         item.position = new Vector2(x, y);

        //         item.stack = 1;
        //         item.scale = 0.5f;
        //         item.width = 32;
        //         item.height = 32;

        //         Main.instance.LoadItem(item.type);

        //         TextureAssets.Item[item.type] = TextureAssets.Item[item.type] ?? TextureAssets.Item[1];
        //         ItemSlot.Draw(spriteBatch, ref item, 14, new Vector2(x, y));
        //     }
        // }
    }
}
