using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using SquidTestingMod.UI;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace SquidTestingMod.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Reload")]
        [DefaultValue("EnterYourModHere")]
        public string ModToReload = "EnterYourModHere";

        [DefaultValue(true)]
        public bool SaveWorldOnReload = true;

        [DefaultValue(true)]
        public bool ClearClientLogOnReload = true;

        [Header("ItemNPCSpawner")]
        [DefaultValue(1000)]
        [Range(100, 10000)]
        [Increment(1000)]
        public int MaxItemsToDisplay;

        [Range(-500f, 500f)]
        [Increment(100f)]
        [DefaultValue(typeof(Vector2), "0, 0")]
        public Vector2 NPCSpawnLocation;

        [Header("ButtonsToShow")]

        [DefaultValue(true)]
        public bool ShowToggleButton = true;

        [DefaultValue(true)]
        public bool ShowConfigButton = true;

        [DefaultValue(true)]
        public bool ShowItemButton = true;

        [DefaultValue(true)]
        public bool ShowNPCButton = true;

        [DefaultValue(true)]
        public bool ShowDebugButton = true;

        [DefaultValue(true)]
        public bool ShowPlayerButton = true;

        [DefaultValue(true)]
        public bool ShowWorldButton = true;

        [DefaultValue(true)]
        public bool ShowReloadSPButton = true;

        [DefaultValue(true)]
        public bool ShowReloadMPButton = true;

        [Header("Misc")]
        [DefaultValue(true)]
        public bool ShowCombatTextOnToggle = true;

        [DefaultValue(true)]
        public bool DrawGodGlow = true;

        [DefaultValue(true)]
        public bool AnimateButtons = true;

        [DefaultValue(true)]
        public bool KeepRunningWhenFocusLost = true;

        [DefaultValue(true)]
        public bool ShowTooltipsDebugPanels = true;

        // Debug Panel Config Settings Goes Here For Temporary Storage

        // World Panel Config Settings Goes Here For Temporary Storage

        public override void OnChanged()
        {
            // Here we can update the game based on the new config values
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            sys?.mainState?.UpdateButtonsAfterConfigChanged();
        }
    }

    internal static class C
    {
        // Instance
        public static Config ConfigInstance => ModContent.GetInstance<Config>();

        // Reload header
        public static string ModToReload => ConfigInstance.ModToReload;
        public static bool SaveWorldOnReload => ConfigInstance.SaveWorldOnReload;
        public static bool ClearClientLogOnReload => ConfigInstance.ClearClientLogOnReload;

        // NPC/Item Spawner
        public static int MaxItemsToDisplay => ConfigInstance.MaxItemsToDisplay;
        public static Vector2 NPCSpawnLocation => ConfigInstance.NPCSpawnLocation;

        // Buttons to show
        public static bool ShowToggleButton => ConfigInstance.ShowToggleButton;
        public static bool ShowConfigButton => ConfigInstance.ShowConfigButton;
        public static bool ShowItemButton => ConfigInstance.ShowItemButton;
        public static bool ShowNPCButton => ConfigInstance.ShowNPCButton;
        public static bool ShowDebugButton => ConfigInstance.ShowDebugButton;
        public static bool ShowPlayerButton => ConfigInstance.ShowPlayerButton;
        public static bool ShowWorldButton => ConfigInstance.ShowWorldButton;
        public static bool ShowReloadSPButton => ConfigInstance.ShowReloadSPButton;
        public static bool ShowReloadMPButton => ConfigInstance.ShowReloadMPButton;

        // Misc
        public static bool DrawGodGlow => ConfigInstance.DrawGodGlow;
        public static bool ShowCombatTextOnToggle => ConfigInstance.ShowCombatTextOnToggle;
        public static bool AnimateButtons => ConfigInstance.AnimateButtons;
        public static bool KeepRunningWhenFocusLost => ConfigInstance.KeepRunningWhenFocusLost;
        public static bool ShowTooltipsDebugPanels => ConfigInstance.ShowTooltipsDebugPanels;
    }
}
