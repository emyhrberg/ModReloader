using System.IO;
using Terraria.ModLoader;
namespace EliteTestingMod.PacketHandlers
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
            var p = ModContent.GetInstance<EliteTestingMod>().GetPacket();
            p.Write(HandlerType);
            p.Write(packetType);
            return p;
        }
    }
}