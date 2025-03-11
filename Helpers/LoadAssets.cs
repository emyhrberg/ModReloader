using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.UI;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;

namespace SquidTestingMod.Helpers
{
    public class LoadAssets : ModSystem
    {
        public override void Load()
        {
            var ignored = Assets.Initialized;
        }
    }

    public static class Assets
    {
        // Buttons
        public static Asset<Texture2D> CollapseDown;
        public static Asset<Texture2D> CollapseUp;
        public static Asset<Texture2D> CollapseLeft;
        public static Asset<Texture2D> CollapseRight;
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
        public static Asset<Texture2D> FilterVanity;
        public static Asset<Texture2D> FilterAccessories;
        public static Asset<Texture2D> FilterPotion;
        public static Asset<Texture2D> FilterPlaceables;
        public static Asset<Texture2D> FilterTown;
        public static Asset<Texture2D> FilterMob;

        // Misc
        public static Asset<Texture2D> SortID;
        public static Asset<Texture2D> SortValue;
        public static Asset<Texture2D> SortRarity;
        public static Asset<Texture2D> SortName;
        public static Asset<Texture2D> SortDamage;
        public static Asset<Texture2D> SortDefense;
        public static Asset<Texture2D> Resize;
        public static Asset<Texture2D> Arrow;

        // Bool for checking if assets are loaded
        public static bool Initialized { get; set; }

        // Constructor
        static Assets()
        {
            foreach (FieldInfo field in typeof(Assets).GetFields())
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
            string modName = "SquidTestingMod"; // Use this, in case above line doesnt work
            return ModContent.Request<Texture2D>($"{modName}/Assets/" + path, AssetRequestMode.AsyncLoad);
        }
    }
}
