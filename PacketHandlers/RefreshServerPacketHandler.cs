using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using MonoMod.RuntimeDetour;
using SquidTestingMod.Common.Players;
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
                ReloadUtilities.PrepareClient(ClientModes.MPMinor);

                await ReloadUtilities.ExitWorldOrServer();

                var hookForUnload = new Hook(typeof(ModLoader).GetMethod(
                        "Unload",
                        BindingFlags.NonPublic | BindingFlags.Static
                    ), (Func<bool> orig) =>
                    {
                        bool o = orig();
                        LogManager.GetLogger("SQUID").Info($"Hi! I am {Process.GetCurrentProcess().Id} procces id!");
                        object loadMods = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.Interface").GetField("loadMods", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
                        typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.UILoadMods").GetMethod("SetProgressText", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(loadMods, ["Waiting for main client", "Waiting for main client"]);

                        
                        using var pipeClient = new NamedPipeClientStream(".", ReloadUtilities.pipeName, PipeDirection.InOut);
                        pipeClient.Connect();

                        using var reader = new StreamReader(pipeClient);
                        using var writer = new StreamWriter(pipeClient) { AutoFlush = true };

                        writer.WriteLine("Im here and ready to reload");

                        string? response = reader.ReadLine();
                        return o;
                    });

                //stops GC from deleting it
                GC.SuppressFinalize(hookForUnload);

                ReloadUtilities.ReloadMod();
            }
        }
    }
}