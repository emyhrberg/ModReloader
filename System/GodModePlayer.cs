using System.Text;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SquidTestingMod.Core.System
{
    public class GodModePlayer : ModPlayer
    {
        public bool IsGodModeEnabled { get; private set; } = false;

        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            // Prevent damage from NPCs if God Mode is enabled
            return !IsGodModeEnabled && base.CanBeHitByNPC(npc, ref cooldownSlot);
        }

        public override bool CanBeHitByProjectile(Projectile proj)
        {
            // Prevent damage from projectiles if God Mode is enabled
            return !IsGodModeEnabled && base.CanBeHitByProjectile(proj);
        }

        public void ToggleGodMode()
        {
            // Toggle the state of God Mode and display a message
            IsGodModeEnabled = !IsGodModeEnabled;
            string status = IsGodModeEnabled ? "Enabled" : "Disabled";
            Main.NewText($"God Mode {status}", IsGodModeEnabled ? Color.Green : Color.Red);
        }

        // draw glow effect
        public override void PostUpdate()
        {
            if (IsGodModeEnabled)
            {
                Lighting.AddLight(Main.LocalPlayer.Center, Color.AliceBlue.ToVector3() * 1f);
            }
        }
    }
}