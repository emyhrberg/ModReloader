using ModReloader.Common.Configs.ConfigElements.ModsConfigElements;
using ModReloader.Common.Configs.ConfigElements.PlayerPicker;
using ModReloader.Common.Configs.ConfigElements.WorldPicker;
using ModReloader.Core.Features;
using ModReloader.Core.Features.MainMenuFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Channels;
using Terraria.ModLoader.Config;

namespace ModReloader.Common.Configs;
public class Config : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Header("ModsToReload")]

    [CustomModConfigItem(typeof(ModSourcesConfig))]
    public List<string> ModsToReload = [];

    [Header("ReloadOptions")]

    [CustomModConfigItem(typeof(PlayerDefinitionElement))]
    public PlayerDefinition Player = new();

    [CustomModConfigItem(typeof(WorldDefinitionElement))]
    public WorldDefinition World = new();

    // Used for testing, do not delete!
    //public NPCDefinition NPCTest;

    //[CustomModConfigItem(typeof(WorldIndexSliderElement))]
    //public string World; // world index in Main.WorldList

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

    [DefaultValue(true)]
    public bool ShowBackToMainMenu;

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

    public override void OnLoaded()
    {
        base.OnLoaded();
        EnsureDefaultPlayerAndWorld();
    }

    public override void OnChanged()
    {
        Log.Info("Config changed");

        base.OnChanged();
        UpdateMainMenuReloadTooltip();
    }

    private void UpdateMainMenuReloadTooltip()
    {
        var mainMenuSys = ModContent.GetInstance<MainMenuSystem>();
        if (mainMenuSys?.state == null)
            return;

        Main.LoadPlayers();
        Main.LoadWorlds();

        int p = Conf.C.Player != null ? Utilities.FindPlayerId(Conf.C.Player.Name) : 0;
        int w = Conf.C.World != null ? Utilities.FindWorldId(Conf.C.World.Name) : 0;

        mainMenuSys.state.UpdatePlayerIndex(p);
        mainMenuSys.state.UpdateWorldIndex(w);
    }

    private void EnsureDefaultPlayerAndWorld()
    {
        bool changed = false;

        // Player
        if (Player == null)
        {
            Main.LoadPlayers();

            if (Main.PlayerList.Count > 0)
            {
                Player = new PlayerDefinition(Main.PlayerList[0].Path);
                Log.Info("Default player set to: " + Player.Name);
                changed = true;
            }
        }

        // World
        if (World == null)
        {
            Main.LoadWorlds();

            if (Main.WorldList.Count > 0)
            {
                World = new WorldDefinition(Main.WorldList[0].Path);
                Log.Info("Default world set to: " + Main.WorldList[0].Name);
                changed = true;
            }
        }

        if (changed)
        {
            ConfigManager.Save(this);
            SaveChanges();
        }
    }

}

public static class Conf
{
    /// <summary> 
    /// Quick instance getter. 
    /// Usage example: Conf.C.YourField 
    /// /// </summary>
    public static Config C => ModContent.GetInstance<Config>();
}
