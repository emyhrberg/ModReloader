using Microsoft.Xna.Framework;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace ModHelper.Common.Systems
{
    public class EnemySpawnSystem : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            base.EditSpawnRate(player, ref spawnRate, ref maxSpawns);

            // Prevent division by zero
            if (SpawnRateMultiplier.Multiplier <= 0f)
            {
                spawnRate = int.MaxValue; // Effectively stops enemy spawns
                maxSpawns = 0;
                return;
            }

            // Apply spawn rate modifier
            spawnRate = (int)(spawnRate / SpawnRateMultiplier.Multiplier);
            maxSpawns = (int)(maxSpawns * SpawnRateMultiplier.Multiplier);

            // Ensure spawn rate doesn't go below 1
            if (spawnRate < 1) spawnRate = 1;

            // Ensure max spawns don't drop below 1 (prevents potential infinite loops)
            if (maxSpawns < 1) maxSpawns = 1;

            // Log.Info("Set spawn rate to: " + spawnRate);
        }
    }

    public static class SpawnRateMultiplier
    {
        public static float Multiplier = 1f; // Default to normal spawn rate (1x)

        public static bool didPrint = false;

        public static void SetSpawnRateMultiplier(float value)
        {
            Multiplier = value;

            // If spawn rate is set to 0, butcher all hostile NPCs
            if (value == 0)
            {
                foreach (var npc in Main.npc)
                {
                    if (npc.active && npc.life > 0 && npc.CanBeChasedBy()) // Ensures it's a valid enemy
                    {
                        npc.StrikeInstantKill();
                        // NPC.HitInfo hit = npc.CalculateHitInfo(npc.lifeMax, -npc.direction, false, 0f);
                        // hit.InstantKill = true;
                        // npc.active = false;
                        // Log.Info("butcher " + npc.FullName);
                    }
                }
                if (!didPrint)
                {
                    ChatHelper.NewText("All hostile NPCs butchered!", new Color(226, 57, 39));
                    didPrint = true;
                }
            }
            else
            {
                didPrint = false;
            }
        }
    }
}
