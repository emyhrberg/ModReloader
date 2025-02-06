using Terraria.ModLoader;

namespace SquidTestingMod
{
    public static class Log
    {
        private static Mod ModInstance => ModContent.GetInstance<SquidTestingMod>();

        public static void Info(string message)
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