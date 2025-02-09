using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.UI
{
    internal class SpawnSystem : GlobalNPC
    {
        public static float spawnRateModifier = 1;

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (spawnRate == 0)
            {
                spawnRate = int.MaxValue;
                maxSpawns = 0;
                return;
            }

            spawnRate = (int)(spawnRate / spawnRateModifier);
            maxSpawns = (int)(maxSpawns * spawnRateModifier);
        }
    }
}