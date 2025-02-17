using SquidTestingMod.Helpers;
using SquidTestingMod.UI;
using System.IO;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace SquidTestingMod.PacketHandlers
{
    internal class RefreshServerPacketHandler : PacketHandler
    {
        public const byte KillingServer = 1;
        public const byte RefreshClients = 2;
        public RefreshServerPacketHandler(byte handlerType) : base(handlerType)
        {
        }
        //Gets packets
        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case (KillingServer):
                    ReceiveKillingServer(reader, fromWho);
                    break;
                case (RefreshClients):
                    ReceiveRefreshClients(reader, fromWho);
                    break;
            }
        }
        public void SendKillingServer(int toWho, int fromWho)
        {
            Log.Info($"Sending SendKillingServer to {toWho} from {fromWho}");
            ModPacket packet = GetPacket(KillingServer, fromWho);
            packet.Send(toWho, fromWho);
        }
        public void SendRefreshClients(int toWho, int fromWho)
        {
            Log.Info($"Sending SendRefreshClients to {toWho} from {fromWho}");
            ModPacket packet = GetPacket(RefreshClients, fromWho);
            packet.Send(toWho, fromWho);
        }
        public void ReceiveKillingServer(BinaryReader reader, int fromWho)
        {
            Log.Info($"Receiving ReceiveKillingServer to {Main.myPlayer} from {fromWho}");
            if (Main.netMode == NetmodeID.Server)
            {
                //Refreshes even the one that invoked KillingServer client, so thats why second argument is -1 instead of fromWho
                //Or else it would ignore fromWho client
                //When RefreshClients would be replaced with KillingClients it should be back to fromWho :)
                SendRefreshClients(-1, -1);
                //delay??
                Log.Info("Attempting to close the server");
                Netplay.Disconnect = true;
            }
        }
        public void ReceiveRefreshClients(BinaryReader reader, int fromWho)
        {
            Log.Info($"Receiving ReceiveRefreshClients to {Main.myPlayer} from {fromWho}");
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                //Task.Run(RefreshButton.RefreshClient);
            }
        }
    }
}