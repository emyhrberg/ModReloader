using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class UseFaster : ModPlayer
    {

        public override float UseTimeMultiplier(Item item)
        {
            // Fast speed for tools
            if (PlayerCheatManager.PlaceFaster)
            {
                if (item.createTile != -1 || item.createWall != -1)
                {
                    return 0.0001f; // Near-instant speed
                }
            }

            // Default speed
            return 1;
        }

        public override float UseAnimationMultiplier(Item item)
        {
            if (PlayerCheatManager.PlaceFaster)
            {
                // Animation update speed
                if (item.createTile != -1 || item.createWall != -1)
                    return 0.0001f; // Near-instant speed
            }

            // Default speed
            return 1;
        }
    }
}

