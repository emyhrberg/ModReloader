using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Configs
{
    /// <summary>
    /// Helper class to avoid redundant ModContent.GetInstance<Config>() calls
    /// Is used like this: Conf.x where x is a property in the Config class
    /// </summary>
    internal static class Conf
    {
        // Instance
        public static Config Instance => ModContent.GetInstance<Config>();

        // Reload header
        public static string ModToReload => Instance.ModToReload;
        public static bool SaveWorldOnReload => Instance.SaveWorldOnReload;
        public static bool ClearClientLogOnReload => Instance.ClearClientLogOnReload;

        // NPC/Item Spawner
        public static int MaxItemsToDisplay => Instance.MaxItemsToDisplay;
        public static Vector2 NPCSpawnLocation => Instance.NPCSpawnLocation;

        // Misc
        public static bool StartInGodMode => Instance.StartInGodMode;
        public static bool DrawGodGlow => Instance.DrawGodGlow;
        public static bool ShowCombatTextOnToggle => Instance.ShowCombatTextOnToggle;
        public static bool ReloadButtonsOnly => Instance.ReloadButtonsOnly;
        public static bool AnimateButtons => Instance.AnimateButtons;
    }
}
