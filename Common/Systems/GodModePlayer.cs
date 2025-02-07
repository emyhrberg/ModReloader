using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Systems
{
    public class GodModePlayer : ModPlayer
    {
        public static bool GodMode;
        private static ModKeybind ToggleGodModeKeybind;

        public override void OnEnterWorld()
        {
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
