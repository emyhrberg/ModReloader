

using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Configs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class UseFaster : ModPlayer
    {
        public override void PostUpdate()
        {
            if (PlayerCheatManager.TeleportMode && Main.mouseRight)
            {
                // holding down right mouse button down
                Main.LocalPlayer.Teleport(Main.MouseWorld);
            }
        }

        public override float UseTimeMultiplier(Item item)
        {
            // Fast speed for tools
            if (PlayerCheatManager.UseFaster)
            {
                if (item.createTile != -1 || item.createWall != -1 || item.pick > 0 || item.axe > 0 || item.hammer > 0)
                {
                    return 0.0001f; // Near-instant speed
                }
            }

            // Default speed
            return 1;
        }

        public override float UseAnimationMultiplier(Item item)
        {
            if (PlayerCheatManager.UseFaster)
            {
                // If the item is a tile, wall, or tool, make it near-instant
                if (item.createTile != -1 || item.createWall != -1 || item.pick > 0 || item.axe > 0 || item.hammer > 0)
                    return 0.0001f; // Near-instant speed
            }
            // Default speed
            return 1;
        }
    }
}

