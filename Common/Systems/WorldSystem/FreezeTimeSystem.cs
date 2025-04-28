using Terraria.ModLoader;

namespace ModHelper.Common.Systems.WorldSystem
{
    public static class FreezeTimeManager
    {
        public static bool FreezeTime = false;
    }

    public class FreezeTimeSystem : ModSystem
    {
        public override void ModifyTimeRate(ref double timeRate, ref double tileUpdateRate, ref double eventUpdateRate)
        {
            if (FreezeTimeManager.FreezeTime)
            {
                timeRate = 0.0;
                // tileUpdateRate = 0.0;
                // eventUpdateRate = 0.0;
            }
        }
    }
}