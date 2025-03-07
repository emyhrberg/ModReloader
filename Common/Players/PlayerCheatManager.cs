using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Configs;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    /// <summary>
    /// Gives player abilities for:
    /// GodMode - Makes the player invincible
    /// FastMode - Increases player speed
    /// BuildMode - Infinite range, instant mining and more
    /// NoClip - Move through blocks
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class PlayerCheatManager : ModPlayer
    {
        // Player movement
        public static bool God = false;
        public static bool Noclip = false;
        public static bool TeleportMode = false;
        public static bool UseFaster = false;
        public static bool PlaceAnywhere = false;

        // Misc
        public static bool InvisibleToEnemies = false;
        public static bool LightMode = false;

        // Helper to toggle all cheats
        public static bool IsAnyCheatEnabled => God || Noclip || TeleportMode || UseFaster || PlaceAnywhere || InvisibleToEnemies || LightMode;

        // Helper to set all cheats toggle
        public static void SetAllCheats(bool value)
        {
            God = value;
            Noclip = value;
            TeleportMode = value;
            UseFaster = value;
            PlaceAnywhere = value;
            InvisibleToEnemies = value;
            LightMode = value;
        }

        // Helper to toggle cheats using a T struct
        private static void ToggleCheat<T>(ref T value, string name) where T : struct
        {
            value = value is bool b ? (T)(object)!b : value;
            if (Conf.ShowCombatTextOnToggle)
            {
                bool isOn = value is bool bVal && bVal;
                CombatText.NewText(Main.LocalPlayer.getRect(),
                isOn ? Color.Green : Color.Red,
                $"{name} {(isOn ? "On" : "Off")}");
            }
        }

        public static void ToggleNoclip()
        {
            ToggleCheat(ref Noclip, "Noclip");
            // show puffy cloud effect when enabling noclip
            if (Noclip)
            {
                var cloudJump = new CloudInABottleJump();
                bool playSound = true;
                cloudJump.OnStarted(Main.LocalPlayer, ref playSound);
                cloudJump.ShowVisuals(Main.LocalPlayer);
            }
        }

        public static void ToggleGod() => ToggleCheat(ref God, "God Mode");
        public static void ToggleLightMode() => ToggleCheat(ref LightMode, "Light Mode");
        public static void ToggleTeleportMode() => ToggleCheat(ref TeleportMode, "Teleport Mode");
        public static void ToggleUseFaster() => ToggleCheat(ref UseFaster, "Use Faster");
        public static void TogglePlaceAnywhere() => ToggleCheat(ref PlaceAnywhere, "Place Anywhere");
        public static void ToggleInvisibleToEnemies() => ToggleCheat(ref InvisibleToEnemies, "Invisible To Enemies");
    }
}