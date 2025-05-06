using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace ModHelper.Helpers
{
    /// <summary>
    /// To add a new asset, simply add a new field like:
    /// public static Asset<Texture2D> MyAsset;
    /// </summary>
    public class LoadAssets : ModSystem
    {
        public override void Load()
        {
            _ = Ass.Initialized;
        }
    }

    public static class Ass
    {
        // Helper buttons
        public static Asset<Texture2D> Button;
        public static Asset<Texture2D> ButtonNoOutline;
        public static Asset<Texture2D> ButtonHighlight;

        // Button sprites
        public static Asset<Texture2D> ButtonReloadSP;
        public static Asset<Texture2D> ButtonReloadMP;
        public static Asset<Texture2D> ButtonMods;
        public static Asset<Texture2D> ButtonModSources;

        // Misc
        public static Asset<Texture2D> Resize;
        public static Asset<Texture2D> ModOpenFolder; // 22x22
        public static Asset<Texture2D> ModOpenProject; // 22x22
        public static Asset<Texture2D> ModReload; // 22x22
        public static Asset<Texture2D> ModCheck; // 22x22
        public static Asset<Texture2D> ModUncheck; // 22x22
        public static Asset<Texture2D> ConfigOpen; // 36x36
        public static Asset<Texture2D> ConfigClose; // 36x36
        public static Asset<Texture2D> ModInfo; // 36x36

        // Collapse
        public static Asset<Texture2D> CollapseUp;
        public static Asset<Texture2D> CollapseDown;

        // Mod filters
        public static Asset<Texture2D> FilterEnabled; // 32x32*3 (3 vertical images spritesheet)
        public static Asset<Texture2D> FilterModSide; // 32x32*3 (3 vertical images spritesheet)
        public static Asset<Texture2D> FilterViewSize; // 32x32*2 (2 vertical images spritesheet)

        // Bool for checking if assets are loaded
        public static bool Initialized { get; set; }

        // Constructor
        static Ass()
        {
            foreach (FieldInfo field in typeof(Ass).GetFields())
            {
                if (field.FieldType == typeof(Asset<Texture2D>))
                {
                    field.SetValue(null, RequestAsset(field.Name));
                }
            }
        }

        private static Asset<Texture2D> RequestAsset(string path)
        {
            // string modName = typeof(Assets).Namespace;
            string modName = "ModHelper"; // Use this, in case above line doesnt work
            return ModContent.Request<Texture2D>($"{modName}/Assets/" + path, AssetRequestMode.AsyncLoad);
        }
    }
}
