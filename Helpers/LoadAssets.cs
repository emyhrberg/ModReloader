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
            base.PostSetupContent();
            // Assets.PreloadAllAssets();
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
        public static Asset<Texture2D> ButtonGodOn;
        public static Asset<Texture2D> ButtonGodOff;
        public static Asset<Texture2D> ButtonFastOn;
        public static Asset<Texture2D> ButtonFastOff;
        public static Asset<Texture2D> ButtonLog;
        public static Asset<Texture2D> ButtonSecondClient;
        public static Asset<Texture2D> ButtonHitbox;
        public static Asset<Texture2D> ButtonUIDebug;

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
            ButtonGodOn = PreloadAsset("ButtonGodOn");
            ButtonGodOff = PreloadAsset("ButtonGodOff");
            ButtonFastOn = PreloadAsset("ButtonFastOn");
            ButtonFastOff = PreloadAsset("ButtonFastOff");
            ButtonLog = PreloadAsset("ButtonLog");
            ButtonSecondClient = PreloadAsset("ButtonSecond");
            ButtonHitbox = PreloadAsset("ButtonHitboxes");
            ButtonUIDebug = PreloadAsset("ButtonUI");

            s.Stop();
            Log.Info($"Time to Preload all assets in {s.ElapsedMilliseconds}ms.");
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
