using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class BuildFaster : ModPlayer
    {
        public override float UseTimeMultiplier(Item item)
        {
            // Fast speed for tools
            if (PlayerCheatManager.BuildFaster)
            {
                if (item.pick > 0 || item.axe > 0 || item.hammer > 0)
                {
                    return 0.1f; // Near-instant speed
                }

                if (item.createTile != -1 || item.createWall != -1)
                {
                    return 0.1f; // Near-instant speed
                }
            }

            // Default speed
            return 1;
        }

        public override float UseAnimationMultiplier(Item item)
        {
            if (PlayerCheatManager.BuildFaster)
            {
                // Animation update speed
                if (item.pick > 0 || item.axe > 0 || item.hammer > 0)
                    return 0.1f; // Near-instant speed

                // Animation update speed
                if (item.createTile != -1 || item.createWall != -1)
                    return 0.1f; // Near-instant speed
            }

            // Default speed
            return 1;
        }
    }
}