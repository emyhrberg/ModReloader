using System;
using System.IO;
using ModHelper.Common.Systems;
using ModHelper.Common.Systems.SpawnRate;
using ModHelper.Helpers;
using ModHelper.UI.Elements;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModHelper.PacketHandlers
{
    internal class SpawnRatePacketHandler : BasePacketHandler
    {
        public const byte SpawnRatePacket = 1;

        public SpawnRatePacketHandler(byte handlerType) : base(handlerType)
        {
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case SpawnRatePacket:
                    ReceiveSpawnRate(reader, fromWho);
                    break;
                default:
                    Log.Warn("Unknown packet type: " + reader.ReadByte());
                    break;
            }
        }

        public void SendSpawnRate(float spawnRate, int fromWho)
        {
            // Create packet
            ModPacket packet = GetPacket(SpawnRatePacket);

            // Write spawn rate value
            packet.Write(spawnRate);

            Log.Info($"Client {Main.myPlayer} sent spawn rate packet: {spawnRate}");

            // Send to server
            packet.Send();
        }

        public void ReceiveSpawnRate(BinaryReader reader, int fromWho)
        {
            try
            {
                // Read spawn rate from packet
                float spawnRate = reader.ReadSingle();

                Log.Info($"Received spawn rate packet: {spawnRate} from {fromWho}");

                // Validate spawn rate (safety check)
                spawnRate = Math.Max(0f, Math.Min(30f, spawnRate));

                // Apply spawn rate change
                SpawnRateMultiplier.Multiplier = spawnRate;

                // Set the slider value in the UI if it exists
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                WorldPanel worldPanel = sys.mainState.worldPanel;
                if (worldPanel != null && worldPanel.spawnRateSlider != null)
                {
                    worldPanel.spawnRateSlider.SetValue(spawnRate);
                }

                // Kill hostile NPCs if spawn rate is 0
                if (spawnRate == 0)
                {
                    KillAllHostileNPCs();
                }

                // If server, broadcast to ALL clients including sender to ensure consistency
                if (Main.netMode == NetmodeID.Server)
                {
                    // Forward to all clients
                    ModPacket packet = GetPacket(SpawnRatePacket);
                    packet.Write(spawnRate);
                    packet.Send();

                    Log.Info($"Server broadcasting spawn rate {spawnRate} to all clients");
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error processing spawn rate packet: {ex.Message}");
            }
        }

        private static void KillAllHostileNPCs()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && !npc.townNPC && !npc.dontTakeDamage)
                {
                    npc.StrikeInstantKill();
                }
            }
        }
    }
}