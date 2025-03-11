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

        [DefaultValue(true)]
        public bool KeepRunningWhenFocusLost = true;

        [DefaultValue(false)]
        public bool EnterWorldSuperMode = false;

        [DefaultValue(false)]
        public bool HideCollapseButton = false;
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
        public static bool KeepRunningWhenFocusLost => C.KeepRunningWhenFocusLost;
        public static bool HideCollapseButton => C.HideCollapseButton;
    }
}
