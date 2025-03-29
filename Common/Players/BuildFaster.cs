using Terraria;
using Terraria.ModLoader;

namespace ModHelper.Common.Players
{
    public class BuildFaster : ModPlayer
    {
        public override float UseTimeMultiplier(Item item)
        {
            PlayerCheatManager p = Main.LocalPlayer.GetModPlayer<PlayerCheatManager>();

            // Fast speed for tools
            if (p.GetBuildFaster())
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
            PlayerCheatManager p = Main.LocalPlayer.GetModPlayer<PlayerCheatManager>();

            if (p.GetBuildFaster())
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