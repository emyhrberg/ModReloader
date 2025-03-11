using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace SquidTestingMod.PacketHandlers
{
    internal class ModNetHandler
    {
        public const byte RefreshingServer = 1;
        internal static RefreshServerPacketHandler RefreshServer = new RefreshServerPacketHandler(RefreshingServer);
        public static void HandlePacket(BinaryReader r, int fromWho)
        {
            switch (r.ReadByte())
            {
                case RefreshingServer:
                    RefreshServer.HandlePacket(r, fromWho);
                    break;
            }
        }
    }
}