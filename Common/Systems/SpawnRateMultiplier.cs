using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Systems
{
    public class EnemySpawnSystem : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);
            // Log.Info("[Before] SpawnRate: " + spawnRate + " MaxSpawns: " + maxSpawns);

            // spawnRate = (int)(spawnRate / SpawnRateMultiplier.Multiplier);
            // maxSpawns = (int)(maxSpawns * SpawnRateMultiplier.Multiplier);

            // Log.Info("[After] SpawnRate: " + spawnRate + " MaxSpawns: " + maxSpawns);

        }
    }

    public static class SpawnRateMultiplier
    {
        public static float Multiplier = 1f;
    }
}