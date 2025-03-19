using System;
using System.ComponentModel;
using System.Reflection;
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
        [DefaultValue("SquidTestingMod")]
        public string ModToReload = "EnterYourModHere";

        [DefaultValue(false)]
        public bool SaveWorldOnReload = false;

        [DefaultValue(false)]
        public bool ClearClientLogOnReload = false;

        [Header("Misc")]

        [DrawTicks]
        [OptionStrings(["left", "bottom"])]
        [DefaultValue("bottom")]
        public string ButtonsPosition;

        [DefaultValue(null)]
        public NPCSpawnerConfig NPCSpawner = new();

        [DefaultValue(false)]
        public bool EnterWorldSuperMode = false;

        [DefaultValue(false)]
        public bool HideCollapseButton = false;


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

            // Delete all buttons and re-add them
            sys.mainState.AllButtons.Clear();
            sys.mainState.RemoveAllChildren();
            sys.mainState.AddEverything();
            sys.mainState.collapse.UpdateCollapseImage();
            Log.Info("Config.OnChanged() ran successfully");
        }
    }

    public class NPCSpawnerConfig
    {
        [Range(-500f, 500f)]
        [Increment(100f)]
        [DefaultValue(typeof(Vector2), "0, 0")]
        public Vector2 SpawnOffset = new Vector2(0, 0);
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

        // Reload header
        public static string ModToReload => C.ModToReload;
        public static bool SaveWorldOnReload => C.SaveWorldOnReload;
        public static bool ClearClientLogOnReload => C.ClearClientLogOnReload;

        // Misc
        public static string ButtonsPosition => C.ButtonsPosition;
        public static Vector2 NPCSpawnLocation => C.NPCSpawner.SpawnOffset;
        public static bool EnterWorldSuperMode => C.EnterWorldSuperMode;
        public static bool HideCollapseButton => C.HideCollapseButton;
    }
}
