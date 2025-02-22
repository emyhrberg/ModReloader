using System.IO;
using System.Threading.Tasks;
using SquidTestingMod.Helpers;
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
        public void SendKillingServer(int toWho, int fromWho, bool shouldSevrerBeSaved)
        {
            Log.Info($"Sending SendKillingServer to {toWho} from {fromWho}");
            ModPacket packet = GetPacket(KillingServer, fromWho);
            packet.Write(shouldSevrerBeSaved);
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

            bool shouldServerBeSaved = reader.ReadBoolean();

            if (Main.netMode == NetmodeID.Server)
            {
                SendRefreshClients(-1, fromWho);
                Netplay.SaveOnServerExit = shouldServerBeSaved;
                //delay??
                Log.Info("Attempting to close the server");
                Netplay.Disconnect = true;
            }
        }
        public void ReceiveRefreshClients(BinaryReader reader, int fromWho)
        {
            Log.Info($"Receiving ReceiveRefreshClients to {Main.myPlayer} from {fromWho}");
            Task.Run(ReceiveRefreshClientsAsync);
        }

        private async static void ReceiveRefreshClientsAsync()
        {

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ReloadUtilities.PrepareClient(ClientMode.MPMain);

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    await ReloadUtilities.ExitWorldOrServer();
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    await ReloadUtilities.ExitAndKillServer();
                }

                await ReloadUtilities.BuildAndReloadMod();
            }
        }
    }
}