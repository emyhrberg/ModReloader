using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
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
            float bgOpacity = IsMouseHovering ? 0.9f : 0.8f;

            Config c = ModContent.GetInstance<Config>();
            Texture2D t = c.ItemSlotStyle switch
            {
                1 => TextureAssets.InventoryBack.Value,
                2 => TextureAssets.InventoryBack2.Value,
                3 => TextureAssets.InventoryBack3.Value,
                4 => TextureAssets.InventoryBack4.Value,
                5 => TextureAssets.InventoryBack5.Value,
                6 => TextureAssets.InventoryBack6.Value,
                7 => TextureAssets.InventoryBack7.Value,
                8 => TextureAssets.InventoryBack8.Value,
                9 => TextureAssets.InventoryBack9.Value,
                10 => TextureAssets.InventoryBack10.Value,
                11 => TextureAssets.InventoryBack11.Value,
                12 => TextureAssets.InventoryBack12.Value,
                13 => TextureAssets.InventoryBack13.Value,
                14 => TextureAssets.InventoryBack14.Value,
                15 => TextureAssets.InventoryBack15.Value,
                16 => TextureAssets.InventoryBack16.Value,
                17 => TextureAssets.InventoryBack17.Value,
                18 => TextureAssets.InventoryBack18.Value,
                19 => TextureAssets.InventoryBack19.Value,
                _ => TextureAssets.InventoryBack.Value,
            };

            // Draw background
            spriteBatch.Draw(t, dimensions.ToRectangle(), c.ItemSlotColor * bgOpacity);

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
            // Rectangle r = dimensions.ToRectangle();
            // int thickness = 1;
            // spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(r.X, r.Y, r.Width, thickness), Color.Black); // Top border
            // spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(r.X, r.Bottom - thickness, r.Width, thickness), Color.Black); // Bottom border
            // spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(r.X, r.Y, thickness, r.Height), Color.Black); // Left border
            // spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(r.Right - thickness, r.Y, thickness, r.Height), Color.Black); // Right border

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
            // check if dragging
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys.mainState.itemButton.itemsPanel.IsDragging)
            {
                Log.Info("Dragging");
                return;
            }

            if (Main.mouseItem.IsAir)
            {
                // Clone our display item and give the clone the max stack.
                Main.mouseItem = displayItem.Clone();
                Main.mouseItem.stack = displayItem.maxStack;
            }
        }

        // When the user right-clicks, we want to give them an incrementing stack of the item.

        public override void RightClick(UIMouseEvent evt)
        {
            if (Main.mouseItem.IsAir)
            {
                // Clone our display item and give the clone a stack of 1.
                Main.mouseItem = displayItem.Clone();
                if (Main.mouseItem.stack < Main.mouseItem.maxStack)
                    Main.mouseItem.stack++;
            }
        }

        public override void RightMouseDown(UIMouseEvent evt)
        {
            base.RightMouseDown(evt);
        }

        public override void Update(GameTime gameTime)
        {
            // base.Update(gameTime);
        }
    }
}
