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
        public ItemSlot(Item[] itemArray, int itemIndex, int itemSlotContext) : base(itemArray, itemIndex, itemSlotContext)
        {

        }

    }
}
