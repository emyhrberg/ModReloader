using System.IO;
using ModHelper.Helpers;
namespace ModHelper.PacketHandlers
{
    internal class ModNetHandler
    {
        // Here we define the packet types we will be using
        public const byte RefreshingServer = 1;
        internal static RefreshServerPacketHandler RefreshServer = new RefreshServerPacketHandler(RefreshingServer);

        public const byte GodMode = 2;
        internal static GodPacketHandler GodModePacketHandler = new GodPacketHandler(GodMode);

        public static void HandlePacket(BinaryReader r, int fromWho)
        {
            // Here we read the packet type and call the appropriate handler
            switch (r.ReadByte())
            {
                case RefreshingServer:
                    RefreshServer.HandlePacket(r, fromWho);
                    break;
                case GodMode:
                    GodModePacketHandler.HandlePacket(r, fromWho);
                    break;
                default:
                    Log.Warn("Unknown packet type: " + r.ReadByte());
                    break;
            }
        }
    }
}