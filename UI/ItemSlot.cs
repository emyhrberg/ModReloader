using System;
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
        private int itemSlotContext;
        private Item displayItem;

        public ItemSlot(Item[] itemArray, int itemIndex, int itemSlotContext) : base(itemArray, itemIndex, itemSlotContext)
        {
            this.itemSlotContext = itemSlotContext;
            displayItem = itemArray[itemIndex].Clone();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetInnerDimensions();
            float bgOpacity = IsMouseHovering ? 1f : 0.4f;

            // Draw background
            spriteBatch.Draw(TextureAssets.InventoryBack14.Value, dimensions.ToRectangle(), Color.Blue * bgOpacity);

            if (!displayItem.IsAir)
            {
                // Load the item’s texture.
                Main.instance.LoadItem(displayItem.type);
                Asset<Texture2D> textureAsset = TextureAssets.Item[displayItem.type];
                Texture2D itemTexture = textureAsset.Value;
                int desiredSize = 25; // target size in pixels

                float scale = desiredSize / (float)Math.Max(itemTexture.Width, itemTexture.Height);
                float x = (dimensions.Width - itemTexture.Width * scale) / 2f;
                float y = (dimensions.Height - itemTexture.Height * scale) / 2f;
                Vector2 drawPos = dimensions.Position() + new Vector2(x, y);

                spriteBatch.Draw(itemTexture, drawPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

            // Draw a black border (1 pixel thick) around the slot.
            Rectangle r = dimensions.ToRectangle();
            int thickness = 1;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(r.X, r.Y, r.Width, thickness), Color.Black); // Top border
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(r.X, r.Bottom - thickness, r.Width, thickness), Color.Black); // Bottom border
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(r.X, r.Y, thickness, r.Height), Color.Black); // Left border
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(r.Right - thickness, r.Y, thickness, r.Height), Color.Black); // Right border

            // draw the hovering tooltip for each item slot
            if (IsMouseHovering)
            {
                Main.HoverItem = displayItem.Clone();
                Main.hoverItemName = Main.HoverItem.Name;
            }
        }

        // When the user left-clicks, we want to give them a full-stack copy without removing the item from our browser.
        public override void LeftClick(UIMouseEvent evt)
        {
            if (Main.mouseItem.IsAir)
            {
                // Clone our display item and give the clone the max stack.
                Main.mouseItem = displayItem.Clone();
                Main.mouseItem.stack = displayItem.maxStack;
            }
        }

        public override void Update(GameTime gameTime)
        {
            // base.Update(gameTime);
        }

    }
}
