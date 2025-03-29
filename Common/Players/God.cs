using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModHelper.Common.Players
{
    public class God : ModPlayer
    {
        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            PlayerCheatManager p = Main.LocalPlayer.GetModPlayer<PlayerCheatManager>();

            // If God mode is enabled and KillAura is NOT enabled, be immune.
            if (p.GetGod() && !p.GetKillAura())
                return true;
            return false;
        }

        public override void PostUpdate()
        {
            PlayerCheatManager p = Main.LocalPlayer.GetModPlayer<PlayerCheatManager>();

            if (p.GetGod())
            {
                // Set player stats to max
                Player.statLife = Player.statLifeMax2;
                Player.statMana = Player.statManaMax2;
            }

            base.PostUpdate();
        }

        public override void PreUpdateBuffs()
        {
            PlayerCheatManager p = Main.LocalPlayer.GetModPlayer<PlayerCheatManager>();

            // Remove all debuffs if god mode is enabled
            // Kinda scuffed because it shows the debuff icon for a split second
            if (p.GetGod())
            {
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    int buffID = Player.buffType[i];
                    bool isDebuff = Main.debuff[buffID];

                    if (buffID <= 0)
                        continue;

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
            PlayerCheatManager p = Main.LocalPlayer.GetModPlayer<PlayerCheatManager>();

            if (p.GetGod())
            {
                // Don't kill the player
                Player.statLife = Player.statLifeMax2;
                return false;
            }
            return true;
        }
    }
}

