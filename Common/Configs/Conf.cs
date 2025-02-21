using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Configs
{
    internal static class Conf
    {
        public static Config Instance => ModContent.GetInstance<Config>();

        public static string ModToReload => Instance.ModToReload;

        public static bool SaveWorldOnReload => Instance.SaveWorldOnReload;

        public static bool ClearClientLogOnReload => Instance.ClearClientLogOnReload;

        public static bool StartInGodMode => Instance.StartInGodMode;

        public static int MaxItemsToDisplay => Instance.MaxItemsToDisplay;

        public static Vector2 NPCSpawnLocation => Instance.NPCSpawnLocation;

    }
}
