using System.IO;
using ModReloader.Helpers;

namespace ModReloader.PacketHandlers
{
    internal class ModNetHandler
    {
        // Here we define the packet types we will be using
        public const byte RefreshingServer = 1;
        internal static RefreshServerPacketHandler RefreshServer = new(RefreshingServer);

        public static void HandlePacket(BinaryReader r, int fromWho)
        {
            // Here we read the packet type and call the appropriate handler
            switch (r.ReadByte())
            {
                case RefreshingServer:
                    RefreshServer.HandlePacket(r, fromWho);
                    break;
                default:
                    Log.Warn("Unknown packet type: " + r.ReadByte());
                    break;
            }
        }
    }
}