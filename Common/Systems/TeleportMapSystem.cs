using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Systems
{
    public class TeleportMapSystem : ModSystem
    {
        private bool mouseRightLastUpdate;

        public override void PostDrawFullscreenMap(ref string mouseText)
        {
            base.PostDrawFullscreenMap(ref mouseText);

            // draw text
            string teleportText = "Right click to teleport";
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(teleportText);
            Vector2 position = new(10f, Main.screenHeight - textSize.Y - 10f);
            Utils.DrawBorderString(Main.spriteBatch, teleportText, position, Color.White);

            if (!Main.mouseRight && this.mouseRightLastUpdate)
            {
                Player player = Main.LocalPlayer;
                Vector2 mapCentrePos = Main.mapFullscreenPos;
                Vector2 targetTile = (new Vector2(Main.mouseX, Main.mouseY) - new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f)) / Main.mapFullscreenScale + mapCentrePos;
                Vector2 targetPos = targetTile * 16f;
                targetPos.Y -= player.height;
                int worldWidth = Main.maxTilesX * 16;
                int worldHeight = Main.maxTilesY * 16;
                targetPos.X = Math.Clamp(targetPos.X, 0f, worldWidth - player.width);
                targetPos.Y = Math.Clamp(targetPos.Y, 0f, worldHeight - player.height);
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    player.Teleport(targetPos, 0, 0);
                    // close map
                    Main.mapFullscreen = false;
                }
            }
            mouseRightLastUpdate = Main.mouseRight;
        }
    }
}