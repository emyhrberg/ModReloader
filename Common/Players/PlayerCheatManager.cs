using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.UI;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class PlayerCheatManager : ModPlayer
    {
        // Cheats
        public static bool God = false;
        public static bool Noclip = false;
        public static bool LightMode = false;
        public static bool KillAura = false;
        public static bool MineAura = false;
        public static bool BuildAnywhere = false;
        public static bool BuildFaster = false;

        // Check if any cheat is active
        public static bool IsAnyCheatEnabled =>
            God || Noclip || LightMode || KillAura || MineAura ||
            BuildAnywhere || BuildFaster;

        // Helper to set all cheats on/off
        public static void SetAllCheats(bool value)
        {
            God = value;
            Noclip = value;
            LightMode = value;
            KillAura = value;
            MineAura = value;
            BuildAnywhere = value;
            BuildFaster = value;
        }

        public static void ToggleNoclip() => ToggleCheat(ref Noclip, "Noclip");
        public static void ToggleGod() => ToggleCheat(ref God, "God Mode");
        public static void ToggleLightMode() => ToggleCheat(ref LightMode, "Light Mode");
        public static void ToggleBuildFaster() => ToggleCheat(ref BuildFaster, "Build Faster");
        public static void ToggleBuildAnywhere() => ToggleCheat(ref BuildAnywhere, "Build Anywhere");
        public static void ToggleKillAura() => ToggleCheat(ref KillAura, "Kill Aura");
        public static void ToggleMineAura() => ToggleCheat(ref MineAura, "Mine Aura");

        // Helper to toggle cheats
        // This does not need to be modified when adding new cheats
        private static void ToggleCheat<T>(ref T value, string name) where T : struct
        {
            if (value is bool booleanValue)
            {
                value = (T)(object)!booleanValue;
            }

            bool isOn = value is bool bVal && bVal;
            CombatText.NewText(Main.LocalPlayer.getRect(), Color.White, $"{name} {(isOn ? "On" : "Off")}");
        }

        public override void OnEnterWorld()
        {
            base.OnEnterWorld();

            // Toggle super mode
            if (Conf.EnterWorldSuperMode)
            {
                SetAllCheats(true);
                ToggleNoclip();
                ToggleMineAura();
                ToggleKillAura();

                SpawnRateMultiplier.Multiplier = 0f;

                // Update text
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                sys.mainState.playerPanel.RefreshCheatTexts();
            }
        }
    }
}
