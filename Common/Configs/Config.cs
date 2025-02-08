using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SquidTestingMod.Common.Configs
{
    public class Config : ModConfig
    {
        // CLIENT SIDE
        public override ConfigScope Mode => ConfigScope.ClientSide;

        // ACTUAL CONFIG
        [Header("Reload")]
        public ReloadConfig Reload = new();

        [Header("General")]
        public GeneralConfig General = new();

        [Header("Gameplay")]
        public GameplayConfig Gameplay = new();

        [Header("ItemBrowser")]
        public ItemBrowserConfig ItemBrowser = new();

        public class ReloadConfig
        {
            [OptionStrings(["None", "Singleplayer", "Multiplayer"])]
            [DefaultValue("None")]
            [DrawTicks]
            public string AutoloadWorld = "None";

            [DefaultValue(false)]
            public bool SaveWorld;

            [DefaultValue(true)]
            public bool InvokeBuildAndReload;

            [DefaultValue("SquidTestingMod")]
            public string ModToReload;

            [DefaultValue(100)]
            [Range(100, 5000)]
            public int WaitingTimeBeforeNavigatingToModSources;

            [DefaultValue(100)]
            [Range(100, 5000)]
            public int WaitingTimeBeforeBuildAndReload;
        }

        public class GeneralConfig
        {
            [DefaultValue(false)]
            public bool OnlyShowWhenInventoryOpen;

            [DefaultValue(true)]
            public bool ShowButtonText;

            [DefaultValue(true)]
            public bool ShowTooltips;

            [Range(0.3f, 1f)]
            [Increment(0.1f)]
            [DrawTicks]
            [DefaultValue(0.7f)]
            public float ButtonSize;
        }

        public class GameplayConfig
        {
            [DefaultValue(true)]
            public bool AlwaysSpawnBossOnTopOfPlayer;

            [DefaultValue(true)]
            public bool StartInGodMode;

            [OptionStrings(["None", "Small", "Big"])]
            [DefaultValue("Small")]
            [DrawTicks]
            public string GodModeOutlineSize = "Small";
        }

        public class ItemBrowserConfig
        {
            [DefaultValue(100)]
            [Range(0, 10000)]
            public int MaxItemsToDisplay = 1000;

            [DefaultValue(1)]
            [Range(1, 19)]
            public int ItemSlotStyle = 1;

            [DefaultValue(typeof(Color), "255, 0, 0, 255"), ColorHSLSlider(false), ColorNoAlpha]
            public Color ItemSlotColor = new(255, 0, 0, 255);
        }

        public override void OnChanged()
        {
            ChangeGodModeOutline();
            ChangeButtonTextVisibility();
            ChangeButtonSizes();
        }

        private static void ChangeButtonSizes()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            sys?.mainState?.UpdateAllButtonsTexture();
        }

        private static void ChangeButtonTextVisibility()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            sys?.mainState?.UpdateAllButtonsTexture();
        }

        private void ChangeGodModeOutline()
        {
            // add null check for the class itself in case it's called before the mod is loaded
            // idk
            if (ModContent.GetInstance<Config>() == null)
                return;

            int type = ModContent.ItemType<BorderShaderDye>();

            if (Gameplay.GodModeOutlineSize == "Small")
            {
                Asset<Effect> smallOutlineEffect = Mod.Assets.Request<Effect>("Effects/LessOutlineEffect");
                GameShaders.Armor.BindShader(type, new ArmorShaderData(smallOutlineEffect, "Pass0"));
            }
            else if (Gameplay.GodModeOutlineSize == "Big")
            {
                Asset<Effect> bigOutlineEffect = Mod.Assets.Request<Effect>("Effects/OutlineEffect");
                GameShaders.Armor.BindShader(type, new ArmorShaderData(bigOutlineEffect, "Pass0"));
            }
        }
    }
}
