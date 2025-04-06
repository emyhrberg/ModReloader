using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ModHelper.Debug
{
    public class GodMode : ModPlayer
    {
#if DEBUG
        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            return true; // Always immune to damage

            // return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
        }
    }
#endif
}