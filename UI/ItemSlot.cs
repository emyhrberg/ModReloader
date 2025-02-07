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
            if (!displayItem.IsAir)
            {
                CalculatedStyle dimensions = GetInnerDimensions();

                // Load the item’s texture.
                Main.instance.LoadItem(displayItem.type);
                Asset<Texture2D> textureAsset = TextureAssets.Item[displayItem.type];
                Texture2D itemTexture = textureAsset.Value;
                // Determine desired drawing size.
                int desiredSize = 48; // target size in pixels

                // Compute a uniform scale factor so that the larger of width or height becomes desiredSize.
                float scale = desiredSize / (float)Math.Max(itemTexture.Width, itemTexture.Height);

                // Optionally, you might want to adjust the scale further based on any custom logic.
                // For now, we simply use the computed scale.
                // Center the scaled texture within the slot.
                Vector2 drawPos = dimensions.Position() + new Vector2((dimensions.Width - itemTexture.Width * scale) / 2f,
                                                                      (dimensions.Height - itemTexture.Height * scale) / 2f);

                spriteBatch.Draw(itemTexture, drawPos, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

            // draw the hovering tooltip for each item slot
            if (IsMouseHovering)
            {
                Main.HoverItem = displayItem.Clone();
                Main.hoverItemName = Main.HoverItem.Name;

                // add red outline hover, meaning draw debug rectangle
                CalculatedStyle dimensions = GetInnerDimensions();
                Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, dimensions.ToRectangle(), Color.Red * 0.2f);
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
