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
        public string ModToReload = "EnterYourModHere";
        public bool SaveWorldOnReload = true;
        public bool ClearClientLogOnReload = true;

        [Header("Misc")]
        public NPCSpawnerConfig NPCSpawner = new();
        public bool EnterWorldSuperMode = false;
        public bool DrawGodGlow = true;
        public bool KeepRunningWhenFocusLost = true;
        public bool HideCollapseButton = false;
    }

    public class NPCSpawnerConfig
    {
        [Range(-500f, 500f)]
        [Increment(100f)]
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
        public static Vector2 NPCSpawnLocation => C.NPCSpawner.SpawnOffset;
        public static bool EnterWorldSuperMode => C.EnterWorldSuperMode;
        public static bool DrawGodGlow => C.DrawGodGlow;
        public static bool KeepRunningWhenFocusLost => C.KeepRunningWhenFocusLost;
        public static bool HideCollapseButton => C.HideCollapseButton;
    }
}
