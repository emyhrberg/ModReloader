using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Systems
{
    [Autoload(Side = ModSide.Client)]
    public class BossSpawnGlobalNPC : GlobalNPC
    {
        // Ensure each NPC gets its own instance of this class
        public override bool InstancePerEntity => true;

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            Config c = ModContent.GetInstance<Config>();
            if (!c.Gameplay.AlwaysSpawnBossOnTopOfPlayer)
                return;

            // Check if the spawned NPC is a boss
            if (IsBoss(npc))
            {
                // Get the player who is closest to the spawn position
                Player targetPlayer = GetClosestPlayer(npc.Center);

                if (targetPlayer != null)
                {
                    // Calculate the desired spawn position (e.g., directly above the player)
                    Vector2 spawnPosition = targetPlayer.Center - new Vector2(0, 200); // 200 pixels above the player

                    // Ensure the spawn position is within the world boundaries
                    spawnPosition = ClampPosition(spawnPosition, npc.width, npc.height);

                    // Set the NPC's position to the new spawn position
                    npc.Center = spawnPosition;

                    // Sync the NPC's new position with all clients
                    // if (Main.netMode == NetmodeID.Server)
                    // {
                    //     NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
                    // }

                    // Optional: Log the spawn modification for debugging
                    Mod.Logger.Info($"Spawned {npc.FullName} at {npc.Center}");
                }
            }
        }

        private Player GetClosestPlayer(Vector2 position)
        {
            Player closestPlayer = null;
            float closestDistance = float.MaxValue;

            foreach (Player player in Main.player)
            {
                if (player.active)
                {
                    float distance = Vector2.Distance(position, player.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPlayer = player;
                    }
                }
            }

            return closestPlayer;
        }

        private Vector2 ClampPosition(Vector2 position, int width, int height)
        {
            float clampedX = MathHelper.Clamp(position.X, width / 2, Main.maxTilesX * 16 - width / 2);
            float clampedY = MathHelper.Clamp(position.Y, height / 2, Main.maxTilesY * 16 - height / 2);
            return new Vector2(clampedX, clampedY);
        }

        private bool IsBoss(NPC npc)
        {
            return npc.boss;
        }

    }
}
