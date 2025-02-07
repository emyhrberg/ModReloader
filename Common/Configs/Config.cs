using System;
using System.ComponentModel;
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
        [Header("Autoload")]
        [OptionStrings(["None", "Singleplayer", "Multiplayer"])]
        [DefaultValue("None")]
        [DrawTicks]
        public string AutoloadWorld = "None";

        // RELOAD HEADER
        [Header("Reload")]

        [DefaultValue(false)]
        public bool SaveWorld;

        [DefaultValue(false)]
        public bool InvokeBuildAndReload;

        [DefaultValue("SquidTestingMod")]
        public string ModToReload;

        [DefaultValue(0)]
        [Range(0, 5000)]
        public int WaitingTimeBeforeNavigatingToModSources;

        [DefaultValue(1000)]
        [Range(0, 5000)]
        public int WaitingTimeBeforeBuildAndReload;

        [Header("General")]
        [DefaultValue(true)]
        public bool ShowToggleButton;

        [DefaultValue(true)]
        public bool ShowButtonText;

        [DefaultValue(true)]
        public bool ShowTooltips;

        [Header("UI")]
        [DefaultValue(false)]
        public bool ShowHitboxes;

        [Header("Gameplay")]

        [DefaultValue(false)]
        public bool AlwaysSpawnBossOnTopOfPlayer;

        [Header("GodMode")]
        [DefaultValue(false)]
        public bool StartInGodMode;

        [OptionStrings(["None", "Small", "Big"])]
        [DefaultValue("Small")]
        [DrawTicks]
        public string GodModeOutlineSize = "Small";

        [Header("PlannedFeatures")]

        [DefaultValue(false)]
        public bool ShowUIElementsHitbox;

        [DefaultValue(false)]
        public bool ShowUIElementsSizes;

        [OptionStrings(["Small", "Medium", "Big"])]
        [DefaultValue("Small")]
        [DrawTicks]
        public string ButtonSizes = "Small";

        public override void OnChanged()
        {
            if (ModContent.GetInstance<Config>() == null)
            {
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                if (sys == null)
                {
                    Log.Info("MainSystem is null");
                    return;
                }
                else
                {
                    Log.Info("MainSystem is not null");
                    sys.mainState.configButton.IsConfigOpen = false;
                }

                return;
            }

            ChangeGodModeOutline();
            ChangeToggleButtonVisibility();
            ChangeButtonTextVisibility();
        }

        private void ChangeButtonTextVisibility()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            sys?.mainState?.ToggleButtonTextVisibility();
        }

        private void ChangeToggleButtonVisibility()
        {
            if (ModContent.GetInstance<Config>() == null)
                return;

            MainSystem sys = ModContent.GetInstance<MainSystem>();

            if (ShowToggleButton)
                sys?.SetUIStateToMyState();
            else
                sys?.SetUIStateToNull();
        }

        private void ChangeGodModeOutline()
        {
            // add null check for the class itself in case it's called before the mod is loaded
            // idk
            if (ModContent.GetInstance<Config>() == null)
                return;

            int type = ModContent.ItemType<BorderShaderDye>();

            if (GodModeOutlineSize == "Small")
            {
                Asset<Effect> smallOutlineEffect = Mod.Assets.Request<Effect>("Effects/LessOutlineEffect");
                GameShaders.Armor.BindShader(type, new ArmorShaderData(smallOutlineEffect, "Pass0"));
            }
            else if (GodModeOutlineSize == "Big")
            {
                Asset<Effect> bigOutlineEffect = Mod.Assets.Request<Effect>("Effects/OutlineEffect");
                GameShaders.Armor.BindShader(type, new ArmorShaderData(bigOutlineEffect, "Pass0"));
            }
        }
    }
}
