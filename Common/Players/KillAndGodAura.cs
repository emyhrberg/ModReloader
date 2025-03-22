using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class KillAndGodAura : GlobalNPC
    {
        public override void ResetEffects(NPC npc)
        {
            base.ResetEffects(npc);

            if (PlayerCheatManager.KillAura && PlayerCheatManager.God)
            {
                Log.SlowInfo("Both KillAura and God mode are enabled!");

                // check if within range 1 blocks
                if (npc.Distance(Main.LocalPlayer.Center) < 5 * 16)
                {
                    // Instantly kill the NPC...
                    npc.StrikeInstantKill();
                }
            }
        }
    }
}