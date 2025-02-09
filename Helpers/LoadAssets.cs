using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.UI;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;

namespace SquidTestingMod.Helpers
{
    /// <summary>
    /// A ModSystem that preloads assets early in the mod loading process.
    /// </summary>
    public class LoadAssets : ModSystem
    {
        public override void Load()
        {
            // Preload assets early (before PostSetupContent, etc.)
            Assets.PreloadAllAssets();
        }

        public override void PostSetupContent()
        {
            Assets.PreloadAllItems();
        }
    }

    /// <summary>
    /// A static helper class for loading and preloading mod assets.
    /// </summary>
    public static class Assets
    {
        // Preloaded assets
        public static Asset<Texture2D> ButtonOn;
        public static Asset<Texture2D> ButtonOff;
        public static Asset<Texture2D> ButtonConfig;
        public static Asset<Texture2D> ButtonItems;
        public static Asset<Texture2D> ButtonReload;
        public static Asset<Texture2D> ButtonNPC;
        public static Asset<Texture2D> ButtonGod;
        public static Asset<Texture2D> ButtonGodOff;
        public static Asset<Texture2D> ButtonTeleport;
        public static Asset<Texture2D> ButtonLog;
        public static Asset<Texture2D> ButtonSecondClient;
        public static Asset<Texture2D> ButtonHitbox;
        public static Asset<Texture2D> ButtonUIDebug;

        // No text buttons
        public static Asset<Texture2D> ButtonOnNoText;
        public static Asset<Texture2D> ButtonOffNoText;
        public static Asset<Texture2D> ButtonConfigNoText;
        public static Asset<Texture2D> ButtonItemsNoText;
        public static Asset<Texture2D> ButtonReloadNoText;
        public static Asset<Texture2D> ButtonNPCNoText;
        public static Asset<Texture2D> ButtonGodNoText;
        public static Asset<Texture2D> ButtonGodOffNoText;
        public static Asset<Texture2D> ButtonTeleportNoText;
        public static Asset<Texture2D> ButtonLogNoText;
        public static Asset<Texture2D> ButtonSecondClientNoText;
        public static Asset<Texture2D> ButtonHitboxNoText;
        public static Asset<Texture2D> ButtonUIDebugNoText;


        public static void PreloadAllAssets()
        {
            Stopwatch s = Stopwatch.StartNew();

            // TEXT BUTTONS
            ButtonOn = PreloadAsset("ButtonOn");
            ButtonOff = PreloadAsset("ButtonOff");
            ButtonConfig = PreloadAsset("ButtonConfig");
            ButtonItems = PreloadAsset("ButtonItems");
            ButtonReload = PreloadAsset("ButtonReload");
            ButtonNPC = PreloadAsset("ButtonNPC");
            ButtonGod = PreloadAsset("ButtonGod");
            ButtonGodOff = PreloadAsset("ButtonGodOff");
            ButtonTeleport = PreloadAsset("ButtonTeleport");
            ButtonLog = PreloadAsset("ButtonLog");
            ButtonSecondClient = PreloadAsset("ButtonSecond");
            ButtonHitbox = PreloadAsset("ButtonHitboxes");
            ButtonUIDebug = PreloadAsset("ButtonUI");

            // NO TEXT BUTTONS
            ButtonOnNoText = PreloadAsset("ButtonOnNoText");
            ButtonOffNoText = PreloadAsset("ButtonOffNoText");
            ButtonConfigNoText = PreloadAsset("ButtonConfigNoText");
            ButtonItemsNoText = PreloadAsset("ButtonItemsNoText");
            ButtonReloadNoText = PreloadAsset("ButtonReloadNoText");
            ButtonNPCNoText = PreloadAsset("ButtonNPCNoText");
            ButtonGodNoText = PreloadAsset("ButtonGodNoText");
            ButtonGodOffNoText = PreloadAsset("ButtonGodOffNoText");
            ButtonTeleportNoText = PreloadAsset("ButtonTeleportNoText");
            ButtonLogNoText = PreloadAsset("ButtonLogNoText");
            ButtonSecondClientNoText = PreloadAsset("ButtonSecondNoText");
            ButtonHitboxNoText = PreloadAsset("ButtonHitboxesNoText");
            ButtonUIDebugNoText = PreloadAsset("ButtonUINoText");

            s.Stop();
            Log.Info($"Time to Preload all assets in {s.ElapsedMilliseconds}ms.");
        }

        public static HashSet<Item> PreloadedItems = [];
        public static HashSet<ItemSlot> PreloadedItemSlots = [];
        public static UIGrid grid;

        public static void PreloadAllItems()
        {
            int allItems = TextureAssets.Item.Length - 1;

            Stopwatch s = Stopwatch.StartNew();

            for (int i = 1; i <= allItems; i++)
            {
                Item item = new();
                item.SetDefaults(i);
                PreloadedItems.Add(item);

                ItemSlot itemSlot = new([item], 0, Terraria.UI.ItemSlot.Context.ShopItem);
                itemSlot.Width.Set(50, 0f);
                itemSlot.Height.Set(50, 0f);
                PreloadedItemSlots.Add(itemSlot);
            }

            // Add all preloaded slots at once
            if (grid == null)
            {
                Log.Info("Creating new grid");
                grid = new UIGrid();
                grid.AddRange(PreloadedItemSlots);
            }

            s.Stop();
            Log.Info($"Preloaded {PreloadedItemSlots.Count} item slots in {s.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Preloads an asset with ImmediateLoad mode in the "SquidTestingMod/Assets" directory.
        /// </summary>
        private static Asset<Texture2D> PreloadAsset(string path)
        {
            return ModContent.Request<Texture2D>("SquidTestingMod/Assets/" + path, AssetRequestMode.ImmediateLoad);
        }
    }
}
