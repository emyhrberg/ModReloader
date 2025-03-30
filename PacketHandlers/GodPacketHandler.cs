using System.IO;
using ModHelper.Common.Players;
using ModHelper.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModHelper.PacketHandlers
{
    internal class GodPacketHandler : PacketHandler
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
            Log.Info($"Sending godMode={godMode} to player={toWho} for playerID={targetPlayerId}");
            ModPacket packet = GetPacket(GodModePacket, targetPlayerId);
            packet.Write(godMode);         // The god mode state
            packet.Write(targetPlayerId);  // The player who should be affected
            packet.Send(toWho);           // Where to send the packet (-1 for all, specific number for specific client)
        }

        public void ReceiveGodMode(BinaryReader reader, int fromWho)
        {
            // Read data from packet
            bool godMode = reader.ReadBoolean();
            int targetPlayerId = reader.ReadInt32();

            Log.Info($"Received god mode packet: godMode={godMode}, targetPlayerID={targetPlayerId}");

            // Apply the change to the target player
            Player targetPlayer = Main.player[targetPlayerId];
            PlayerCheatManager playerCheatManager = targetPlayer.GetModPlayer<PlayerCheatManager>();
            playerCheatManager.GetCheats().Find(c => c.Name == "God").SetValue(godMode);

            Log.Info($"Set god mode for player {targetPlayer.name} (ID:{targetPlayerId}) to {godMode}");

            // If we're the server, broadcast to all clients
            if (Main.netMode == NetmodeID.Server)
            {
                Log.Info($"Server: Broadcasting god mode status for player {targetPlayerId} to all clients");
                SendGodMode(-1, targetPlayerId, godMode); // CORRECT - use original targetPlayerId
            }
        }
    }
}