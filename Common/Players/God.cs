using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class God : ModPlayer
    {
        // MAYBE use this?
        // prob not needed, it works with ImmuneTo and PreKill
        // public override void PreUpdate()
        // {
        // base.PreUpdate();
        // }

        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if (PlayerCheatManager.God)
                return true; // Immune to all damage
            return false;
        }

        public override void PostUpdate()
        {
            if (PlayerCheatManager.God)
            {
                // Set player stats to max
                Player.statLife = Player.statLifeMax2;
                Player.statMana = Player.statManaMax2;
            }

            base.PostUpdate();
        }

        public override void PreUpdateBuffs()
        {
            // Remove all debuffs if god mode is enabled
            // Kinda scuffed because it shows the debuff icon for a split second
            if (PlayerCheatManager.God)
            {
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    int buffID = Player.buffType[i];
                    bool isDebuff = Main.debuff[buffID];

                    if (buffID <= 0)
                        continue;

                    // Log buff info
                    // Main.NewText($"Buff ID: {buffID}, Name: {Lang.GetBuffName(buffID)}, isDebuff: {isDebuff}", Color.Yellow);
                    // Log.Info("Buff ID: " + buffID + ", Name: " + Lang.GetBuffName(buffID) + ", isDebuff: " + isDebuff);

                    // Remove debuffs if they are not in the NurseCannotRemoveDebuff exclusion list.
                    // Nurse exclusion list includes e.g werewolf, cozy fire, heart lantern, etc.
                    if (isDebuff && !BuffID.Sets.NurseCannotRemoveDebuff[buffID])
                    {
                        Player.DelBuff(i);
                        i--; // Adjust index to avoid skipping any buff
                    }
                }
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (PlayerCheatManager.God)
            {
                // Don't kill the player
                Player.statLife = Player.statLifeMax2;
                return false;
            }
            return true;
        }
    }
}

