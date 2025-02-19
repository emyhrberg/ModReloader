using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.UI;
using Terraria;
using Terraria.Graphics.Shaders;
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

        [Header("Gameplay")]
        public GameplayConfig Gameplay = new();

        [Header("ItemBrowser")]
        public ItemBrowserConfig ItemBrowser = new();

        [Header("DisableButtons")]
        public DisableButtonConfig DisableButton = new();

        public class ReloadConfig
        {
            [DefaultValue("SquidTestingMod")]
            public string ModToReload;

            [DefaultValue(false)]
            public bool SaveWorldOnReload;

            [DefaultValue(false)]
            public bool ClearClientLogOnReload = false;

            [OptionStrings(["Disabled", "Singleplayer", "Multiplayer"])]
            [DefaultValue("Disabled")]
            [DrawTicks]
            public string AutoloadWorld = "Disabled";

            [DefaultValue(100)]
            [Range(100, 5000)]
            [Increment(100)]
            public int WaitingTimeBeforeNavigatingToModSources;

            [DefaultValue(100)]
            [Increment(100)]
            [Range(100, 5000)]
            public int WaitingTimeBeforeBuildAndReload;
        }

        public class GameplayConfig
        {
            [DefaultValue(false)]
            public bool AlwaysSpawnBossOnTopOfPlayer;

            [DefaultValue(false)]
            public bool StartInGodMode;
        }

        public class ItemBrowserConfig
        {
            [DefaultValue(100)]
            [Range(0, 10000)]
            [Increment(500)]
            public int MaxItemsToDisplay = 1000;
        }

        public class DisableButtonConfig
        {
            [DefaultValue(false)]
            [ReloadRequired]
            public bool DisableConfig;

            [DefaultValue(false)]
            [ReloadRequired]
            public bool DisableReload;

            [DefaultValue(false)]
            [ReloadRequired]
            public bool DisableItemBrowser;

            [DefaultValue(false)]
            [ReloadRequired]
            public bool DisableNPCBrowser;

            [DefaultValue(false)]
            [ReloadRequired]
            public bool DisableGod;

            [DefaultValue(false)]
            [ReloadRequired]
            public bool DisableFast;

            [DefaultValue(false)]
            [ReloadRequired]
            public bool DisableHitboxes;

            [DefaultValue(false)]
            [ReloadRequired]
            public bool DisableUIElements;

            [DefaultValue(false)]
            [ReloadRequired]
            public bool DisableLog;
        }

        public override void OnChanged()
        {
            // Here we can update the game based on the new config values
            // MainSystem sys = ModContent.GetInstance<MainSystem>();
            // sys.mainState.UpdateButtons();
        }
    }
}
