using System.Collections.Generic;
using System.Diagnostics;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;

public static class ItemCache
{
    public static List<Item> AllItems { get; private set; } = new List<Item>();

    public static void Initialize()
    {
        Stopwatch s = Stopwatch.StartNew();

        if (AllItems.Count > 0)
            return;

        int totalItems = TextureAssets.Item.Length - 1;
        for (int i = 1; i <= totalItems; i++)
        {
            Item item = new();
            item.SetDefaults(i);
            AllItems.Add(item);
        }

        s.Stop();
        Log.Info($"ItemCache initialized in {s.ElapsedMilliseconds}ms");
    }
}
