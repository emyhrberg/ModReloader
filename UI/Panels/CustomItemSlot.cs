using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    // Instead of inheriting the vanilla behavior we override the drawing and click behavior.
    public class CustomItemSlot : UIItemSlot
    {
        // We store the item that should be shown in the panel.
        private Item displayItem;
        private int _itemSlotContext;

        public CustomItemSlot(Item[] itemArray, int itemIndex, int itemSlotContext) : base(itemArray, itemIndex, itemSlotContext)
        {
            // set size
            Width.Set(44, 0f);
            Height.Set(44, 0f);

            displayItem = itemArray[itemIndex].Clone();
            _itemSlotContext = itemSlotContext;
        }

        public Item GetDisplayItem()
        {
            return displayItem;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw inventory background
            CalculatedStyle dimensions = GetInnerDimensions();
            float bgOpacity = IsMouseHovering ? 1.0f : 0.4f; // 0.9 when hovering, 0.4 when not
            Texture2D inventoryBack = TextureAssets.InventoryBack9.Value;
            spriteBatch.Draw(inventoryBack, dimensions.ToRectangle(), Color.White * bgOpacity);

            // Draw the item
            ItemSlot.DrawItemIcon(
                item: displayItem,
                context: _itemSlotContext,
                spriteBatch: spriteBatch,
                screenPositionForItemCenter: GetDimensions().Center(),
                scale: 1f,
                sizeLimit: 24f,
                environmentColor: Color.White
                );

            // draw the hovering tooltip for each item slot
            if (IsMouseHovering)
            {
                Main.HoverItem = displayItem.Clone();
                Main.hoverItemName = Main.HoverItem.Name;
            }
        }

        // When the user left-clicks, we want to give them a full-stack copy without removing the item from our panel.
        public override void LeftClick(UIMouseEvent evt)
        {
            // if dragging, do not perform any action
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys.mainState.itemSpawnerPanel.IsDragging || sys.mainState.itemSpawnerPanel.GetActive() == false)
            {
                Log.Info("Dont spawn item, panel is hidden");
                return;
            }

            // force player inventory to open
            Main.playerInventory = true;

            // Clone our display item and give the clone the max stack.
            Main.mouseItem = displayItem.Clone();
            Main.mouseItem.stack = displayItem.maxStack;
        }

        // public override void RightMouseDown(UIMouseEvent evt)
        // {
        //     // if dragging, do not perform any action
        //     MainSystem sys = ModContent.GetInstance<MainSystem>();
        //     if (sys.mainState.itemSpawnerPanel.IsDragging || sys.mainState.itemSpawnerPanel.GetActive() == false)
        //     {
        //         Log.Info("Dont spawn item, panel is hidden");
        //         return;
        //     }

        //     // force player inventory to open
        //     Main.playerInventory = true;

        //     // Clone our display item and give the clone a stack of 1.
        //     Main.mouseItem = displayItem.Clone();
        //     Main.mouseItem.stack = 1;
        // }

        private int stackDelay = 30;

        public override void Update(GameTime gameTime)
        {
            // Only do this if the mouse is over this slot, right button is down,
            // the mouseItem is the same type, and the stack isn't at max.
            if (IsMouseHovering && Main.mouseRight &&
                Main.mouseItem.type == displayItem.type &&
                Main.mouseItem.stack < Main.mouseItem.maxStack)
            {
                if (stackDelay > 0)
                    stackDelay--;
                else if (Main.mouseItem.stack < Main.mouseItem.maxStack)
                    Main.mouseItem.stack++;
            }
        }
    }
}
