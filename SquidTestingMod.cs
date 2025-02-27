using SquidTestingMod.Helpers;
using SquidTestingMod.PacketHandlers;
using System.IO;
using Terraria.ModLoader;

namespace SquidTestingMod
{
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
            //On_ModLoader.Unload -= On_ModLoader_Unload;
        }

        public override void Unload()
        {
            ClientDataHandler.WriteData();
            //On_ModLoader.Unload += On_ModLoader_Unload;
        }

    }
}
