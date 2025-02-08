using SquidTestingMod.Common.Configs;
using SquidTestingMod.UI;
using Terraria.ModLoader;

namespace SquidTestingMod.Helpers
{
    public static class Log
    {
        private static Mod ModInstance => ModContent.GetInstance<SquidTestingMod>();

        public static void Info(string message)
        {
            ModInstance.Logger.Info(message);
        }

        public static void Debug(string message)
        {
            ModInstance.Logger.Info(message);
        }

        public static void Warn(string message)
        {
            ModInstance.Logger.Warn(message);
        }

        public static void Error(string message)
        {
            ModInstance.Logger.Error(message);
        }
    }
}