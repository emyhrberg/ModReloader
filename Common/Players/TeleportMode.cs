

using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Configs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class TeleportMode : ModPlayer
    {
        public override void PostUpdate()
        {
            if (PlayerCheatManager.TeleportMode && Main.mouseRight)
            {
                // holding down right mouse button down
                Main.LocalPlayer.Teleport(Main.MouseWorld);
            }
        }
    }
}

