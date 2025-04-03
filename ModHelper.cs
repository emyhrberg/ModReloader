using ModHelper.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModHelper
{
    // If no Autoload(Side) is provided, it will default to Both (which is wanted in this case)
    // [Autoload(Side = ModSide.Client)]
    // [Autoload(Side = ModSide.Both)]
    public class ModHelper : Mod
    {
        public override void Load()
        {
            // if (Main.netMode != NetmodeID.Server)
            // ClientDataHandler.ReadData();
        }

        public override void Unload()
        {
            // if (Main.netMode != NetmodeID.Server)
            // ClientDataHandler.WriteData();
        }
    }
}