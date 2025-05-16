using System.Collections.Generic;
using System.ComponentModel;
using ModReloader.Helpers;
using ModReloader.UI.Elements.ConfigElements;
using Terraria.ModLoader.Config;

namespace ModReloader.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Reload")]

        [CustomModConfigItem(typeof(ModSourcesConfig))]
        public List<string> ModsToReload = [];

        [DefaultValue(true)]
        public bool AutoJoinWorld;

        [DefaultValue(false)]
        public bool SaveWorldBeforeReloading;

        [CustomModConfigItem(typeof(PlayerPicker))]
        [DefaultValue(0)]
        public int Player;

        [CustomModConfigItem(typeof(WorldPicker))]
        [DefaultValue(0)]
        public int World;

        [Header("ExtraInfo")]

        [DefaultValue(true)]
        public bool ShowDebugInfo;

        [DefaultValue(true)]
        public bool ShowMainMenuInfo;

        [DefaultValue(true)]
        public bool ShowErrorMenuInfo;

        [DefaultValue(true)]
        public bool ShowCopyToClipboardButton;

        [Header("Misc")]

        [DrawTicks]
        [OptionStrings(["File", "Folder"])]
        [DefaultValue("Folder")]
        public string OpenLogType;

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
