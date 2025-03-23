using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Helpers
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
        public static Asset<Texture2D> ButtonUI;
        public static Asset<Texture2D> ButtonWorld;
        public static Asset<Texture2D> ButtonWorld2;
        public static Asset<Texture2D> ButtonReloadSP;
        public static Asset<Texture2D> ButtonReloadMP;
        public static Asset<Texture2D> ButtonMods;
        public static Asset<Texture2D> ButtonSecond;

        // Filter buttons
        public static Asset<Texture2D> FilterBG;
        public static Asset<Texture2D> FilterBGActive;
        public static Asset<Texture2D> FilterBGActiveModSort;
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
        public static Asset<Texture2D> SortID;
        public static Asset<Texture2D> SortValue;
        public static Asset<Texture2D> SortRarity;
        public static Asset<Texture2D> SortName;
        public static Asset<Texture2D> SortDamage;
        public static Asset<Texture2D> SortDefense;
        public static Asset<Texture2D> Resize;

        // Misc
        public static Asset<Texture2D> Arrow;
        public static Asset<Texture2D> ModOpenFolder; // 22x22
        public static Asset<Texture2D> ModOpenProject; // 22x22
        public static Asset<Texture2D> ModReload; // 22x22
        public static Asset<Texture2D> ModCheck; // 22x22
        public static Asset<Texture2D> ModUncheck; // 22x22

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
            string modName = "SquidTestingMod"; // Use this, in case above line doesnt work
            return ModContent.Request<Texture2D>($"{modName}/Assets/" + path, AssetRequestMode.AsyncLoad);
        }
    }
}
