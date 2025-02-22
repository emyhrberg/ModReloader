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
        public static Asset<Texture2D> ButtonNPCSS;
        public static Asset<Texture2D> ButtonNPCSS_XMAS;
        public static Asset<Texture2D> ButtonPlayer;
        public static Asset<Texture2D> ButtonPlayerSS;
        public static Asset<Texture2D> ButtonDebug;
        public static Asset<Texture2D> ButtonDebugWrenchSS;
        public static Asset<Texture2D> ButtonWorld;
        public static Asset<Texture2D> ButtonWorldSS;
        public static Asset<Texture2D> ButtonReload;
        public static Asset<Texture2D> ButtonReloadSPSS;
        public static Asset<Texture2D> ButtonReloadMPSS;

        // Filter buttons
        public static Asset<Texture2D> FilterAll;
        public static Asset<Texture2D> FilterMelee;
        public static Asset<Texture2D> FilterRanged;
        public static Asset<Texture2D> FilterMagic;
        public static Asset<Texture2D> FilterSummon;

        // Checkboxes
        public static Asset<Texture2D> CheckBox;
        public static Asset<Texture2D> CheckMark;

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
            ButtonNPCSS = PreloadAsset("ButtonNPCSS");
            ButtonNPCSS_XMAS = PreloadAsset("ButtonNPCSS_XMAS");
            ButtonReload = PreloadAsset("ButtonReload");
            ButtonReloadSPSS = PreloadAsset("ButtonReloadSPSS");
            ButtonReloadMPSS = PreloadAsset("ButtonReloadMPSS");
            ButtonPlayer = PreloadAsset("ButtonPlayer");
            ButtonPlayerSS = PreloadAsset("ButtonPlayerSS");
            ButtonDebug = PreloadAsset("ButtonDebug");
            ButtonDebugWrenchSS = PreloadAsset("ButtonDebugWrenchSS");
            ButtonWorld = PreloadAsset("ButtonWorld");
            ButtonWorldSS = PreloadAsset("ButtonWorldSS");
            FilterAll = PreloadAsset("FilterAll");
            FilterMelee = PreloadAsset("FilterMelee");
            FilterRanged = PreloadAsset("FilterRanged");
            FilterMagic = PreloadAsset("FilterMagic");
            FilterSummon = PreloadAsset("FilterSummon");
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
