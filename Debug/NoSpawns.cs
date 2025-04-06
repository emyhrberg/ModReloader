using Terraria;
using Terraria.ModLoader;

namespace ModHelper.Debug
{
    public class NoSpawns : GlobalNPC
    {
#if DEBUG
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {

            // Prevent division by zero
            spawnRate = 9999999; // Effectively stops enemy spawns
            maxSpawns = 0;

            // Kill all active NPCs
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].friendly)
                {
                    // instakill
                    Main.npc[i].StrikeInstantKill();
                }
            }

            // Call the base method to keep the original behavior when SpawnRateEnabled is false
            base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
        }
#endif
    }
}
