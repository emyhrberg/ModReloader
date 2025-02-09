using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Systems
{
    public class GodModePlayer : ModPlayer
    {
        public static bool GodMode;
        private static ModKeybind ToggleGodModeKeybind;

        public override void OnEnterWorld()
        {
            if (Main.netMode != NetmodeID.SinglePlayer)
                return;

            Config c = ModContent.GetInstance<Config>();
            Log.Info("Enter world: GodMode is set to " + c.Gameplay.StartInGodMode);
            GodMode = c.Gameplay.StartInGodMode;
        }

        public override void Load()
        {
            ToggleGodModeKeybind = KeybindLoader.RegisterKeybind(Mod, "Toggle God Mode", Keys.H);
        }

        public override void Unload()
        {
            ToggleGodModeKeybind = null;
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (ToggleGodModeKeybind.JustPressed)
            {
                GodMode = !GodMode;
                // Update the button texture
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                sys.mainState.godButton.UpdateTexture();
                Log.Info("God mode toggled. Now: " + GodMode);
            }
        }

        // *** HOOKS TO DISABLE TAKING DAMAGE ***
        public override void PostUpdate()
        {
            if (GodMode)
                Player.statLife = Player.statLifeMax2;
        }

        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if (GodMode)
                return true;
            return false;
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (GodMode)
            {
                // Don't kill the player
                Player.statLife = Player.statLifeMax2;
                return false;
            }
            return true;
        }
    }
}
