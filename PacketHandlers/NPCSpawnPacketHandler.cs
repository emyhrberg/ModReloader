using System.IO;
using ModHelper.Common.Players;
using ModHelper.Helpers;
using ModHelper.UI.Elements;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModHelper.PacketHandlers
{
    internal class NPCSpawnPacketHandler(byte handlerType) : BasePacketHandler(handlerType)
    {
        // Here we define the packet types we will be using
        public const byte NPCSpawnPacket = 1;

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case NPCSpawnPacket:
                    ReceiveNPCSpawnPacket(reader, fromWho);
                    break;
                default:
                    Log.Warn("Unknown packet type: " + reader.ReadByte());
                    break;
            }
        }

        public void SendNPCSpawnPacket(int fromWho, int x, int y, int npcType)
        {
            ModPacket packet = GetPacket(packetType: NPCSpawnPacket, fromWho: Main.myPlayer);
            packet.Write(x);
            packet.Write(y);
            packet.Write(npcType);

            Log.Info("Client " + Main.myPlayer + " sent NPC spawn packet: " + x + ", " + y + ", " + npcType);

            if (Main.netMode == NetmodeID.Server)
            {
                packet.Send(-1, fromWho); // Send to all clients
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                packet.Send(fromWho); // Send to the server
            }
        }

        public void ReceiveNPCSpawnPacket(BinaryReader reader, int fromWho)
        {
            int x = reader.ReadInt32();
            int y = reader.ReadInt32();
            int npcType = reader.ReadInt32();

            if (Main.netMode == NetmodeID.Server)
            {
                // Spawn the NPC on the server
                NPC.NewNPC(new SpawnNPCEntitySource("CustomData"), x, y, npcType);
                Log.Info($"Server spawned NPC {npcType} at ({x}, {y}) for client {fromWho}");
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // Spawn the NPC on the client
                NPC.NewNPC(new SpawnNPCEntitySource("CustomData"), x, y, npcType);
                Log.Info($"Client spawned NPC {npcType} at ({x}, {y})");
            }
        }
    }
}