using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class MineFaster : ModPlayer
    {
        public override float UseTimeMultiplier(Item item)
        {
            // Fast speed for tools
            if (PlayerCheatManager.MineFaster)
            {
                if (item.pick > 0 || item.axe > 0 || item.hammer > 0)
                {
                    return 0.1f; // Near-instant speed
                }
            }

            // Default speed
            return 1;
        }

        public override float UseAnimationMultiplier(Item item)
        {
            if (PlayerCheatManager.MineFaster)
            {
                // Animation update speed
                if (item.pick > 0 || item.axe > 0 || item.hammer > 0)
                    return 0.1f; // Near-instant speed
            }

            // Default speed
            return 1;
        }
    }
}