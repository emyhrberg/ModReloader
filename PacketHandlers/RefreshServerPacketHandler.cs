using System;
using System.IO;
using System.Threading.Tasks;
using ModReloader.Helpers;
using Terraria.ID;
namespace ModReloader.PacketHandlers
{
    internal class RefreshServerPacketHandler : BasePacketHandler
    {
        public const byte ReloadMP = 1;
        public const byte RefreshMajorClient = 2;
        public const byte RefreshMinorClient = 3;
        public RefreshServerPacketHandler(byte handlerType) : base(handlerType) { }

        // Only for server:
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
                case RefreshMajorClient:
                    ReceiveRefreshMajorClient(reader, fromWho);
                    break;
                case RefreshMinorClient:
                    ReceiveRefreshMinorClient(reader, fromWho);
                    break;
            }
        }

        //Major MP client:
        public void SendReloadMP(int toWho, int ignoreWho, bool shouldSevrerBeSaved, bool onlyReload)
        {
            Log.Info($"Sending ReloadMP to {toWho} from {Main.myPlayer} " +
                $"shouldSevrerBeSaved: {shouldSevrerBeSaved}, onlyReload: {onlyReload}");

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket packet = GetPacket(ReloadMP);
                packet.Write(shouldSevrerBeSaved);
                packet.Write(onlyReload);
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
                bool onlyReload = reader.ReadBoolean();
                majorClient = (byte)fromWho;

                Netplay.SaveOnServerExit = shouldServerBeSaved;

                SendRefreshMajorClient(majorClient, -1, Environment.ProcessId, Utilities.FindWorldId());
                SendRefreshMinorClient(-1, majorClient, onlyReload);

                Netplay.Disconnect = true;
            }
        }

        //Server:
        public void SendRefreshMajorClient(int toWho, int ignoreWho, int serverPID, int serverWorldID)
        {
            Log.Info($"Sending RefreshMajorClient to {toWho} from {Main.myPlayer} " +
                $"serverPID: {serverPID}");

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket(RefreshMajorClient);
                packet.Write(serverPID);
                packet.Write(serverWorldID);
                packet.Send(toWho, ignoreWho);
            }
        }

        //Major MP client:
        public void ReceiveRefreshMajorClient(BinaryReader reader, int fromWho)
        {
            Log.Info($"Receiving RefreshMajorClient to {Main.myPlayer} from {fromWho}");
            int serverPID = reader.ReadInt32();
            int serverWorldID = reader.ReadInt32();
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Log.Info("Reloading major MP client");
                Task.Run(() => ReloadUtilities.MultiPlayerMajorReload(serverPID, serverWorldID));
            }
        }

        //Server:
        public void SendRefreshMinorClient(int toWho, int ignoreWho, bool onlyReload)
        {
            Log.Info($"Sending RefreshMinorClient to {toWho} from {Main.myPlayer}");

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket(RefreshMinorClient);
                packet.Write(onlyReload);
                packet.Send(toWho, ignoreWho);
            }
        }

        //Minor MP client:
        public void ReceiveRefreshMinorClient(BinaryReader reader, int fromWho)
        {
            Log.Info($"Receiving RefreshMinorClient to {Main.myPlayer} from {fromWho}");

            bool onlyReload = reader.ReadBoolean();

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Log.Info("Reloading minor MP client");
                Task.Run(() => ReloadUtilities.MultiPlayerMinorReload(onlyReload));
            }
        }
    }
}