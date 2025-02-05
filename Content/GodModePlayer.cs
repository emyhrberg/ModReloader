using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;
using SquidTestingMod.src;

namespace SquidTestingMod
{
    public class GodModePlayer : ModPlayer
    {
        public static bool GodMode;
        private static ModKeybind ToggleGodModeKeybind;

        public override void OnEnterWorld()
        {
            Config c = ModContent.GetInstance<Config>();
            ModContent.GetInstance<SquidTestingMod>().Logger.Info("enter world godmode!" + c.StartInGodMode);
            GodMode = c.StartInGodMode;
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
                Main.NewText($"God Mode: {(GodMode ? "Enabled" : "Disabled")}", GodMode ? Color.Green : Color.Red);
            }
        }

        // HOOKS, SETTING DAMAGE TO 0.
        // Todo does it work for drowning/lava? lol not important
        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot) => !GodMode;
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers) { if (GodMode) modifiers.FinalDamage *= 0; }
        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo) { if (GodMode) hurtInfo.Damage = 0; }

    }
}
