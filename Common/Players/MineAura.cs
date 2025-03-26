using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EliteTestingMod.Common.Players
{
    public class MineAura : ModPlayer
    {
        // Change this to control how far from the player tiles/walls are mined.
        // Example: 3 or 10.
        public static int mineRange = 3;

        public override void PostUpdate()
        {
            // Only run in singleplayer
            if (Main.netMode != NetmodeID.SinglePlayer)
                return;

            // If the cheat is not active, exit
            if (!PlayerCheatManager.MineAura)
                return;

            // Convert the player's position to tile coordinates.
            Point tileCoords = Player.position.ToTileCoordinates();
            int tileX = tileCoords.X;
            int tileY = tileCoords.Y;

            // If the player presses LEFT or RIGHT, mine an area in front of them
            if (Player.controlLeft || Player.controlRight)
            {
                // Start from 1 because 0 is the player's tile
                // We go up to mineRange
                for (int offsetX = 1; offsetX <= mineRange; offsetX++)
                {
                    // We'll also mine vertically from -mineRange..mineRange
                    for (int offsetY = -mineRange; offsetY <= mineRange; offsetY++)
                    {
                        int x = tileX + offsetX * Player.direction;
                        int y = tileY + offsetY;
                        KillTileAndWall(x, y);
                    }
                }
            }

            // If the player presses UP
            if (Player.controlUp)
            {
                // We'll mine an area above the player
                for (int offsetX = -mineRange; offsetX <= mineRange; offsetX++)
                {
                    for (int offsetY = 1; offsetY <= mineRange; offsetY++)
                    {
                        int x = tileX + offsetX;
                        int y = tileY - offsetY;
                        KillTileAndWall(x, y);
                    }
                }
            }

            // If the player presses DOWN
            if (Player.controlDown)
            {
                // We'll mine an area below the player
                for (int offsetX = -mineRange; offsetX <= mineRange; offsetX++)
                {
                    for (int offsetY = 1; offsetY <= mineRange; offsetY++)
                    {
                        int x = tileX + offsetX;
                        int y = tileY + offsetY;
                        KillTileAndWall(x, y);
                    }
                }
            }
        }

        /// <summary>
        /// Helper that kills both the tile and the wall at (x, y).
        /// </summary>
        private void KillTileAndWall(int x, int y)
        {
            // Kill the tile
            WorldGen.KillTile(x, y, fail: false, effectOnly: false, noItem: false);
            // Also kill the wall
            WorldGen.KillWall(x, y);
        }
    }
}
