using System;
using System.Collections.Generic;
using System.Linq;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Common.Systems;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{


    public class PlayerCheatManager : ModPlayer
    {
        // Booleans for each cheat
        public static bool God = false;
        public static bool Noclip = false;
        public static bool LightMode = false;
        public static bool KillAura = false;
        public static bool MineAura = false;
        public static bool BuildAnywhere = false;
        public static bool BuildFaster = false;

        // Master list of cheats
        public static List<CheatDefinition> Cheats =
        [
            new CheatDefinition("God", "Makes you immortal", () => God, v => God = v),
            new CheatDefinition("Noclip", "Fly through blocks (use shift+ctrl to go faster)", () => Noclip, v => Noclip = v),
            new CheatDefinition("Light", "Light up your surroundings", () => LightMode, v => LightMode = v),
            new CheatDefinition("Kill Aura", "Insta-kill touching enemies", () => KillAura, v => KillAura = v),
            new CheatDefinition("Mine Aura", "Mine tiles around you", () => MineAura, v => MineAura = v),
            new CheatDefinition("Build Anywhere", "Place blocks in mid-air", () => BuildAnywhere, v => BuildAnywhere = v),
            new CheatDefinition("Build Faster", "Place and mine faster", () => BuildFaster, v => BuildFaster = v)
        ];

        // Called by “Toggle All”
        public static void SetAllCheats(bool value)
        {
            foreach (var cheat in Cheats)
                cheat.SetValue(value);
        }

        public override void OnEnterWorld()
        {
            base.OnEnterWorld();

            // Toggle super mode
            if (Conf.EnterWorldSuperMode)
            {
                SetAllCheats(true);
                // noclip and mine aura are not included in super mode
                Noclip = false;
                MineAura = false;

                SpawnRateMultiplier.Multiplier = 0f;
            }
        }

        public class CheatDefinition
        {
            public string Name { get; }
            public string Description { get; }
            public Func<bool> GetValue { get; }
            public Action<bool> SetValue { get; }

            public CheatDefinition(string name, string description, Func<bool> getter, Action<bool> setter)
            {
                Name = name;
                Description = description;
                GetValue = getter;
                SetValue = setter;
            }

            public void Toggle() => SetValue(!GetValue());
        }
    }
}
