using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI.Elements.DebugElements
{
    public class DebugText : UIText
    {
        private bool Active = true;

        public DebugText(string text, float textScale = 0.9f, bool large = false) : base(text, textScale, large)
        {
            TextColor = Color.White;
            VAlign = 1.00f;
            HAlign = 0.005f;

            // start text at top left corner of its element
            TextOriginX = 0f;
            TextOriginY = 0f;

            // Arbitrary size, should use ChatManager.GetStringSize() instead
            Width.Set(150, 0);
            Top.Set(-5, 0);
            Height.Set(20 * 3 + 10, 0);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            //always call base! otherwise IsMouseHovering wont work
            base.MouseOver(evt);
            // TextColor = Color.Yellow;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            //always call base! otherwise IsMouseHovering wont work
            base.MouseOut(evt);
            // TextColor = Color.White;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // base.LeftClick(evt);

            // Open client log
            // Log.OpenClientLog();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Conf.C.AddDebugText)
            {
                return;
            }

            // update the text to show playername, whoAmI, and FPS
            string playerName = Main.LocalPlayer.name;
            int whoAmI = Main.myPlayer;
            int fps = Main.frameRate;
            int ups = Main.updateRate;

            string netmode = Main.netMode switch
            {
                NetmodeID.SinglePlayer => "SP",
                NetmodeID.MultiplayerClient => "MP",
                _ => "Unknown"
            };

            string logFileName = Path.GetFileName(Logging.LogPath);

            string text = "";
            text += $"\nName: {playerName}, ID: {whoAmI}, Mode: {netmode}";
            //text += $"\nDebugger: {Debugger.IsAttached}, PID: {System.Environment.ProcessId}";
            text += $"\n{fps}fps {ups}ups ({Main.upTimerMax:0}ms)";

            //Main.instance.Window.Title = " += PID HERE? FOR EASY DEBUG INFO";

            SetText(text, 0.9f, large: false);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //Top.Set(-5, 0);
            //Height.Set(20 * 3+10, 0);

            // draw debug hitbox
            //DrawHelper.DrawDebugHitbox(this, Color.Green);

            if (!Active)
            {
                return;
            }

            // also, if chat is open, hide the text
            if (Main.drawingPlayerChat)
            {
                return;
            }

            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                // Main.LocalPlayer.mouseInterface = true; // disable item use if the button is hovered
                string fileName = Path.GetFileName(Logging.LogPath);
                //Main.hoverItemName = $"Left click to open {fileName}\nRight click to hide text";
            }
        }
    }
}