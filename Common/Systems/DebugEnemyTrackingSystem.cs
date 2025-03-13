using System;
using System.Collections.Generic;
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
        // Use flags enum for better tracking management
        [Flags]
        public enum TrackingTypes
        {
            None = 0,
            Enemies = 1,
            Critters = 2,
            TownNPCs = 4,
            All = Enemies | Critters | TownNPCs
        }

        // Current tracking state
        private static TrackingTypes _activeTracking = TrackingTypes.None;

        // Screen bounds for tracking visuals
        private const float ScreenMarginLeft = 20f;
        private const float ScreenMarginRight = 50f;
        private const float ScreenMarginTop = 20f;
        private const float ScreenMarginBottom = 20f;
        private const float LeftPanelWidth = 100f;
        private const float MaxTrackingDistance = 2000f;

        // Track color mapping
        private static readonly Dictionary<TrackingTypes, Color> TrackingColors = new()
        {
            { TrackingTypes.Enemies, Color.Red },
            { TrackingTypes.Critters, Color.LightGreen },
            { TrackingTypes.TownNPCs, Color.Cyan },
        };

        #region Public Toggle Methods

        public static void ToggleEnemyTracking() => ToggleTracking(TrackingTypes.Enemies);

        public static void ToggleCritterTracking() => ToggleTracking(TrackingTypes.Critters);

        public static void ToggleTownNPCTracking() => ToggleTracking(TrackingTypes.TownNPCs);

        #endregion

        #region Drawing

        public override void PostDrawInterface(SpriteBatch sb)
        {
            if (_activeTracking == TrackingTypes.None)
                return;

            RestartSB(sb);
            DrawTrackedNPCs(sb);
        }

        private void DrawTrackedNPCs(SpriteBatch sb)
        {
            // Screen bounds
            float left = ScreenMarginLeft;
            float right = Main.screenWidth - ScreenMarginRight;
            float top = ScreenMarginTop;
            float bottom = Main.screenHeight - ScreenMarginBottom;

            foreach (NPC npc in Main.npc)
            {
                // Skip if NPC is not active or too far away
                if (!npc.active || npc.Distance(Main.LocalPlayer.Center) > MaxTrackingDistance)
                    continue;

                // Skip if this NPC type isn't being tracked
                if (!ShouldTrack(npc))
                    continue;

                // Get position data
                Vector2 screenPos = npc.Center - Main.screenPosition;
                Vector2 offsetFromPlayer = npc.Center - Main.LocalPlayer.Center;
                float rotation = offsetFromPlayer.ToRotation();

                // Check if within screen bounds
                bool withinScreen = screenPos.X >= left + LeftPanelWidth && screenPos.X <= right
                                   && screenPos.Y >= top && screenPos.Y <= bottom;

                // Calculate arrow position
                Vector2 arrowPos = withinScreen ? screenPos : CalculateArrowPosition(screenPos, left, right, top, bottom);

                // Draw the arrow with appropriate color
                Color arrowColor = GetArrowColor(npc);
                DrawArrow(sb, arrowPos, rotation, arrowColor);

                // Draw name
                DrawNPCName(sb, npc.FullName, arrowPos);
            }
        }

        private Vector2 CalculateArrowPosition(Vector2 screenPos, float left, float right, float top, float bottom)
        {
            Vector2 arrowPos = new(
                MathHelper.Clamp(screenPos.X, left, right),
                MathHelper.Clamp(screenPos.Y, top, bottom)
            );

            // Adjust for left panel UI elements
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

            // Outline
            Color outlineColor = Color.Black;
            Vector2[] outlineOffsets = {
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

        private void RestartSB(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);
        }

        #endregion

        #region Helper Methods

        private static void ToggleTracking(TrackingTypes trackingType)
        {
            // Toggle the flag for this tracking type
            if (_activeTracking.HasFlag(trackingType))
                _activeTracking &= ~trackingType; // Remove flag
            else
                _activeTracking |= trackingType;  // Add flag

            // Get the appropriate color for the message
            Color messageColor = _activeTracking.HasFlag(trackingType)
                ? TrackingColors[trackingType]
                : Color.Gray;

            // Display status message
            string statusText = GetStatusText(trackingType, _activeTracking.HasFlag(trackingType));
            CombatText.NewText(Main.LocalPlayer.getRect(), messageColor, statusText);
        }

        private static string GetStatusText(TrackingTypes trackingType, bool isActive)
        {
            string status = isActive ? "ON" : "OFF";
            return trackingType switch
            {
                TrackingTypes.Enemies => $"Enemy Tracking {status}",
                TrackingTypes.Critters => $"Critter Tracking {status}",
                TrackingTypes.TownNPCs => $"Town NPC Tracking {status}",
                _ => $"Tracking {status}"
            };
        }

        private bool ShouldTrack(NPC npc)
        {
            // Check if this NPC should be tracked based on active flags
            if (_activeTracking.HasFlag(TrackingTypes.Enemies) &&
                !npc.CountsAsACritter &&
                !npc.townNPC &&
                npc.CanBeChasedBy())
                return true;

            if (_activeTracking.HasFlag(TrackingTypes.Critters) &&
                npc.CountsAsACritter)
                return true;

            if (_activeTracking.HasFlag(TrackingTypes.TownNPCs) &&
                npc.townNPC)
                return true;

            return false;
        }

        private Color GetArrowColor(NPC npc)
        {
            // Determine arrow color based on NPC type
            if (npc.townNPC && _activeTracking.HasFlag(TrackingTypes.TownNPCs))
                return TrackingColors[TrackingTypes.TownNPCs];

            if (npc.CountsAsACritter && _activeTracking.HasFlag(TrackingTypes.Critters))
                return TrackingColors[TrackingTypes.Critters];

            if (!npc.CountsAsACritter && !npc.townNPC && _activeTracking.HasFlag(TrackingTypes.Enemies))
                return TrackingColors[TrackingTypes.Enemies];

            return Color.White; // Default color
        }

        #endregion
    }
}