using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ModHelper.Common.Systems
{
    public class MainMenuButtonRightSide : UIText
    {
        // Width and height of the text
        private float w;
        private float h;
        private Action action;
        private bool MouseHovered = false;
        private string text;
        private string tooltip;
        private int yOffset = 0;

        public MainMenuButtonRightSide(string text, float verticalOffset, Action action = null, float textScale = 0.55f, string tooltip = "", int yOffset = 0) : base(text, textScale, large: true)
        {
            // Position
            HAlign = 0.5f;
            VAlign = 0.23f + verticalOffset;
            Top.Set(0, 0);
            Left.Set(260f + 50f, 0);

            // Size
            w = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One).X;
            h = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One).Y;
            Width.Set(w, 0);
            Height.Set(h, 0);

            // Settings
            TextColor = Color.Gray;
            this.action = action;
            this.text = text;
            this.tooltip = tooltip;
            this.yOffset = yOffset;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            action?.Invoke();
            Log.Info("Clicked on: " + Text);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            TextColor = Color.Yellow;
            MouseHovered = true;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            TextColor = Color.Gray;
            MouseHovered = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override bool ContainsPoint(Vector2 point)
        {
            Vector2 newPoint = point + new Vector2(x: -20, y: -10);
            return base.ContainsPoint(newPoint);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // 0 = Main menu (singleplayer, multiplayer, achievemenets, etc.)
            // Only draw in this mode
            // workaround for the fact that Main.menuMode is not set to 0 when the main menu is open
            if (Main.menuMode != 0)
            {
                return;
            }

            // Draw debug if needed
            // var debugDrawer = new BasicDebugDrawer(spriteBatch.GraphicsDevice);
            // debugDrawer.Begin();
            // Rectangle hitbox = GetDimensions().ToRectangle();
            // hitbox.X += 20; // Invert the X offset from ContainsPoint
            // hitbox.Y += 10; // Invert the Y offset from ContainsPoint
            // debugDrawer.DrawSquare(new Vector4(hitbox.X, hitbox.Y, hitbox.Width, hitbox.Height), Color.Red * 0.3f);
            // debugDrawer.End();

            // Apply the current scale
            // this.SetText(this.Text, currentScale, large: true);

            // Draw with the updated scale
            base.Draw(spriteBatch);

            if (MouseHovered && !string.IsNullOrEmpty(tooltip))
            {
                DrawHelper.DrawMainMenuTooltipPanel(this, text, tooltip, yOffset);
            }
        }
    }
}