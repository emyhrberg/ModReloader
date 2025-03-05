using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Players
{
    public class PlaceAnywhere : ModPlayer
    {
        public override void Load()
        {
            // base.Load(); (This line is commented out and fine to leave as-is)

            // Hook into the player's tile and wall placement events
            // for placing tiles anywhere (not just near the player)
            On_Player.PlaceThing_Tiles += PlaceThing_Tiles;
            On_Player.PlaceThing_Walls += PlaceThing_Walls;
        }

        public override void UpdateEquips()
        {
            if (PlayerCheatManager.PlaceAnywhere)
            {
                // Set infinite range
                Player.tileRangeX = int.MaxValue;
                Player.tileRangeY = int.MaxValue;
                Player.blockRange = int.MaxValue;
            }
        }

        public override void Unload()
        {
            On_Player.PlaceThing_Tiles -= PlaceThing_Tiles;
            On_Player.PlaceThing_Walls -= PlaceThing_Walls;
        }

        private void PlaceThing_Tiles(On_Player.orig_PlaceThing_Tiles orig, Player player)
        {
            // Only apply when build mode is on
            if (PlayerCheatManager.PlaceAnywhere && player == Main.LocalPlayer)
            {
                Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
                ushort wallType = tile.WallType;
                tile.WallType = 4; // Temporarily set wall to wood to allow placement
                orig(player);
                tile.WallType = wallType; // Restore original wall
            }
            else
            {
                orig(player);
            }
        }

        private void PlaceThing_Walls(On_Player.orig_PlaceThing_Walls orig, Player player)
        {
            // Only apply when build mode is on
            if (PlayerCheatManager.PlaceAnywhere && player == Main.LocalPlayer)
            {
                Tile tile = Framing.GetTileSafely(Player.tileTargetX - 1, Player.tileTargetY);
                bool hasTile = tile.HasTile;
                tile.HasTile = true; // Temporarily set tile to true to allow wall placement
                orig(player);
                tile.HasTile = hasTile; // Restore original tile state
            }
            else
            {
                orig(player);
            }
        }
    }
}
