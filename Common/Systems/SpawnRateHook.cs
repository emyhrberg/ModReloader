using System;
using System.Reflection;
using EliteTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace EliteTestingMod.Common.Systems
{
    public class SpawnRateHook : ModSystem
    {
        public static int StoredSpawnRate { get; private set; } = 0;
        public static int StoredMaxSpawns { get; private set; } = 0;

        // Define a delegate matching the original method signature
        private delegate void GlobalNPC_EdittSpawnRate(GlobalNPC self, Player player, ref int spawnRate, ref int maxSpawns);

        public override void Load()
        {
            // Get the method Terraria.ModLoader.GlobalNPC
            // Assembly a = typeof(Main).Assembly;
            // Type GlobalNPC = a.GetType("Terraria.ModLoader.GlobalNPC");
            // MethodInfo editSpawnRateMethod = GlobalNPC.GetMethod("EditSpawnRate", BindingFlags.Public | BindingFlags.Instance);


            MethodInfo editSpawnRateMethod = typeof(EnemySpawnSystem).GetMethod("EditSpawnRate", BindingFlags.Public | BindingFlags.Instance);

            if (editSpawnRateMethod != null)
            {
                MonoModHooks.Add(editSpawnRateMethod, EditSpawnRateHook);
            }
            else
            {
                Log.Warn("Could not find EnemySpawnSystem.EditSpawnRate.");
            }
        }

        public override void Unload()
        {
            StoredSpawnRate = 0;
            StoredMaxSpawns = 0;
            Log.Info("Unloaded SpawnRateHook and reset stored values.");
        }

        // Hook method with matching signature for MonoModHooks.Add:
        // First parameter is the delegate for the original method
        private void EditSpawnRateHook(GlobalNPC_EdittSpawnRate orig, GlobalNPC self, Player player, ref int spawnRate, ref int maxSpawns)
        {
            // Call the original method with ref parameters
            orig(self, player, ref spawnRate, ref maxSpawns);

            // Store the values after they've been modified
            StoredSpawnRate = spawnRate;
            StoredMaxSpawns = maxSpawns;
        }
    }
}