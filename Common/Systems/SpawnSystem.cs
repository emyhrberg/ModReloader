using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.UI
{
    internal class SpawnSystem : GlobalNPC
    {
        public static float spawnRateModifier = 1;

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {

            // Log.Info($"Modifier: {spawnRateModifier} | Spawn rate: {spawnRate}, max spawns: {maxSpawns}");

            if (spawnRateModifier == 0)
            {
                spawnRate = 0;
                maxSpawns = 0;
                return;
            }

            spawnRate = (int)(spawnRate / spawnRateModifier);
            maxSpawns = (int)(maxSpawns * spawnRateModifier);
        }

        // Log enemies on screen
        public override void PostAI(NPC npc)
        {
            if (npc.whoAmI == 0)
            {
                int enemyCount = 0;
                foreach (NPC n in Main.npc)
                {
                    if (n.active && !n.friendly)
                        enemyCount++;
                }
                // Log.Info("Enemies on screen: " + enemyCount);
            }
        }

    }
}