using System;
using System.IO;
using System.Threading.Tasks;
using ModHelper.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace ModHelper.PacketHandlers
{
    internal class PlayersInLocalHostPacketHandler : BasePacketHandler
    {
        public const byte AnnouncePlayers = 1;
        public const byte RequestPlayers = 2;
        public PlayersInLocalHostPacketHandler(byte handlerType) : base(handlerType) { }

        // Only for server:
        // private byte majorClient = 0;
        // private bool shouldServerBeSaved = false;

        //Gets packets
        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case AnnouncePlayers:
                    ReceiveAnnouncePlayers(reader, fromWho);
                    break;
                case RequestPlayers:
                    ReceiveRequestPlayers(reader, fromWho);
                    break;
            }
        }


        public void ReceiveAnnouncePlayers(BinaryReader reader, int fromWho)
        {
            Log.Info($"Receiving AnnouncePlayers to {Main.myPlayer} from {fromWho}");

            if (Main.netMode == NetmodeID.Server)
            {
                // Handle the packet here
                // For example, you can send a response back to the client
                int playerCount = reader.ReadInt32();
                Log.Info($"Server received player count: {playerCount} from {fromWho}");
            }
        }

        public void ReceiveRequestPlayers(BinaryReader reader, int fromWho)
        {
            Log.Info($"Receiving RequestPlayers to {Main.myPlayer} from {fromWho}");

            if (Main.netMode == NetmodeID.Server)
            {
                // Handle the packet here
                // For example, you can send a response back to the client
                SendPlayersInLocalHost(fromWho);
            }
        }

        public void SendPlayersInLocalHost(int toWho)
        {
            Log.Info($"Sending PlayersInLocalHost to {toWho} from {Main.myPlayer}");

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = GetPacket(AnnouncePlayers);
                // Here you can write the data you want to send
                // For example, you can write the number of players in the local host
                packet.Write(Main.player.Length);
                packet.Send(toWho);
                Log.Info($"Sent PlayersInLocalHost: {Main.player.Length} to {toWho} from {Main.myPlayer}");
            }
        }
    }
}