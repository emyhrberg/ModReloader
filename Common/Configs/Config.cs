using System.Collections.Generic;
using System.ComponentModel;
using ModReloader.Common.Configs.ConfigElements;
using ModReloader.Common.Configs.ConfigElements.ModSources;
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
    }

    public static class Conf
    {
        public static void Save()
        {
            try
            {
                ConfigManager.Save(C);
            }
            catch
            {
                Log.Error("An error occurred while manually saving ModConfig!.");
            }
        }

        // Instance of the Config class
        // Use it like 'Conf.C.YourConfigField' for easy access to the config values
        public static Config C => ModContent.GetInstance<Config>();
    }
}
