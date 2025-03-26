

using Terraria;
using Terraria.ModLoader;

namespace ModHelper.Common.Players
{
    public class KillAura : ModPlayer
    {
        // public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        // {
        //     if (PlayerCheatManager.KillAura)
        //     {
        //         npc.StrikeInstantKill();
        //     }
        // }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            // base.ModifyHitByNPC(npc, ref modifiers);

            // If both KillAura and God mode are enabled...
            if (PlayerCheatManager.KillAura)
            {
                // Instantly kill the NPC...
                npc.StrikeInstantKill();
            }
        }
    }
}

