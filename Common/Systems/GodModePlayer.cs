using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI;
using Terraria;
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
            Log.Info("enter world godmode is set to" + c.Gameplay.StartInGodMode);
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
                // update the button texture
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                sys.mainState.godButton.UpdateTexture();
                Log.Info("God mode toggled. God mode is now " + GodMode);
            }
        }

        // HOOKS, SETTING DAMAGE TO 0.
        // Todo does it work for drowning/lava? lol not important
        public override void OnHurt(Player.HurtInfo info)
        {
            Log.Info($"OnHurt: Damage = {info.Damage}, Source = {info.DamageSource}");
            if (GodMode)
            {
                // Heal the damage immediately.
                Player.statLife += info.Damage;
                Log.Info("GodMode active: Healed damage in OnHurt.");
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            Log.Info($"OnHitByNPC: Damage = {hurtInfo.Damage}, NPC type = {npc.type}");
            if (GodMode)
            {
                // Heal the damage immediately.
                Player.statLife += hurtInfo.Damage;
                Log.Info("GodMode active: Healed NPC damage in OnHitByNPC.");
            }
            base.OnHitByNPC(npc, hurtInfo);
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            Log.Info($"OnHitByProjectile: Damage = {hurtInfo.Damage}, Projectile type = {proj.type}");
            if (GodMode)
            {
                // Heal the damage immediately.
                Player.statLife += hurtInfo.Damage;
                Log.Info("GodMode active: Healed projectile damage in OnHitByProjectile.");
            }
            base.OnHitByProjectile(proj, hurtInfo);
        }

        public override void PostUpdate()
        {
            if (GodMode)
            {
                // Optionally, clear harmful debuffs.
                for (int i = 0; i < Player.buffType.Length; i++)
                {
                    Player.ClearBuff(i);
                }
                // Restore player's life to max.
                Player.statLife = Player.statLifeMax2;
            }
        }
    }
}
