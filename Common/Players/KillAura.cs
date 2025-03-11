

using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Configs;
using Terraria;
using Terraria.DataStructures;
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

