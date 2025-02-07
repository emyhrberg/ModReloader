using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

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


        // No text buttons
        public static Asset<Texture2D> ButtonOnNoText;
        public static Asset<Texture2D> ButtonOffNoText;
        public static Asset<Texture2D> ButtonConfigNoText;
        public static Asset<Texture2D> ButtonItemsNoText;
        public static Asset<Texture2D> ButtonReloadNoText;

        public static void PreloadAllAssets()
        {
            // TEXT BUTTONS
            ButtonOn = PreloadAsset("ButtonOn");
            ButtonOff = PreloadAsset("ButtonOff");
            ButtonConfig = PreloadAsset("ButtonConfig");
            ButtonItems = PreloadAsset("ButtonItems");
            ButtonReload = PreloadAsset("ButtonReload");

            // NO TEXT BUTTONS
            ButtonOnNoText = PreloadAsset("ButtonOnNoText");
            ButtonOffNoText = PreloadAsset("ButtonOffNoText");
            ButtonConfigNoText = PreloadAsset("ButtonConfigNoText");
            ButtonItemsNoText = PreloadAsset("ButtonItemsNoText");
            ButtonReloadNoText = PreloadAsset("ButtonReloadNoText");
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
