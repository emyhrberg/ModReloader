using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Systems
{
    public class TrackingSystem : ModSystem
    {
        public record Tracking(string Name, string TooltipHoverText, Func<bool> GetValue, Action<bool> SetValue)
        {
            public void Toggle() => SetValue(!GetValue());
        }

        public static bool AreAllActive => Trackings.All(t => t.GetValue());

        public static void SetAll(bool value)
        {
            foreach (var tracking in Trackings)
                tracking.SetValue(value);
        }

        public static bool TrackEnemies = false;
        public static bool TrackTownNPCs = false;
        public static bool TrackCritters = false;

        public static List<Tracking> Trackings = new()
        {
            new("Enemies", "Track all enemies", () => TrackEnemies, v => TrackEnemies = v),
            new("Town NPCs", "Track all town NPCs", () => TrackTownNPCs, v => TrackTownNPCs = v),
            new("Critters", "Track all critters", () => TrackCritters, v => TrackCritters = v)
        };

        private const float ScreenMarginLeft = 20f;
        private const float ScreenMarginRight = 50f;
        private const float ScreenMarginTop = 20f;
        private const float ScreenMarginBottom = 20f;
        private const float LeftPanelWidth = 100f;
        private const float MaxTrackingDistance = 2000f;

        public override void PostDrawInterface(SpriteBatch sb)
        {
            RestartSB(sb);

            if (TrackEnemies) DrawTrackedEnemies(sb, HitboxSystem.HitboxColors["Enemies"]);
            if (TrackTownNPCs) DrawTrackedTownNPCs(sb, HitboxSystem.HitboxColors["Town NPCs"]);
            if (TrackCritters) DrawTrackedCritters(sb, HitboxSystem.HitboxColors["Critters"]);
        }

        private void DrawTrackedEnemies(SpriteBatch sb, Color arrowColor)
        {
            float left = ScreenMarginLeft;
            float right = Main.screenWidth - ScreenMarginRight;
            float top = ScreenMarginTop;
            float bottom = Main.screenHeight - ScreenMarginBottom;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || npc.friendly || npc.townNPC || npc.Distance(Main.LocalPlayer.Center) > MaxTrackingDistance)
                    continue;

                Vector2 screenPos = npc.Center - Main.screenPosition;
                Vector2 offsetFromPlayer = npc.Center - Main.LocalPlayer.Center;
                float rotation = offsetFromPlayer.ToRotation();

                bool withinScreen = screenPos.X >= left + LeftPanelWidth && screenPos.X <= right
                                    && screenPos.Y >= top && screenPos.Y <= bottom;
                Vector2 arrowPos = withinScreen ? screenPos : CalculateArrowPosition(screenPos, left, right, top, bottom);

                DrawArrow(sb, arrowPos, rotation, arrowColor);
                DrawNPCName(sb, npc.FullName, arrowPos);
            }
        }

        private void DrawTrackedTownNPCs(SpriteBatch sb, Color arrowColor)
        {
            float left = ScreenMarginLeft;
            float right = Main.screenWidth - ScreenMarginRight;
            float top = ScreenMarginTop;
            float bottom = Main.screenHeight - ScreenMarginBottom;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || !npc.townNPC || npc.Distance(Main.LocalPlayer.Center) > MaxTrackingDistance)
                    continue;

                Vector2 screenPos = npc.Center - Main.screenPosition;
                Vector2 offsetFromPlayer = npc.Center - Main.LocalPlayer.Center;
                float rotation = offsetFromPlayer.ToRotation();

                bool withinScreen = screenPos.X >= left + LeftPanelWidth && screenPos.X <= right
                                    && screenPos.Y >= top && screenPos.Y <= bottom;
                Vector2 arrowPos = withinScreen ? screenPos : CalculateArrowPosition(screenPos, left, right, top, bottom);

                DrawArrow(sb, arrowPos, rotation, arrowColor);
                DrawNPCName(sb, npc.FullName, arrowPos);
            }
        }

        private void DrawTrackedCritters(SpriteBatch sb, Color arrowColor)
        {
            float left = ScreenMarginLeft;
            float right = Main.screenWidth - ScreenMarginRight;
            float top = ScreenMarginTop;
            float bottom = Main.screenHeight - ScreenMarginBottom;

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || npc.catchItem <= 0 || npc.Distance(Main.LocalPlayer.Center) > MaxTrackingDistance)
                    continue;

                Vector2 screenPos = npc.Center - Main.screenPosition;
                Vector2 offsetFromPlayer = npc.Center - Main.LocalPlayer.Center;
                float rotation = offsetFromPlayer.ToRotation();

                bool withinScreen = screenPos.X >= left + LeftPanelWidth && screenPos.X <= right
                                    && screenPos.Y >= top && screenPos.Y <= bottom;
                Vector2 arrowPos = withinScreen ? screenPos : CalculateArrowPosition(screenPos, left, right, top, bottom);

                DrawArrow(sb, arrowPos, rotation, arrowColor);
                DrawNPCName(sb, npc.FullName, arrowPos);
            }
        }

        private Vector2 CalculateArrowPosition(Vector2 screenPos, float left, float right, float top, float bottom)
        {
            Vector2 arrowPos = new(
                MathHelper.Clamp(screenPos.X, left, right),
                MathHelper.Clamp(screenPos.Y, top, bottom)
            );
            if (screenPos.X < left)
                arrowPos.X = left + LeftPanelWidth;
            return arrowPos;
        }

        private void DrawArrow(SpriteBatch sb, Vector2 position, float rotation, Color color)
        {
            sb.Draw(
                texture: Ass.Arrow.Value,
                position: position,
                sourceRectangle: null,
                color: color,
                rotation: rotation,
                origin: new Vector2(Ass.Arrow.Value.Width / 2f, Ass.Arrow.Value.Height / 2f),
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: 0f
            );
        }

        private void DrawNPCName(SpriteBatch sb, string name, Vector2 arrowPos)
        {
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            Vector2 textSize = font.MeasureString(name);
            Vector2 npcNamePos = new Vector2(arrowPos.X - textSize.X / 2f, arrowPos.Y + 20f);

            Color outlineColor = Color.Black;
            Vector2[] outlineOffsets = {
                new Vector2(-1, -1),
                new Vector2(1, -1),
                new Vector2(-1, 1),
                new Vector2(1, 1)
            };
            foreach (Vector2 off in outlineOffsets)
                sb.DrawString(font, name, npcNamePos + off, outlineColor);

            sb.DrawString(font, name, npcNamePos, Color.White);
        }

        private void RestartSB(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
        }
    }
}
