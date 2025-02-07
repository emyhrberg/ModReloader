using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    // Instead of inheriting the vanilla behavior we override the drawing and click behavior.
    public class ItemSlot : UIItemSlot
    {
        // We store the item that should be shown in the browser.
        private Item[] itemArray;
        private int itemIndex;
        private int itemSlotContext;
        // IMPORTANT: Store a “master copy” for drawing so that we don’t accidentally change the browser item.
        private Item displayItem;

        public ItemSlot(Item[] itemArray, int itemIndex, int itemSlotContext) : base(itemArray, itemIndex, itemSlotContext)
        {
            this.itemArray = itemArray;
            this.itemIndex = itemIndex;
            this.itemSlotContext = itemSlotContext;
            // Clone it once so that any temporary changes for drawing won’t alter the “source” item.
            this.displayItem = itemArray[itemIndex].Clone();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Get the dimensions for this UI element.
            CalculatedStyle dimensions = GetInnerDimensions();

            // Draw the slot background.
            Texture2D backgroundTexture = TextureAssets.InventoryBack.Value;
            spriteBatch.Draw(backgroundTexture, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            // Draw the item icon using the vanilla helper.
            if (!displayItem.IsAir)
            {
                Terraria.UI.ItemSlot.Draw(spriteBatch, ref displayItem, itemSlotContext, dimensions.Position(), Color.White);
            }

            // Draw a colored overlay.
            // spriteBatch.Draw(TextureAssets.ColorBar.Value, dimensions.ToRectangle(), Color.Red * 0.2f);
        }

        // When the user left-clicks, we want to give them a full-stack copy without removing the item from our browser.
        public override void LeftClick(UIMouseEvent evt)
        {
            // Do not call base.LeftClick(evt) here – that would trigger the default inventory behavior.
            if (Main.mouseItem.IsAir)
            {
                // Clone our display item and give the clone the max stack.
                Main.mouseItem = displayItem.Clone();
                Main.mouseItem.stack = displayItem.maxStack;
            }
        }

        public override void Update(GameTime gameTime)
        {
            // do nothing
        }

        // (Optional) Override any additional drag or right-click handlers if you want to completely disable taking items.
    }
}
