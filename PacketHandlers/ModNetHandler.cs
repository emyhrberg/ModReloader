using System.IO;
using ModHelper.Helpers;
namespace ModHelper.PacketHandlers
{
    internal class ModNetHandler
    {
        // Here we define the packet types we will be using
        public const byte RefreshingServer = 1;
        internal static RefreshServerPacketHandler RefreshServer = new(RefreshingServer);

        /// <summary>
        /// Handles the incoming packets.
        /// </summary>
        /// <param name="reader">Reader to read the packet data.</param>
        /// <param name="fromWho">Index of the player who sent the packet.</param>
        public static void HandlePacket(BinaryReader reader, int fromWho)
        {
            // Here we read the packet type and call the appropriate handler
            switch (reader.ReadByte())
            {
                case RefreshingServer:
                    RefreshServer.HandlePacket(reader, fromWho);
                    break;
                default:
                    Log.Warn("Unknown packet type: " + reader.ReadByte());
                    break;
            }
        }
    }
}