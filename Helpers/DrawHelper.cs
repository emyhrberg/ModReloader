using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ModHelper.Helpers
{
    public static class DrawHelper
    {
        /// <summary>
        /// Draws a tooltip panel just above a BaseButton element.
        /// </summary>
        public static void DrawMainMenuTooltipPanel(this UIElement element, string text, string tooltip, int yOffset = 0)
        {
            int pad = 6;
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            float nameWidth = ChatManager.GetStringSize(font, text, Vector2.One).X;
            float tipWidth = ChatManager.GetStringSize(font, tooltip, Vector2.One).X * 0.9f;
            float width = Math.Max(nameWidth, tipWidth) + pad * 4;
            float height = ChatManager.GetStringSize(font, $"{text}\n", Vector2.One).Y + pad * 2 - 4;

            if (!string.IsNullOrEmpty(tooltip))
            {
                height = ChatManager.GetStringSize(font, $"{text}\n{tooltip}", Vector2.One).Y + pad * 2 - 4;
            }

            CalculatedStyle dims = element.GetDimensions();

            // Move Y position up by decreasing the Y value of the dimensions
            dims.Y -= 80f;
            dims.Y -= yOffset;

            float tooltipX = dims.X + (dims.Width - width) / 2f;
            float tooltipY = dims.Y - height;
            Rectangle rect = new((int)tooltipX, (int)tooltipY, (int)width, (int)height);

            // draw bg panel
            Color darkBlue = new Color(22, 22, 55) * 0.925f;
            Utils.DrawInvBG(Main.spriteBatch, rect, darkBlue);

            // Calculate center X of panel
            float centerX = tooltipX + width / 2f;

            // draw main header text (centered)
            float headerTextWidth = ChatManager.GetStringSize(font, text, Vector2.One).X;
            Vector2 headerPos = new Vector2(centerX - headerTextWidth / 2f, tooltipY + pad);
            Utils.DrawBorderString(Main.spriteBatch, text, headerPos, Color.White);

            // draw tooltip description text (centered)
            // NOTE: not really centered.
            if (!string.IsNullOrEmpty(tooltip))
            {
                float tooltipTextWidth = ChatManager.GetStringSize(font, tooltip, Vector2.One).X * 0.9f;
                Vector2 tooltipPos = new Vector2(centerX - tooltipTextWidth / 2f, headerPos.Y + ChatManager.GetStringSize(font, text, Vector2.One).Y + 4);
                Utils.DrawBorderString(Main.spriteBatch, tooltip, tooltipPos, Color.LightGray, 0.9f);
            }
        }

        /// <summary>
        /// Draws a tooltip panel just above a BaseButton element.
        /// </summary>
        public static void DrawTooltipPanel(this UIElement element, string text, string tooltip)
        {
            if (element == null || string.IsNullOrWhiteSpace(text))
                return;

            int pad = 6;
            DynamicSpriteFont font = FontAssets.MouseText.Value;
            // Get string sizes
            float nameWidth = ChatManager.GetStringSize(font, text, Vector2.One).X;
            float tipWidth = !string.IsNullOrEmpty(tooltip)
                ? ChatManager.GetStringSize(font, tooltip, Vector2.One).X * 0.9f
                : 0f;
            float width = Math.Max(nameWidth, tipWidth) + pad * 4;
            float height = ChatManager.GetStringSize(font, $"{text}\n", Vector2.One).Y + pad * 2 - 4;
            if (!string.IsNullOrEmpty(tooltip))
            {
                height = ChatManager.GetStringSize(font, $"{text}\n{tooltip}", Vector2.One).Y + pad * 2 - 4;
            }

            CalculatedStyle dims = element.GetDimensions();

            // Calculate centered tooltip position above element
            float tooltipX = dims.X + (dims.Width - width) / 2f;
            float tooltipY = dims.Y - height;
            // Clamp tooltip position so it never goes offscreen
            tooltipX = Math.Max(tooltipX, 0);
            tooltipY = Math.Max(tooltipY, 0);

            Rectangle rect = new Rectangle((int)tooltipX, (int)tooltipY, (int)width, (int)height);

            // Draw background panel
            Color darkBlue = new Color(22, 22, 55) * 0.925f;
            Utils.DrawInvBG(Main.spriteBatch, rect, darkBlue);

            // Center text
            float centerX = tooltipX + width / 2f;
            float headerTextWidth = ChatManager.GetStringSize(font, text, Vector2.One).X;
            Vector2 headerPos = new Vector2(centerX - headerTextWidth / 2f, tooltipY + pad);
            Utils.DrawBorderString(Main.spriteBatch, text, headerPos, Color.White);

            // Draw description text if provided
            if (!string.IsNullOrEmpty(tooltip))
            {
                float tooltipTextWidth = ChatManager.GetStringSize(font, tooltip, Vector2.One).X * 0.9f;
                Vector2 tooltipPos = new Vector2(centerX - tooltipTextWidth / 2f,
                    headerPos.Y + ChatManager.GetStringSize(font, text, Vector2.One).Y + 4);
                Utils.DrawBorderString(Main.spriteBatch, tooltip, tooltipPos, Color.LightGray, 0.9f);
            }
        }

        /// <summary>
        /// Draws a texture at the proper scale to fit within the given UI element.
        /// /// </summary>
        public static void DrawProperScale(SpriteBatch spriteBatch, UIElement element, Texture2D tex, float scale = 1.0f, float opacity = 1.0f, bool active = false)
        {
            if (tex == null || element == null)
            {
                Log.SlowInfo("Failed to find texture to draw. Skipping draw.", seconds: 5);
            }

            // Get the UI element's dimensions
            CalculatedStyle dims = element.GetDimensions();

            // Compute a scale that makes it fit within the UI element
            float scaleX = dims.Width / tex.Width;
            float scaleY = dims.Height / tex.Height;
            float drawScale = Math.Min(scaleX, scaleY) * scale;

            // Top-left anchor: just place it at dims.X, dims.Y
            Vector2 drawPosition = new Vector2(dims.X, dims.Y);

            float actualOpacity = opacity;
            if (active)
            {
                actualOpacity = 1f;
            }

            // Draw the texture anchored at top-left with the chosen scale
            spriteBatch.Draw(
                tex,
                drawPosition,
                null,
                Color.White * actualOpacity,
                0f,
                Vector2.Zero,
                drawScale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
