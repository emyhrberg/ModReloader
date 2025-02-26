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
            // Preload all assets
            Assets.PreloadAllAssets();
        }
    }

    /// <summary>
    /// A static helper class for loading and preloading mod assets.
    /// </summary>
    public static class Assets
    {
        // Button textures
        public static Asset<Texture2D> Button;
        public static Asset<Texture2D> ButtonOnOff;
        public static Asset<Texture2D> ButtonConfig;
        public static Asset<Texture2D> ButtonItems;
        public static Asset<Texture2D> ButtonNPC;
        public static Asset<Texture2D> ButtonNPC_XMAS;
        public static Asset<Texture2D> ButtonPlayer;
        public static Asset<Texture2D> ButtonDebug;
        public static Asset<Texture2D> ButtonWorld;
        public static Asset<Texture2D> ButtonReloadSP;
        public static Asset<Texture2D> ButtonReloadMP;

        // Filter buttons
        public static Asset<Texture2D> FilterBG;
        public static Asset<Texture2D> FilterBGActive;
        public static Asset<Texture2D> FilterAll;
        public static Asset<Texture2D> FilterMelee;
        public static Asset<Texture2D> FilterRanged;
        public static Asset<Texture2D> FilterMagic;
        public static Asset<Texture2D> FilterSummon;
        public static Asset<Texture2D> FilterArmor;
        public static Asset<Texture2D> FilterAccessories;
        public static Asset<Texture2D> FilterPotions;
        public static Asset<Texture2D> FilterPlaceables;

        // Filter NPC buttons
        public static Asset<Texture2D> FilterTown;
        public static Asset<Texture2D> FilterMob;

        // Sort buttons
        public static Asset<Texture2D> SortID;
        public static Asset<Texture2D> SortValue;
        public static Asset<Texture2D> SortRarity;
        public static Asset<Texture2D> SortName;

        // Checkboxes
        public static Asset<Texture2D> CheckBox;
        public static Asset<Texture2D> CheckMark;

        public static void PreloadAllAssets()
        {
            // Start timer
            Stopwatch s = Stopwatch.StartNew();

            // ALL ASSETS
            Button = PreloadAsset("Button");
            ButtonOnOff = PreloadAsset("ButtonOnOff");
            ButtonConfig = PreloadAsset("ButtonConfig");
            ButtonItems = PreloadAsset("ButtonItems");
            ButtonNPC = PreloadAsset("ButtonNPC");
            ButtonNPC_XMAS = PreloadAsset("ButtonNPC_XMAS");
            ButtonReloadSP = PreloadAsset("ButtonReloadSP");
            ButtonReloadMP = PreloadAsset("ButtonReloadMP");
            ButtonPlayer = PreloadAsset("ButtonPlayer");
            ButtonDebug = PreloadAsset("ButtonDebug");
            ButtonWorld = PreloadAsset("ButtonWorld");

            FilterBG = PreloadAsset("FilterBG");
            FilterBGActive = PreloadAsset("FilterBGActive");
            FilterAll = PreloadAsset("FilterAll");
            FilterMelee = PreloadAsset("FilterMelee");
            FilterRanged = PreloadAsset("FilterRanged");
            FilterMagic = PreloadAsset("FilterMagic");
            FilterSummon = PreloadAsset("FilterSummon");
            FilterArmor = PreloadAsset("FilterArmor");
            FilterAccessories = PreloadAsset("FilterAccessories");
            FilterPotions = PreloadAsset("FilterPotion");
            FilterPlaceables = PreloadAsset("FilterPlaceables");

            FilterTown = PreloadAsset("FilterTown");
            FilterMob = PreloadAsset("FilterMob");

            SortID = PreloadAsset("SortID");
            SortValue = PreloadAsset("SortValue");
            SortRarity = PreloadAsset("SortRarity");
            SortName = PreloadAsset("SortName");

            CheckBox = PreloadAsset("CheckBox");
            CheckMark = PreloadAsset("CheckMark");


            // Stop timer
            s.Stop();
            Log.Info($"Time to Preload all assets in {s.ElapsedMilliseconds}ms.");
        }


        /// <summary>
        /// Preloads an asset with ImmediateLoad mode in the "SquidTestingMod/Assets" directory.
        /// </summary>
        private static Asset<Texture2D> PreloadAsset(string path)
        {
            return ModContent.Request<Texture2D>("SquidTestingMod/Assets/" + path, AssetRequestMode.AsyncLoad);
        }
    }
}
