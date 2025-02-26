
using Terraria.ID;
using Terraria.ModLoader;


public class MyEditTravelMerchShopClass : GlobalNPC
{
    public override void SetupTravelShop(int[] shop, ref int nextSlot)
    {
        shop[nextSlot] = ItemID.PlatinumCoin; // change to whatever item you want to add
        nextSlot++;
    }
}

