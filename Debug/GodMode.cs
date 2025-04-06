using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace ModHelper.Debug
{
    public class GodMode : ModPlayer
    {
        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if (DebugConfig.IS_DEBUGGING)
                return true; // Always immune to damage

            return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
        }
    }
}