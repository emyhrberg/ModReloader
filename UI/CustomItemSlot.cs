using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ItemSlot : UIItemSlot
    {
        private Item[] itemArray;
        private int itemIndex;
        private int itemSlotContext;
        private Item item;

        public ItemSlot(Item[] itemArray, int itemIndex, int itemSlotContext) : base(itemArray, itemIndex, itemSlotContext)
        {
            this.itemArray = itemArray;
            this.itemIndex = itemIndex;
            this.itemSlotContext = itemSlotContext;
            this.item = itemArray[itemIndex];
        }

        // Draw a red rectangle around the item slot.
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            CalculatedStyle dimensions = GetInnerDimensions();
            spriteBatch.Draw(TextureAssets.ColorBar.Value, dimensions.ToRectangle(), Color.Red * 0.5f);
        }

        // On click, create item clone and put in mouse slot.
        public override void LeftClick(UIMouseEvent evt)
        {
            if (Main.mouseItem.IsAir)
            {
                // clone item and place in mouse slot
                Main.mouseItem = item.Clone();
                Main.mouseItem.stack = item.maxStack;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
