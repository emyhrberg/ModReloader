using Terraria;
using Terraria.ModLoader;

namespace ModHelper.Common.Players
{
    public class TeleportMode : ModPlayer
    {
        public override void PostUpdate()
        {
            PlayerCheatManager p = Main.LocalPlayer.GetModPlayer<PlayerCheatManager>();

            // Only teleport if TeleportMode is on, right mouse is held, AND mouse not consumed by UI.
            if (p.GetTeleportWithRightClick() && Main.mouseRight && !Main.LocalPlayer.mouseInterface)
            {
                Main.LocalPlayer.Teleport(Main.MouseWorld);
            }
        }
    }
}