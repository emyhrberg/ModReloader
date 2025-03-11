

using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.UI.Panels;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class LowAggro : ModPlayer
    {
        public override void PostUpdate()
        {
            if (PlayerCheatManager.LowAggro)
            {
                Main.LocalPlayer.aggro = -9999;
            }
        }
    }
}

