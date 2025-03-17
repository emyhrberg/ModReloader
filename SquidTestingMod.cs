using System.IO;
using SquidTestingMod.Helpers;
using SquidTestingMod.PacketHandlers;
using Terraria.ModLoader;

namespace SquidTestingMod
{
    // Use both sides currently (it is default if none is set), but can be changed to client only if needed
    [Autoload(Side = ModSide.Client)]
    public class SquidTestingMod : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModNetHandler.HandlePacket(reader, whoAmI);
        }

        public override void Load()
        {
            ClientDataHandler.ReadData();
        }

        public override void Unload()
        {
            ClientDataHandler.WriteData();
        }
    }
}
