using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ModHelper.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace ModHelper.PacketHandlers
{
    internal class RefreshServerPacketHandler : BasePacketHandler
    {
        public const byte ReloadMP = 1;
        public const byte PrepareMinorClients = 2;
        public const byte AnswerFromMinorClients = 3;
        public const byte RefreshMajorClient = 4;
        public const byte RefreshMinorClient = 5;
        public RefreshServerPacketHandler(byte handlerType) : base(handlerType) { }

        // Only for server:
        private int clientsLeftToSendAnswer = 0;
        private bool isRefreshMajorClientSent = false;
        private byte majorClient = 0;
        private bool shouldServerBeSaved = false;

        //Gets packets
        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case ReloadMP:
                    ReceiveReloadMP(reader, fromWho);
                    break;
                case PrepareMinorClients:
                    ReceivePrepareMinorClients(reader, fromWho);
                    break;
                case AnswerFromMinorClients:
                    ReceiveAnswerFromMinorClients(reader, fromWho);
                    break;
                case RefreshMajorClient:
                    ReceiveRefreshMajorClient(reader, fromWho);
                    break;
                case RefreshMinorClient:
                    ReceiveRefreshMinorClient(reader, fromWho);
                    break;
            }
        }

        //Major MP client:
        public void SendReloadMP(int toWho, int ignoreWho, bool shouldSevrerBeSaved)
        {
            Log.Info($"Sending ReloadMP to {toWho} from {Main.myPlayer}");

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = GetPacket(ReloadMP);
                packet.Write(shouldSevrerBeSaved);
                packet.Send(toWho, ignoreWho);
            }
        }

        //Server:
        public void ReceiveReloadMP(BinaryReader reader, int fromWho)
        {
            Log.Info($"Receiving ReloadMP to {Main.myPlayer} from {fromWho}");

            if (Main.netMode == NetmodeID.Server)
            {
                shouldServerBeSaved = reader.ReadBoolean();
                majorClient = (byte)fromWho;
                clientsLeftToSendAnswer = Main.player.Where((p) => p.active).Count() - 1;
                isRefreshMajorClientSent = false;

                SendPrepareMinorClients(-1, majorClient);
            }
        }

        //Server:
        public void SendPrepareMinorClients(int toWho, int ignoreWho)
        {
            Log.Info($"Sending PrepareMinorClients to {toWho} from {Main.myPlayer}");

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket(PrepareMinorClients);
                packet.Send(toWho, ignoreWho); // fromWho is Major MP client
            }
        }

        //Minor MP client:
        public void ReceivePrepareMinorClients(BinaryReader reader, int fromWho)
        {
            Log.Info($"Receiving PrepareMinorClients to {Main.myPlayer} from {fromWho}");

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                SendAnswerFromMinorClients(fromWho, -1);
            }
        }

        //Minor MP client:
        public void SendAnswerFromMinorClients(int toWho, int ignoreWho)
        {
            Log.Info($"Sending AnswerFromMinorClients to {toWho} from {Main.myPlayer}");
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = GetPacket(AnswerFromMinorClients);
                packet.Send(toWho, ignoreWho);
            }
        }

        //Server:
        public void ReceiveAnswerFromMinorClients(BinaryReader reader, int fromWho)
        {
            Log.Info($"Receiving AnswerFromMinorClients to {Main.myPlayer} from {fromWho}");

            if (Main.netMode == NetmodeID.Server)
            {
                clientsLeftToSendAnswer--;
                if (clientsLeftToSendAnswer <= 0 && !isRefreshMajorClientSent)
                {
                    isRefreshMajorClientSent = true;
                    SendRefreshMajorClient(majorClient, -1);
                    SendRefreshMinorClient(-1, majorClient);
                }
            }
        }

        //Server:
        public void SendRefreshMajorClient(int toWho, int ignoreWho)
        {
            Log.Info($"Sending RefreshMajorClient to {toWho} from {Main.myPlayer}");

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket(RefreshMajorClient);
                packet.Send(toWho, ignoreWho);
            }
        }

        //Major MP client:
        public void ReceiveRefreshMajorClient(BinaryReader reader, int fromWho)
        {
            Log.Info($"Receiving RefreshMajorClient to {Main.myPlayer} from {fromWho}");

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Log.Info("Reloading major MP client");
                Task.Run(ReloadUtilities.MultiPlayerMajorReload);
            }
        }

        //Server:
        public void SendRefreshMinorClient(int toWho, int ignoreWho)
        {
            Log.Info($"Sending RefreshMinorClient to {toWho} from {Main.myPlayer}");

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket(RefreshMajorClient);
                packet.Send(toWho, ignoreWho);
            }
        }

        //Minor MP client:
        public void ReceiveRefreshMinorClient(BinaryReader reader, int fromWho)
        {
            Log.Info($"Receiving RefreshMinorClient to {Main.myPlayer} from {fromWho}");

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Log.Info("Reloading minor MP client");
                Task.Run(ReloadUtilities.MultiPlayerMinorReload);
            }
        }
    }
}