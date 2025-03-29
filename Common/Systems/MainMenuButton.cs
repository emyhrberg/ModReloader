using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace ModHelper.Common.Systems
{
    public class MainMenuButton : UIText
    {
        // Zoom
        private float currentScale = 0.6f;
        private float targetScale = 0.7f;
        private float NORMAL_SCALE = 0.6f;
        private float HOVER_SCALE = 0.7f;
        private float SCALE_SPEED = 0.2f;

        // Width and height of the text
        private float w;
        private float h;

        private Action action;

        public MainMenuButton(string text, float verticalOffset, Action action = null) : base(text, textScale: 0.45f, large: true)
        {
            // Position
            HAlign = 0.12f;
            VAlign = 0.3f + verticalOffset;
            Top.Set(0, 0);
            Left.Set(0, 0);

            // Size
            w = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One).X;
            h = ChatManager.GetStringSize(FontAssets.MouseText.Value, text, Vector2.One).Y;
            Width.Set(w, 0);
            Height.Set(h, 0);

            // Settings
            TextColor = Color.Gray;
            this.action = action;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            action?.Invoke();
            Log.Info("Clicked on: " + Text);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            TextColor = Color.Yellow;
            targetScale = HOVER_SCALE; // Set target scale to hover scale
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            TextColor = Color.Gray;
            targetScale = NORMAL_SCALE; // Reset target scale when not hovering
        }

        public override void Update(GameTime gameTime)
        {
            // hot reload testing
            //HAlign = -0.01f;
            //VAlign = 0.3f;
            //Top.Set(0, 0);
            //Left.Set(0, 0);
            //Recalculate();

            // Save original center position before scaling
            CalculatedStyle originalDimensions = GetDimensions();
            Vector2 originalCenter = new Vector2(
                originalDimensions.X + originalDimensions.Width / 2,
                originalDimensions.Y + originalDimensions.Height / 2
            );

            // Smooth transition between scales
            if (currentScale != targetScale)
            {
                currentScale = MathHelper.Lerp(currentScale, targetScale, SCALE_SPEED);
                if (Math.Abs(currentScale - targetScale) < 0.01f)
                    currentScale = targetScale;

                // Apply the new scale
                this.SetText(this.Text, currentScale, large: true);

                // Update width/height based on the new scale
                DynamicSpriteFont font = FontAssets.MouseText.Value;
                float newWidth = ChatManager.GetStringSize(font, Text, Vector2.One * currentScale).X;
                float newHeight = ChatManager.GetStringSize(font, Text, Vector2.One * currentScale).Y;

                Width.Set(newWidth, 0);
                Height.Set(newHeight, 0);

                Recalculate();

                // Get new dimensions after scaling
                CalculatedStyle newDimensions = GetDimensions();

                // Calculate offsets to keep centered
                float offsetX = (newDimensions.Width - originalDimensions.Width) / 2;
                float offsetY = (newDimensions.Height - originalDimensions.Height) / 2;

                // Adjust position to maintain center point
                Left.Set(Left.Pixels - offsetX, Left.Percent);
                Top.Set(Top.Pixels - offsetY, Top.Percent);

                Recalculate();
            }

            base.Update(gameTime);
        }

        public override bool ContainsPoint(Vector2 point)
        {
            Vector2 newPoint = point + new Vector2(x: -20, y: -10);
            return base.ContainsPoint(newPoint);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.End();
            //spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);



            // Draw debug if needed
            if (Main.menuMode == 0)
            {
                var debugDrawer = new BasicDebugDrawer(spriteBatch.GraphicsDevice);
                debugDrawer.Begin();
                Rectangle hitbox = GetDimensions().ToRectangle();
                hitbox.X += 20; // Invert the X offset from ContainsPoint
                hitbox.Y += 10; // Invert the Y offset from ContainsPoint
                debugDrawer.DrawSquare(new Vector4(hitbox.X, hitbox.Y, hitbox.Width, hitbox.Height), Color.Red * 0.3f);
                debugDrawer.End();

                // Apply the current scale
                this.SetText(this.Text, currentScale, large: true);

                // Draw with the updated scale
                base.Draw(spriteBatch);
            }
        }
    }
}