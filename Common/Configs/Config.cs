using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using SquidTestingMod.Helpers;
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

        [Range(-500f, 500f)]
        [Increment(100f)]
        [DefaultValue(typeof(Vector2), "0, 0")]
        public Vector2 NPCSpawnLocation = new Vector2(0, 0);

        [Header("Buttons")]
        [DefaultValue(60)]
        [Range(60, 80)]
        [Increment(10)]
        public int ButtonSize;

        [DefaultValue(true)]
        public bool AnimateButtons = true;

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
        public bool KeepRunningWhenFocusLost = true;

        [DefaultValue(true)]
        public bool ShowTooltipsDebugPanels = true;

        // Debug Panel Config Settings Goes Here For Temporary Storage

        // World Panel Config Settings Goes Here For Temporary Storage

        public override void OnChanged()
        {
            // Here we can update the game based on the new config values
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            if (sys == null || sys.mainState == null) return;

            // Check if any of the button options have changed
            int currentHash = 0;
            currentHash |= Conf.ShowToggleButton ? 1 : 0;
            currentHash |= (Conf.ShowConfigButton ? 1 : 0) << 1;
            currentHash |= (Conf.ShowItemButton ? 1 : 0) << 2;
            currentHash |= (Conf.ShowNPCButton ? 1 : 0) << 3;
            currentHash |= (Conf.ShowDebugButton ? 1 : 0) << 4;
            currentHash |= (Conf.ShowPlayerButton ? 1 : 0) << 5;
            currentHash |= (Conf.ShowWorldButton ? 1 : 0) << 6;
            currentHash |= (Conf.ShowReloadSPButton ? 1 : 0) << 7;
            currentHash |= (Conf.ShowReloadMPButton ? 1 : 0) << 8;
            currentHash |= Conf.ButtonSize << 9;

            if (currentHash != _prevButtonVisibilityHash)
            {
                _prevButtonVisibilityHash = currentHash; // update stored state
                sys.mainState.UpdateButtonsAfterConfigChanged();
                Log.Info("Updated button visibility");
            }
            else
            {
                Log.Info("No changes in button visibility");
            }
        }

        private int _prevButtonVisibilityHash;
    }

    internal static class Conf
    {
        // Instance
        public static Config ConfigInstance => ModContent.GetInstance<Config>();

        // Reload header
        public static string ModToReload => ConfigInstance.ModToReload;
        public static bool SaveWorldOnReload => ConfigInstance.SaveWorldOnReload;
        public static bool ClearClientLogOnReload => ConfigInstance.ClearClientLogOnReload;

        // NPC/Item Spawner
        public static Vector2 NPCSpawnLocation => ConfigInstance.NPCSpawnLocation;

        // Buttons to show
        public static int ButtonSize => ConfigInstance.ButtonSize;
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
