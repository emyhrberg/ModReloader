using Terraria;
using Terraria.ModLoader;

namespace EliteTestingMod.Common.Players
{
    public class TeleportMode : ModPlayer
    {
        public override void PostUpdate()
        {
            // Only teleport if TeleportMode is on, right mouse is held, AND mouse not consumed by UI.
            if (PlayerCheatManager.TeleportWithRightClick && Main.mouseRight && !Main.LocalPlayer.mouseInterface)
            {
                Main.LocalPlayer.Teleport(Main.MouseWorld);
            }
        }
    }
}