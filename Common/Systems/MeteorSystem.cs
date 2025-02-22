using System;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Systems
{
    public class MeteorILPatcher
    {
        // Call this from your mod’s Load method.
        public static void Apply()
        {
            IL_WorldGen.dropMeteor += DropMeteor_IL;
        }

        private static void DropMeteor_IL(ILContext il)
        {
            Log.Info("IL patch for WorldGen.dropMeteor started.");

            ILCursor c = new ILCursor(il);

            // Move the cursor to the very last return instruction.
            if (c.TryGotoNext(i => i.OpCode == OpCodes.Ret))
            {
                // Insert our delegate right before the method returns.
                c.EmitDelegate<Action>(() =>
                {
                    // At this point, WorldGen.dropMeteor has finished.
                    // Count meteor tiles (type 37) in the world up to the world surface.
                    int meteorCount = 0;
                    for (int x = 0; x < Main.maxTilesX; x++)
                    {
                        for (int y = 0; y < (int)Main.worldSurface; y++)
                        {
                            if (Main.tile[x, y] != null && Main.tile[x, y].HasTile && Main.tile[x, y].TileType == 37)
                            {
                                meteorCount++;
                            }
                        }
                    }

                    if (meteorCount == 0)
                    {
                        // Log that dropMeteor didn’t create any meteor tiles.
                        Log.Error("WorldGen.dropMeteor did not drop any meteor tiles.");

                        // Force drop a meteor at the player's position.
                        if (Main.LocalPlayer != null)
                        {
                            int playerTileX = (int)(Main.LocalPlayer.position.X / 16f);
                            int playerTileY = (int)(Main.LocalPlayer.position.Y / 16f);
                            bool forced = WorldGen.meteor(playerTileX, playerTileY, true);
                            if (forced)
                            {
                                Log.Error("Forced meteor drop succeeded at player's position (" + playerTileX + ", " + playerTileY + ").");
                            }
                            else
                            {
                                Log.Error("Forced meteor drop failed at player's position (" + playerTileX + ", " + playerTileY + ").");
                            }
                        }
                        else
                        {
                            Log.Error("No LocalPlayer available to force meteor drop.");
                        }
                    }
                    else
                    {
                        Log.Info("WorldGen.dropMeteor succeeded: " + meteorCount + " meteor tiles present.");
                    }
                });
            }
            else
            {
                Log.Error("IL patch for WorldGen.dropMeteor failed: Could not locate return instruction.");
            }
        }
    }
}
