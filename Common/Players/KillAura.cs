

using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class KillAura : ModPlayer
    {
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            Log.Info("KillAura is: " + PlayerCheatManager.KillAura);
            if (PlayerCheatManager.KillAura)
            {
                Log.Info("KillAura activated");
                npc.StrikeInstantKill();
            }
        }
    }
}

