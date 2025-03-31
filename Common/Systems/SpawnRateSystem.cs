using Microsoft.Xna.Framework;
using ModHelper.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace ModHelper.Common.Systems
{
    // Make this class a ModSystem to leverage tModLoader's built-in synchronization
    public class SpawnRateSystem : ModSystem
    {
        // This will be automatically synchronized
        public static float Multiplier { get; private set; } = 1f;
        private static bool didPrint = false;

        public override void Load()
        {
            Multiplier = 1f;
            didPrint = false;
        }

        public static void SetMultiplier(float value)
        {
            Multiplier = value;

            if (value == 0 && !didPrint)
            {
                // ChatHelper.NewText("All hostile NPCs butchered!", Color.Green);
                didPrint = true;
            }
            else if (value > 0)
            {
                didPrint = false;
            }
        }
    }
}