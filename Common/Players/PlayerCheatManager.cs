using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Configs;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    [Autoload(Side = ModSide.Client)]
    public class PlayerCheatManager : ModPlayer
    {
        // Abilities
        public static bool God = false;
        public static bool Noclip = false;
        public static bool TeleportMode = false;
        public static bool LightMode = false;
        public static bool KillAura = false;
        public static bool MineAura = false;

        // Build
        public static bool PlaceAnywhere = false;
        public static bool PlaceFaster = false;
        public static bool MineFaster = false;

        // Check if any cheat is active
        public static bool IsAnyCheatEnabled =>
            God || Noclip || TeleportMode || LightMode || KillAura || MineAura ||
            PlaceAnywhere || PlaceFaster || MineFaster;

        // Helper to set all cheats on/off
        public static void SetAllCheats(bool value)
        {
            God = value;
            Noclip = value;
            TeleportMode = value;
            LightMode = value;
            KillAura = value;
            MineAura = value;
            PlaceAnywhere = value;
            PlaceFaster = value;
            MineFaster = value;
        }

        public static void ToggleNoclip() => ToggleCheat(ref Noclip, "Noclip");
        public static void ToggleGod() => ToggleCheat(ref God, "God Mode");
        public static void ToggleLightMode() => ToggleCheat(ref LightMode, "Light Mode");
        public static void ToggleTeleportMode() => ToggleCheat(ref TeleportMode, "Teleport Mode");
        public static void ToggleMineFaster() => ToggleCheat(ref MineFaster, "Mine Faster");
        public static void TogglePlaceFaster() => ToggleCheat(ref PlaceFaster, "Place Faster");
        public static void TogglePlaceAnywhere() => ToggleCheat(ref PlaceAnywhere, "Place Anywhere");
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

            if (Conf.ShowCombatTextOnToggle)
            {
                bool isOn = value is bool bVal && bVal;
                CombatText.NewText(Main.LocalPlayer.getRect(), Color.White, $"{name} {(isOn ? "On" : "Off")}");
            }
        }
    }
}
