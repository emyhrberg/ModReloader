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

        [Header("ModsToReload")]

        [CustomModConfigItem(typeof(ModSourcesConfig))]
        public List<string> ModsToReload = [];

        [Header("ReloadOptions")]

        [CustomModConfigItem(typeof(PlayerPicker))]
        [DefaultValue(0)]
        public int Player;

        [CustomModConfigItem(typeof(WorldPicker))]
        [DefaultValue(0)]
        public int World;

        [DefaultValue(true)]
        public bool AutoJoinWorld;

        [DefaultValue(true)]
        public bool AutoSaveWorld;

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

        [DefaultValue(true)]
        public bool RightClickToolOptions;

        [DefaultValue(true)]
        public bool LogLevelPersistOnReloads;

        [DefaultValue(true)]
        public bool MoveChat;

        [DefaultValue(true)]
        public bool ShowBuilderToggle;

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
