using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class God : ModPlayer
    {
        // This property is updated via network packets.
        public bool GodGlow { get; set; } = false;

        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if (PlayerCheatManager.God)
                return true;
            return false;
        }

        public override void PostUpdate()
        {
            if (PlayerCheatManager.God)
            {
                Player.statLife = Player.statLifeMax2;
                Player.statMana = Player.statManaMax2;
            }
            base.PostUpdate();
        }

        public override void PreUpdateBuffs()
        {
            if (PlayerCheatManager.God)
            {
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    int buffID = Player.buffType[i];
                    if (buffID <= 0)
                        continue;
                    bool isDebuff = Main.debuff[buffID];
                    if (isDebuff && !BuffID.Sets.NurseCannotRemoveDebuff[buffID])
                    {
                        Player.DelBuff(i);
                        i--;
                    }
                }
            }
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (PlayerCheatManager.God)
            {
                Player.statLife = Player.statLifeMax2;
                return false;
            }
            return true;
        }
    }
}
