using System;
using System.Collections.Generic;
using System.ComponentModel;
using ModReloader.Common.Configs.ConfigElements.ModsConfigElements;
using ModReloader.Common.Configs.ConfigElements.PlayerAndWorld;
using ModReloader.Core.Features.MainMenuFeatures;
using ModReloader.Core.Features.Reload;
using Terraria.ModLoader.Config;

namespace ModReloader.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("ModsToReload")]

        [CustomModConfigItem(typeof(ModSourcesConfig))]
        public List<string> ModsToReload = [];

        [Header("ReloadOptions")]

        [CustomModConfigItem(typeof(PlayerIndexSliderElement))]
        [DefaultValue("")]
        public string Player; // player index in Main.PlayerList

        [CustomModConfigItem(typeof(WorldIndexSliderElement))]
        public string World; // world index in Main.WorldList

        [DefaultValue(true)]
        public bool AutoJoinWorld;

        [DefaultValue(true)]
        public bool SaveWorld;

        [DefaultValue(false)]
        public bool DebugReload;

        [Header("ExtraInfo")]

        [DefaultValue(true)]
        public bool ShowDebugInfo;

        [DefaultValue(true)]
        public bool ShowMainMenuInfo;

        [DefaultValue(true)]
        public bool ShowErrorMenuInfo;

        [DefaultValue(true)]
        public bool ShowCopyToClipboardButton;

        public enum WorldSize { ExtraSmall, Small, Medium, Large }
        [DrawTicks]
        [DefaultValue(WorldSize.Small)]
        public WorldSize CreateTestWorldSize;

        public enum WorldDifficulty { Normal, Expert, Master, Journey }
        [DrawTicks]
        [DefaultValue(WorldDifficulty.Normal)]
        public WorldDifficulty CreateTestWorldDifficulty;

        [Header("Misc")]

        [DefaultValue(true)]
        public bool RightClickToolOptions;

        [DefaultValue(true)]
        public bool LogLevelPersistOnReloads;

        [DefaultValue(false)]
        public bool ClearLogOnReload;

        [DefaultValue(true)]
        public bool LogDebugMessages;

        [DefaultValue(true)]
        public bool ShowBackToMainMenu;

        public override void OnChanged()
        {
            base.OnChanged();
            UpdateMainMenuReloadTooltip();
        }

        private void UpdateMainMenuReloadTooltip()
        {
            // Update reload tooltip
            var mainMenuSys = ModContent.GetInstance<MainMenuSystem>();
            if (mainMenuSys == null)
            {
                Log.Info("main menu sys is null!");
                return;
            }
            var state = mainMenuSys.state;
            if (state == null) return;

            Main.LoadPlayers();
            Main.LoadWorlds();

            int p = Utilities.FindPlayerId(Conf.C.Player);
            int w = Utilities.FindWorldId(Conf.C.World);

            state.UpdatePlayerIndex(p);
            state.UpdateWorldIndex(w);
        }
    }

    public static class Conf
    {
        /// <summary> Quick instance getter. Usage example: Conf.C.Field /// </summary>
        public static Config C => ModContent.GetInstance<Config>();
    }
}
