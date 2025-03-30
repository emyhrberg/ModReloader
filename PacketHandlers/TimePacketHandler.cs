using System;
using System.IO;
using ModHelper.Common.Players;
using ModHelper.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModHelper.PacketHandlers
{
    internal class TimePacketHandler : BasePacketHandler
    {
        // Here we define the packet types we will be using
        public const byte TimePacket = 1;

        public TimePacketHandler(byte handlerType) : base(handlerType)
        {
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case TimePacket:
                    ReceiveTime(reader, fromWho);
                    break;
                default:
                    Log.Warn("Unknown packet type: " + reader.ReadByte());
                    break;
            }
        }

        public void SendTime(bool dayTime, double time, int fromWho)
        {
            // Create a new packet - GetPacket already writes the handler type and packet type
            ModPacket packet = GetPacket(packetType: TimePacket, fromWho: fromWho);

            // Validate time value before sending
            time = Math.Max(0, Math.Min(time, dayTime ? 54000.0 : 32400.0));

            // Write the time data
            packet.Write(dayTime);
            packet.Write(time);

            Log.Info($"Client {Main.myPlayer} sent time packet: {dayTime}, {time}");

            // Send the packet to the server (or all clients if we are the server)
            if (Main.netMode == NetmodeID.Server)
            {
                packet.Send(-1, fromWho); // Send to all clients
            }
            else
            {
                packet.Send(fromWho); // Send to the server
            }
        }

        public void ReceiveTime(BinaryReader reader, int fromWho)
        {
            // Read data from packet
            bool dayTime = reader.ReadBoolean();
            double time = reader.ReadDouble();

            // Apply the new time to the world
            Main.dayTime = dayTime;
            Main.time = time;

            Log.Info($"Received time packet: {dayTime}, {time} from {fromWho}");

            // If we're the server, broadcast to all clients
            if (Main.netMode == NetmodeID.Server)
            {
                Log.Info($"Server received time packet: {dayTime}, {time} from {fromWho}");
                NetMessage.SendData(MessageID.WorldData);
            }
        }
    }
}