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
        public static bool IsGodModeOn = false;

        public override void OnEnterWorld()
        {
            if (Main.dedServ)
                return;

            Config c = ModContent.GetInstance<Config>();
            IsGodModeOn = c.StartInGodMode;
            Log.Info("Enter world: GodMode is set to " + c.StartInGodMode);
        }

        // *** HOOKS TO DISABLE TAKING DAMAGE ***
        public override void PostUpdate()
        {
            if (IsGodModeOn)
                Player.statLife = Player.statLifeMax2;
        }

        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if (IsGodModeOn)
                return true;
            return false;
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (IsGodModeOn)
            {
                // Don't kill the player
                Player.statLife = Player.statLifeMax2;
                return false;
            }
            return true;
        }

        // *** TODO HOOKS TO DISABLE DEBUFFS ***
        // Use player.buffimmune for each debuff
        public override void UpdateBadLifeRegen()
        {
            if (IsGodModeOn)
            {
                // Make the player immune to all debuffs
                for (int i = 0; i < Player.buffImmune.Length; i++)
                {
                    Player.buffImmune[i] = true;
                }
            }
        }
    }
}
