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
        public const byte GodMode = 1;

        public GodPacketHandler(byte handlerType) : base(handlerType)
        {
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case GodMode:
                    ReceiveGodMode(reader, fromWho);
                    break;
                default:
                    Log.Warn("Unknown packet type: " + reader.ReadByte());
                    break;
            }
        }

        public void SendGodMode(int toWho, int fromWho, bool godMode)
        {
            Log.Info($"Sending SendGodMode to {toWho} from {fromWho}");
            ModPacket packet = GetPacket(GodMode, fromWho);
            packet.Write(godMode);
            packet.Write(fromWho);
            packet.Send(toWho, fromWho);
        }

        public void ReceiveGodMode(BinaryReader reader, int fromWho)
        {
            // Multiplayer client receiving god mode packet from server
            Log.Info($"Client receiving ReceiveGodMode to {Main.myPlayer} from {fromWho}");
            bool godMode = reader.ReadBoolean();
            int targetPlayerId = reader.ReadInt32();

            // Set the god mode for the right player
            Player targetPlayer = Main.player[targetPlayerId];
            PlayerCheatManager playerCheatManager = targetPlayer.GetModPlayer<PlayerCheatManager>();

            playerCheatManager.GetCheats().Find(c => c.Name == "God").SetValue(godMode);
            Log.Info("Set god mode for client " + Main.LocalPlayer.whoAmI + " to " + godMode);

            if (Main.netMode == NetmodeID.Server)
            {
                Log.Info("Server: Sending god mode to client " + fromWho);
                SendGodMode(-1, fromWho, godMode);
            }
        }
    }
}