using System.IO;
using ModHelper.Common.Players;
using ModHelper.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModHelper.PacketHandlers
{
    internal class GodPacketHandler : BasePacketHandler
    {
        // Here we define the packet types we will be using
        public const byte GodModePacket = 1;

        public GodPacketHandler(byte handlerType) : base(handlerType)
        {
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case GodModePacket:
                    ReceiveGodMode(reader, fromWho);
                    break;
                default:
                    Log.Warn("Unknown packet type: " + reader.ReadByte());
                    break;
            }
        }

        public void SendGodMode(int toWho, int targetPlayerId, bool godMode)
        {
            ModPacket packet = GetPacket(GodModePacket);
            packet.Write(godMode);         // The god mode state
            packet.Write(targetPlayerId);  // The player who should be affected
            packet.Send(toWho);           // Where to send the packet (-1 for all, specific number for specific client)
        }

        public void ReceiveGodMode(BinaryReader reader, int fromWho)
        {
            // Read data from packet
            bool godMode = reader.ReadBoolean();
            int targetPlayerId = reader.ReadInt32();

            // Apply the change to the target player
            Player targetPlayer = Main.player[targetPlayerId];
            PlayerCheatManager playerCheatManager = targetPlayer.GetModPlayer<PlayerCheatManager>();
            playerCheatManager.GetCheats().Find(c => c.Name == "God").SetValue(godMode);

            // If we're the server, broadcast to all clients
            if (Main.netMode == NetmodeID.Server)
            {
                SendGodMode(-1, targetPlayerId, godMode);
            }
        }
    }
}