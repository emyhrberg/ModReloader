using System.IO;
using SquidTestingMod.Common.Players;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace SquidTestingMod.Networking
{
    /// <summary>
    /// Handles GodGlow packets. This handler sends the god glow state (a boolean) along with the player index.
    /// The server forwards the packet to all clients so that everyone updates that player's god glow state.
    /// </summary>
    internal class GodGlowPacketHandler : PacketHandler
    {
        public const byte SyncGodGlowState = 1;

        public GodGlowPacketHandler(byte handlerType) : base(handlerType) { }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            byte packetId = reader.ReadByte();
            Log.Info($"Handling packet with ID: {packetId} from player: {fromWho}");
            switch (packetId)
            {
                case SyncGodGlowState:
                    ReceiveGodGlowState(reader, fromWho);
                    break;
                default:
                    Log.Warn($"Unknown GodGlow packet type: {packetId}");
                    break;
            }
        }

        /// <summary>
        /// Sends the god glow state for the given player.
        /// </summary>
        /// <param name="toWho">Destination (-1 broadcasts to all)</param>
        /// <param name="fromWho">Sender's player index</param>
        /// <param name="playerIndex">The index of the player whose state is changing</param>
        /// <param name="godGlowState">True if god glow is enabled</param>
        public void SendGodGlowState(int toWho, int fromWho, int playerIndex, bool godGlowState)
        {
            Log.Info($"Sending GodGlowState: PlayerIndex={playerIndex}, GodGlowState={godGlowState}, ToWho={toWho}, FromWho={fromWho}");
            ModPacket packet = GetPacket(SyncGodGlowState, fromWho);
            packet.Write(playerIndex);
            packet.Write(godGlowState);
            packet.Send(toWho, fromWho);
        }

        private void ReceiveGodGlowState(BinaryReader reader, int fromWho)
        {
            int playerIndex = reader.ReadInt32();
            bool godGlowState = reader.ReadBoolean();
            Log.Info($"Received GodGlowState: PlayerIndex={playerIndex}, GodGlowState={godGlowState}, FromWho={fromWho}");

            if (Main.netMode == NetmodeID.Server)
            {
                // On the server, forward the packet to all clients except the sender.
                Log.Info($"Forwarding GodGlowState to all clients except sender: {fromWho}");
                SendGodGlowState(-1, fromWho, playerIndex, godGlowState);
            }
            else
            {
                // On the client, update the mod player's god glow state.
                Log.Info($"Updating GodGlowState on client for PlayerIndex={playerIndex}");
                God modPlayer = Main.player[playerIndex].GetModPlayer<God>();
                modPlayer.GodGlow = godGlowState;
            }
        }
    }
}
