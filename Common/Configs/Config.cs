using System;
using System.ComponentModel;
using SquidTestingMod.UI;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SquidTestingMod.Common.Configs
{
    public class Config : ModConfig
    {
        // CLIENT SIDE
        public override ConfigScope Mode => ConfigScope.ClientSide;

        // RELOAD CONFIG
        [Header("Reload")]
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

        // GAMEPLAY CONFIG
        [Header("Gameplay")]

        [DefaultValue(false)]
        public bool StartInGodMode;

        // ITEM SPAWNER CONFIG
        [Header("ItemNPCBrowser")]
        [DefaultValue(1000)]
        [Range(500, 2000)]
        [Increment(500)]
        public int MaxItemsToDisplay;

        // DISABLE BUTTON CONFIG
        [Header("DisableButtons")]
        [DefaultValue(false)]
        public bool DisableConfig;

        [DefaultValue(false)]
        public bool DisableReload;

        [DefaultValue(false)]
        public bool DisableItemBrowser;

        [DefaultValue(false)]
        public bool DisableNPCBrowser;

        [DefaultValue(false)]
        public bool DisableGod;

        [DefaultValue(false)]
        public bool DisableFast;

        [DefaultValue(false)]
        public bool DisableHitboxes;

        [DefaultValue(false)]
        public bool DisableUIElements;

        [DefaultValue(false)]
        public bool DisableLog;

        public override void OnChanged()
        {
            // Here we can update the game based on the new config values
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys == null)
            {
                return;
            }

            sys.mainState.UpdateButtons();
        }
    }
}
