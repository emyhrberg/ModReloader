using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SquidTestingMod.PacketHandlers
{
    //Taken from https://github.com/tModLoader/tModLoader/wiki/intermediate-netcode#good-practice-managing-many-packets
    internal abstract class PacketHandler
    {
        internal byte HandlerType { get; set; }

        public abstract void HandlePacket(BinaryReader reader, int fromWho);

        protected PacketHandler(byte handlerType)
        {
            HandlerType = handlerType;
        }

        protected ModPacket GetPacket(byte packetType, int fromWho)
        {
            var p = ModContent.GetInstance<SquidTestingMod>().GetPacket();
            p.Write(HandlerType);
            p.Write(packetType);
            return p;
        }
    }
}
