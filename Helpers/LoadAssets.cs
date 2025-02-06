using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
        public static Asset<Texture2D> ButtonRefresh;
        public static Asset<Texture2D> ButtonNPCs;
        public static Asset<Texture2D> ButtonItems;
        public static Asset<Texture2D> ButtonConfig;
        public static Asset<Texture2D> PlayerGlow;


        public static void PreloadAllAssets()
        {
            ButtonRefresh = PreloadAsset("ButtonRefresh");
            ButtonNPCs = PreloadAsset("ButtonNPCs");
            ButtonItems = PreloadAsset("ButtonItems");
            ButtonConfig = PreloadAsset("ButtonConfig");
            PlayerGlow = PreloadAsset("PlayerGlow");
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
