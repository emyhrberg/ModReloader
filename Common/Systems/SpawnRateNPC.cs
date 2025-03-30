using Terraria;
using Terraria.ModLoader;

namespace ModHelper.Common.Systems
{
    public class SpawnRateNPC : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            // Use the synchronized multiplier from the ModSystem
            float multiplier = SpawnRateSystem.Multiplier;

            // Prevent division by zero
            if (multiplier <= 0f)
            {
                spawnRate = int.MaxValue; // Effectively stops enemy spawns
                maxSpawns = 0;
                return;
            }

            // Apply spawn rate modifier
            spawnRate = (int)(spawnRate / multiplier);
            maxSpawns = (int)(maxSpawns * multiplier);

            // Ensure spawn rate doesn't go below 1
            if (spawnRate < 1) spawnRate = 1;

            // Ensure max spawns don't drop below 1
            if (maxSpawns < 1) maxSpawns = 1;
        }
    }
}