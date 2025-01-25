using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SkipSelect.Core.System
{
    public class GodModePlayer : ModPlayer
    {
        public bool IsGodModeEnabled { get; private set; } = true;

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
    }
}