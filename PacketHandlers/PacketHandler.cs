using System.IO;
using Terraria.ModLoader;
namespace ModHelper.PacketHandlers
{
    //Taken from https://github.com/tModLoader/tModLoader/wiki/intermediate-netcode#good-practice-managing-many-packets
    internal abstract class PacketHandler
    {
        // The type of the packet handler. This is used to identify the packet handler.
        internal byte HandlerType { get; set; }

        public abstract void HandlePacket(BinaryReader reader, int fromWho);

        // Constructor for the packet handler.
        protected PacketHandler(byte handlerType)
        {
            HandlerType = handlerType;
        }

        // This is the packet that will be sent to the client and server.
        // It will be used to send data to the client and server.
        protected ModPacket GetPacket(byte packetType, int fromWho)
        {
            var p = ModContent.GetInstance<ModHelper>().GetPacket();
            p.Write(HandlerType);
            p.Write(packetType);
            return p;
        }
    }
}