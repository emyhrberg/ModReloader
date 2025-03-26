using Terraria;
using Terraria.ModLoader;

namespace EliteTestingMod.Common.Players
{
    public class BuildAnywhere : ModPlayer
    {
        public override void Load()
        {
            // base.Load(); (This line is commented out and fine to leave as-is)

            // Hook into the player's tile and wall placement events
            // for placing tiles anywhere (not just near the player)
            On_Player.PlaceThing_Tiles += PlaceThing_Tiles;
            On_Player.PlaceThing_Walls += PlaceThing_Walls;
        }

        // NOTE: DANGER
        // There was a bug here where the
        // UI stops working because PlaceAnywhere modifies tile interaction while UI is open.
        // Fixed by setting to a high value (e.g 100) instead of max value.
        public override void UpdateEquips()
        {
            if (PlayerCheatManager.BuildAnywhere)
            {
                // Set infinite range
                int range = 100;
                Player.tileRangeX = range;
                Player.tileRangeY = range;
                Player.blockRange = range;
            }
        }

        public override void Unload()
        {
            On_Player.PlaceThing_Tiles -= PlaceThing_Tiles;
            On_Player.PlaceThing_Walls -= PlaceThing_Walls;
        }

        private void PlaceThing_Tiles(On_Player.orig_PlaceThing_Tiles orig, Player player)
        {
            if (PlayerCheatManager.BuildAnywhere && player == Main.LocalPlayer)
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
            if (PlayerCheatManager.BuildAnywhere && player == Main.LocalPlayer)
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
