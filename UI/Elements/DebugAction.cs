using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    // open log, clear log text
    public class DebugAction : UIText
    {
        private bool Active = true;
        private string hover = "";
        private Action action;
        private float leftOffset;

        public DebugAction(string text, string hover = "", Action action = null, float left = 0f) : base(text, textScale: 0.9f, large: false)
        {
            TextColor = Color.White;

            // start text at top left corner of its element
            TextOriginX = 0f;
            TextOriginY = 0f;

            // Arbitrary size, should use ChatManager.GetStringSize() instead
            Width.Set(80, 0);
            Height.Set(30, 0); // 20 * 3 for 3 lines of text
            Left.Set(left, 0);

            this.hover = hover;
            this.action = action;
            this.leftOffset = left;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            //always call base! otherwise IsMouseHovering wont work
            base.MouseOver(evt);
            //TextColor = new Color(237, 246, 255);
            TextColor = Color.Yellow;
            // Color textColor = hovered ? new Color(237, 246, 255) : new Color(173, 173, 198);
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            //always call base! otherwise IsMouseHovering wont work
            base.MouseOut(evt);
            TextColor = Color.White;
            //TextColor = new Color(173, 173, 198);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // always call base! otherwise IsMouseHovering wont work
            base.LeftClick(evt);
            if (action != null)
            {
                action.Invoke();
            }
            else
            {
                // open log file in default text editor
                // string logPath = Path.Combine(Main.SavePath, "Logs", "tModLoader.txt");
                // Process.Start(new ProcessStartInfo(logPath) { UseShellExecute = true });
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
            {
                return;
            }

            if (leftOffset == 81)
            {
                Left.Set(73, 0);
                //DrawHelper.DrawDebugHitbox(this, Color.Orange);
            }
            //else
            //{
            //DrawHelper.DrawDebugHitbox(this, Color.Blue);
            //}

            VAlign = 1.00f;
            HAlign = 0.005f;
            Top.Set(0, 0);

            // also, if chat is open, hide the text
            if (Main.drawingPlayerChat)
            {
                return;
            }

            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true; // disable item use if the button is hovered
                string fileName = Path.GetFileName(Logging.LogPath);
                //Main.hoverItemName = hover;
            }
        }

        public override void Update(GameTime gameTime)
        {
            // prob not needed?
            //base.Update(gameTime);

            //if (IsMouseHovering)
            //{
            //    // Main.hoverItemName = "Click to open log file";
            //    Main.hoverItemName = hover;
            //}
        }
    }
}