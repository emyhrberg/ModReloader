using System.IO;
namespace ModReloader.PacketHandlers
{
    // Reference:
    // https://github.com/tModLoader/tModLoader/wiki/intermediate-netcode#good-practice-managing-many-packets

    // This class acts as a base class for all packet handlers.
    // When adding new packet handlers, you should inherit from this class and implement the HandlePacket method.
    internal abstract class BasePacketHandler
    {
        // The type of the packet handler. This is used to identify the packet handler.
        internal byte HandlerType { get; set; }

        public abstract void HandlePacket(BinaryReader reader, int fromWho);

        // Constructor for the packet handler.
        protected BasePacketHandler(byte handlerType)
        {
            HandlerType = handlerType;
        }

        // This is the packet that will be sent to the client and server.
        // It will be used to send data to the client and server.
        protected ModPacket GetPacket(byte packetType)
        {
            var p = ModContent.GetInstance<ModReloader>().GetPacket();
            p.Write(HandlerType);
            p.Write(packetType);
            return p;
        }
    }
}