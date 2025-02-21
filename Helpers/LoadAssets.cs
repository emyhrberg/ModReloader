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
        public static Asset<Texture2D> ButtonOn;
        public static Asset<Texture2D> ButtonOff;
        public static Asset<Texture2D> ButtonConfig;
        public static Asset<Texture2D> ButtonItems;
        public static Asset<Texture2D> ButtonNPC;
        public static Asset<Texture2D> ButtonGodOn;
        public static Asset<Texture2D> ButtonGodOff;
        public static Asset<Texture2D> ButtonFastOn;
        public static Asset<Texture2D> ButtonFastOff;
        public static Asset<Texture2D> ButtonLog;
        public static Asset<Texture2D> ButtonSecondClient;
        public static Asset<Texture2D> ButtonHitboxOn;
        public static Asset<Texture2D> ButtonHitboxOff;
        public static Asset<Texture2D> ButtonUIDebug;
        public static Asset<Texture2D> ButtonReloadSP;
        public static Asset<Texture2D> ButtonReloadMP;

        // Filter buttons
        public static Asset<Texture2D> FilterAll;
        public static Asset<Texture2D> FilterMelee;
        public static Asset<Texture2D> FilterRanged;
        public static Asset<Texture2D> FilterMagic;
        public static Asset<Texture2D> FilterSummon;

        public static void PreloadAllAssets()
        {
            // Start timer
            Stopwatch s = Stopwatch.StartNew();

            // ALL ASSETS
            ButtonOn = PreloadAsset("ButtonOn");
            ButtonOff = PreloadAsset("ButtonOff");
            ButtonConfig = PreloadAsset("ButtonConfig");
            ButtonItems = PreloadAsset("ButtonItems");
            ButtonNPC = PreloadAsset("ButtonNPC");
            ButtonGodOn = PreloadAsset("ButtonGodOn");
            ButtonGodOff = PreloadAsset("ButtonGodOff");
            ButtonFastOn = PreloadAsset("ButtonFastOn");
            ButtonFastOff = PreloadAsset("ButtonFastOff");
            ButtonLog = PreloadAsset("ButtonLog");
            ButtonSecondClient = PreloadAsset("ButtonSecond");
            ButtonHitboxOn = PreloadAsset("ButtonHitboxOn");
            ButtonHitboxOff = PreloadAsset("ButtonHitboxOff");
            ButtonUIDebug = PreloadAsset("ButtonUI");
            ButtonReloadSP = PreloadAsset("ButtonReloadSP");
            ButtonReloadMP = PreloadAsset("ButtonReloadMP");
            FilterAll = PreloadAsset("FilterAll");
            FilterMelee = PreloadAsset("FilterMelee");
            FilterRanged = PreloadAsset("FilterRanged");
            FilterMagic = PreloadAsset("FilterMagic");
            FilterSummon = PreloadAsset("FilterSummon");

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
