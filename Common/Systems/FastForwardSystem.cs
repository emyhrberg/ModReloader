using Microsoft.Xna.Framework;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Systems
{
    /// <summary>
    /// 0 => normal speed (1×),
    /// 1 => 2× speed,
    /// 2 => 3× speed, etc.
    /// </summary>
    public class FastForwardSystem : ModSystem
    {
        public static int speedup = 0;

        public override void Load()
        {
            On_Main.DoUpdate += UpdateExtraTimes;
        }

        private void UpdateExtraTimes(On_Main.orig_DoUpdate orig, Main self, ref GameTime gameTime)
        {
            // Always call the original method once
            orig(self, ref gameTime);

            // Log.Info($"Speedup: {speedup}");

            // Then call it 'speedup' more times for the extra speed.
            for (int k = 0; k < speedup; k++)
            {
                orig(self, ref gameTime);
            }
        }
    }
}
