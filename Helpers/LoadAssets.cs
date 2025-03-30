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
        // Buttons
        public static Asset<Texture2D> Button;
        public static Asset<Texture2D> ButtonNoOutline;
        public static Asset<Texture2D> ButtonReloadSP;
        public static Asset<Texture2D> ButtonReloadMP;
        public static Asset<Texture2D> ButtonConfig;
        public static Asset<Texture2D> CollapseUp;

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
