using Terraria;
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
        }

        public override void OnEnterWorld()
        {
            // base.OnEnterWorld();

            // Set time to 6:00 AM -ish for daytime light
            if (DebugHelper.IsDebugBuild)
            {
                Main.time = 13000;
                Main.dayTime = true;
            }
        }
#else
        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if (DebugHelper.IsDebugBuild)
            {
                return true; // Always immune to damage in debug mode
            }
            return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
        }
#endif

    }
}