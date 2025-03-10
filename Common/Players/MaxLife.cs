using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class MaxLife : ModPlayer
    {
        public static int maxLife = 0; // Default max HP

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            health = StatModifier.Default;
            mana = StatModifier.Default;

            // Apply max life modification
            health.Base = maxLife;
        }
    }
}
