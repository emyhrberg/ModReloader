using System;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Xna.Framework;
using ModHelper.Helpers;
using ModHelper.UI;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ModHelper.Common.Configs
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Reload")]

        [DefaultValue(true)]
        public bool Reload = true;

        [DefaultValue("")]
        public string LatestModToReload = "";

        [DefaultValue(true)]
        public bool SaveWorldOnReload = true;

        [DefaultValue(false)]
        public bool ClearClientLogOnReload = false;

        [Header("UI")]

        [OptionStrings(["Left", "Bottom"])]
        [DefaultValue("Bottom")]
        public string ButtonPosition;

        [Range(50f, 80f)]
        [Increment(5f)]
        [DefaultValue(70)]
        public float ButtonSize = 70;

        [Range(300, 700f)]
        [Increment(50f)]
        [DefaultValue(460)]
        public float PanelWidth = 460;

        [Range(300, 700f)]
        [Increment(50f)]
        [DefaultValue(500f)]
        public float PanelHeight = 500;

        [DefaultValue(false)]
        public bool DraggablePanels = false;

        [Header("Game")]
        [DefaultValue(false)]
        public bool EnterWorldSuperMode = false;

        [DefaultValue(true)]
        public bool GameKeepRunning = true;

        [DefaultValue(true)]
        public bool ShowGameKeepRunningText = true;

        [DefaultValue(true)]
        public bool GodGlow = true;

        [Header("Logging")]
        [DefaultValue(true)]
        public bool LogToLogFile = true;

        [DefaultValue(true)]
        public bool LogToChat = true;

        [Header("NPCSpawner")]

        [Range(-500f, 500f)]
        [Increment(100f)]
        [DefaultValue(typeof(Vector2), "0, 0")]
        public Vector2 SpawnOffset = new Vector2(0, 0);

        public override void OnChanged()
        {
            base.OnChanged();

            // Get mainstate
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys == null)
            {
                Log.Info("MainSystem is null in Config.OnChanged()");
                return;
            }
            MainState mainState = sys.mainState;

            if (mainState == null)
            {
                Log.Info("MainState is null in Config.OnChanged()");
                return;
            }

            // Delete all buttons and re-add them
            mainState.AreButtonsShowing = true;
            mainState.AllButtons.Clear();
            mainState.RemoveAllChildren();
            mainState.AddEverything();

            // expand so we can see the changes
            mainState.collapse.SetCollapsed(false);

            Log.Info("Config.OnChanged() ran successfully");
        }
    }

    internal static class Conf
    {
        // NOTE: Stolen from CalamityMod
        // https://github.com/CalamityTeam/CalamityModPublic/blob/e0838b30b8fdf86aeb4037931c8123703acd7c7e/CalamityMod.cs#L550
        #region Force ModConfig save (Reflection)
        internal static void ForceSaveConfig(Config cfg)
        {
            // There is no current way to manually save a mod configuration file in tModLoader.
            // The method which saves mod config files is private in ConfigManager, so reflection is used to invoke it.
            try
            {
                MethodInfo saveMethodInfo = typeof(ConfigManager).GetMethod("Save", BindingFlags.Static | BindingFlags.NonPublic);
                if (saveMethodInfo is not null)
                    saveMethodInfo.Invoke(null, [cfg]);
            }
            catch
            {
                Log.Error("An error occurred while manually saving ModConfig!.");
            }
        }
        #endregion

        // Instance
        public static Config C => ModContent.GetInstance<Config>();
    }
}
