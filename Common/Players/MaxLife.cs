using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class MaxLife : ModPlayer
    {
        public static int maxLife = 0; // Om noll, används vanilla-värdet

        public override void ModifyMaxStats(out StatModifier health, out StatModifier mana)
        {
            base.ModifyMaxStats(out health, out mana);

            // Initiera maxLife med vanillavärdet om det inte satts än
            if (maxLife == 0)
            {
                maxLife = (int)health.Base;
            }

            // Sätt spelarens max liv till exakt maxLife-värdet
            health.Base = maxLife;
        }
    }
}
