using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SquidTestingMod.Networking
{
    /// <summary>
    /// Manages and dispatches packets to the appropriate PacketHandler.
    /// </summary>
    internal static class PacketManager
    {
        // Register your handlers here.
        internal static GodGlowPacketHandler GodGlowHandler = new GodGlowPacketHandler(PacketHandler.PacketHandlerTypes.GodGlow);

        // This method is called from the mod's HandlePacket override.
        public static void HandlePacket(BinaryReader reader, int fromWho)
        {
            // The first byte is the handler type.
            byte handlerType = reader.ReadByte();
            switch (handlerType)
            {
                case PacketHandler.PacketHandlerTypes.GodGlow:
                    GodGlowHandler.HandlePacket(reader, fromWho);
                    break;
                default:
                    ModContent.GetInstance<SquidTestingMod>().Logger.Warn($"Unknown packet handler type: {handlerType}");
                    break;
            }
        }
    }
}
