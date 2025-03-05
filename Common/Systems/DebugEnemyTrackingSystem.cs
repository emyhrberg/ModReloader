using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Systems
{
    public class DebugEnemyTrackingSystem : ModSystem
    {
        private static bool IsTracking = false;

        public static void ToggleTracking()
        {
            IsTracking = !IsTracking;
            CombatText.NewText(Main.LocalPlayer.getRect(), IsTracking ? Color.Green : Color.Red, IsTracking ? "Tracking ON" : "Tracking OFF");
        }

        private void RestartSB(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
        }

        public override void PostDrawInterface(SpriteBatch sb)
        {
            RestartSB(sb);

            if (!IsTracking) return;

            // Hardcode screen bounds
            float left = 20f;
            float right = Main.screenWidth - 50f;
            float top = 20f;
            float bottom = Main.screenHeight - 20f;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || !npc.CanBeChasedBy() || npc.Distance(Main.LocalPlayer.Center) > 2000f)
                    continue;

                // Calculate positions and angles
                Vector2 screenPos = npc.Center - Main.screenPosition;
                Vector2 offsetFromPlayer = npc.Center - Main.LocalPlayer.Center;
                float distance = offsetFromPlayer.Length(); // how far NPC is from the player
                float rotation = offsetFromPlayer.ToRotation();

                // Check if the NPC is within the screen bounds
                bool withinScreen = screenPos.X >= left + 100f && screenPos.X <= right
                                    && screenPos.Y >= top && screenPos.Y <= bottom;

                // Decide arrowPos
                Vector2 arrowPos = screenPos;
                if (!withinScreen)
                {
                    arrowPos = new Vector2(
                        MathHelper.Clamp(screenPos.X, left, right),
                        MathHelper.Clamp(screenPos.Y, top, bottom)
                    );

                    if (screenPos.X < left)
                    {
                        arrowPos.X = left + 100f;
                        Log.Info("forced arrowpos.X to " + arrowPos.X);
                    }
                }

                // Log debugging info
                Log.Slow($"[DebugEnemyTrackingSystem] NPC: {npc.FullName}, Dist: {distance:F2}, withinScreen: {withinScreen}, " +
                 $"screenPos: {screenPos}, arrowPos: {arrowPos}, leftBound: {left}, rightBound: {right}, rotation: {MathHelper.ToDegrees(rotation):F2}");

                // Draw the arrow
                sb.Draw(
                    texture: Assets.Arrow.Value,
                    position: arrowPos,
                    sourceRectangle: null,
                    color: Color.White,
                    rotation: rotation,
                    origin: new Vector2(Assets.Arrow.Value.Width / 2f, Assets.Arrow.Value.Height / 2f),
                    scale: 1f,
                    effects: SpriteEffects.None,
                    layerDepth: 0f
                );

                // Draw name below arrow
                DrawNPCName(sb, npc.FullName, arrowPos);
            }
        }

        private void DrawNPCName(SpriteBatch sb, string name, Vector2 arrowPos)
        {
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            Vector2 textSize = font.MeasureString(name);
            Vector2 npcNamePos = new Vector2(arrowPos.X - textSize.X / 2f, arrowPos.Y + 20f);

            // Outline
            Color outlineColor = Color.Black;
            Vector2[] outlineOffsets =
            {
                new Vector2(-1, -1),
                new Vector2(1, -1),
                new Vector2(-1, 1),
                new Vector2(1, 1)
            };
            foreach (Vector2 off in outlineOffsets)
                sb.DrawString(font, name, npcNamePos + off, outlineColor);

            // Main text
            sb.DrawString(font, name, npcNamePos, Color.White);
        }
    }
}