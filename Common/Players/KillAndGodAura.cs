using Terraria;
using Terraria.ModLoader;

namespace ModHelper.Common.Players
{
    public class KillAndGodAura : GlobalNPC
    {
        public override void ResetEffects(NPC npc)
        {
            base.ResetEffects(npc);

            PlayerCheatManager p = Main.LocalPlayer.GetModPlayer<PlayerCheatManager>();

            if (p.GetKillAura() && p.GetGod())
            {
                // Log.SlowInfo("Both KillAura and God mode are enabled!");

                // check if within range 1 blocks
                if (npc.Distance(Main.LocalPlayer.Center) < 5 * 16 && !npc.townNPC)
                {
                    // Instantly kill the NPC...
                    npc.StrikeInstantKill();
                }
            }
        }
    }
}