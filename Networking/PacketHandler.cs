using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SquidTestingMod.Networking
{
    /// <summary>
    /// Abstract base class for packet handlers. Automatically writes the handler type and packet type.
    /// </summary>
    internal abstract class PacketHandler
    {
        public static class PacketHandlerTypes
        {
            public const byte GodGlow = 1;
            // Add other handler type constants here...
        }

        internal byte HandlerType { get; }

        protected PacketHandler(byte handlerType)
        {
            HandlerType = handlerType;
        }

        // Creates a packet with the handler type and packet type written to it.
        protected ModPacket GetPacket(byte packetType, int fromWho)
        {
            ModPacket packet = ModContent.GetInstance<SquidTestingMod>().GetPacket();
            packet.Write(HandlerType);
            packet.Write(packetType);
            // If on the server, also write the sender's id.
            if (Main.netMode == NetmodeID.Server)
                packet.Write((byte)fromWho);
            return packet;
        }

        public abstract void HandlePacket(BinaryReader reader, int fromWho);
    }
}