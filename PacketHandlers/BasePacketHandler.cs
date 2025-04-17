using System.IO;
using Terraria.ModLoader;
namespace ModHelper.PacketHandlers
{
    // Reference:
    // https://github.com/tModLoader/tModLoader/wiki/intermediate-netcode#good-practice-managing-many-packets

    // This class acts as a base class for all packet handlers.
    // When adding new packet handlers, you should inherit from this class and implement the HandlePacket method.
    internal abstract class BasePacketHandler
    {
        // The type of the packet handler. This is used to identify the packet handler.
        internal byte HandlerType { get; set; }

        /// <summary>
        /// Method to handle the packet.
        /// </summary>
        /// <param name="reader">Reader to read the packet data.</param>
        /// <param name="fromWho">Index of the player who sent the packet.</param>
        public abstract void HandlePacket(BinaryReader reader, int fromWho);

        protected BasePacketHandler(byte handlerType)
        {
            HandlerType = handlerType;
        }

        /// <summary>
        /// Method to get the packet with the specified packet type.
        /// </summary>
        /// <param name="packetType">The type of the packet.</param>
        /// <returns>Returns the ModPacket.</returns>
        protected ModPacket GetPacket(byte packetType)
        {
            var p = ModContent.GetInstance<ModHelper>().GetPacket();
            p.Write(HandlerType);
            p.Write(packetType);
            return p;
        }
    }
}