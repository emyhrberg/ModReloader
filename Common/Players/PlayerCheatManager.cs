using System;
using System.Collections.Generic;
using System.Linq;
using EliteTestingMod.Common.Configs;
using EliteTestingMod.Common.Systems;
using EliteTestingMod.UI;
using EliteTestingMod.UI.Elements;
using Terraria.ModLoader;

namespace EliteTestingMod.Common.Players
{
    public class PlayerCheatManager : ModPlayer
    {
        public record Cheat(string Name, string Description, Func<bool> GetValue, Action<bool> SetValue)
        {
            public void Toggle() => SetValue(!GetValue());
        }

        // Booleans for each cheat
        public static bool God = false;
        public static bool Noclip = false;
        public static bool LightMode = false;
        public static bool KillAura = false;
        public static bool MineAura = false;
        public static bool BuildAnywhere = false;
        public static bool BuildFaster = false;
        public static bool TeleportWithRightClick = false;

        // Master list of cheats
        public static List<Cheat> Cheats =
        [
            new Cheat("God", "Makes you immortal", () => God, v => God = v),
            new Cheat("Noclip", "Fly through blocks (use shift+ctrl to go faster)", () => Noclip, v => Noclip = v),
            new Cheat("Light", "Light up your surroundings", () => LightMode, v => LightMode = v),
            new Cheat("Kill Aura", "Insta-kill touching enemies", () => KillAura, v => KillAura = v),
            new Cheat("Mine Aura", "Mine tiles around you", () => MineAura, v => MineAura = v),
            new Cheat("Build Anywhere", "Place blocks in mid-air", () => BuildAnywhere, v => BuildAnywhere = v),
            new Cheat("Build Faster", "Place and mine faster", () => BuildFaster, v => BuildFaster = v),
            new Cheat("Teleport With Right Click", "Right click to teleport to your mouse position", () => TeleportWithRightClick, v => TeleportWithRightClick = v)
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
                EnableSupermode();
            }
        }

        public void EnableSupermode()
        {
            SetAllCheats(true);
            Noclip = false;
            MineAura = false;
            KillAura = false;
            TeleportWithRightClick = false;
            SpawnRateMultiplier.Multiplier = 0f;

            // Update the enabled texts all enabled except mine aura and noclip
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            PlayerPanel p = sys.mainState.playerPanel;
            foreach (Option o in p.cheatOptions)
            {
                if (o.text == "Mine Aura" || o.text == "Noclip" || o.text == "Kill Aura")
                {
                    o.SetState(Option.State.Disabled);
                }
                else
                {
                    o.SetState(Option.State.Enabled);
                }
            }

            // WorldPanel w = sys.mainState.worldPanel;
            // w.spawnRateSlider.SetValue(0f);

            Conf.C.EnterWorldSuperMode = true;
            Conf.ForceSaveConfig(Conf.C);
        }

        public void DisableSupermode()
        {
            SetAllCheats(false);
            SpawnRateMultiplier.Multiplier = 1f;
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            // WorldPanel w = sys.mainState.worldPanel;
            // w.spawnRateSlider.SetValue(0f);

            // Update the enabled texts all Disabled
            PlayerPanel p = sys.mainState.playerPanel;
            foreach (Option o in p.cheatOptions)
            {
                o.SetState(Option.State.Disabled);
            }

            // Disable the config option
            Conf.C.EnterWorldSuperMode = false;
            Conf.ForceSaveConfig(Conf.C);
        }
    }
}
