

using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class KillAura : ModPlayer
    {
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (PlayerCheatManager.KillAura)
            {
                npc.StrikeInstantKill();
            }
        }
    }
}

