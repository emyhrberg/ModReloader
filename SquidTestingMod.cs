using System.IO;
using SquidTestingMod.Networking;
using SquidTestingMod.Reload;
using Terraria.ModLoader;

namespace SquidTestingMod
{
    // Use both sides currently (it is default if none is set), but can be changed to client only if needed
    // [Autoload(Side = ModSide.Client)]
    public class SquidTestingMod : Mod
    {
        public override void Load()
        {
            ReloadUtils.ReadData();
        }

        public override void Unload()
        {
            ReloadUtils.WriteData();
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            PacketManager.HandlePacket(reader, whoAmI);
        }
    }
}
