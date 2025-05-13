using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.UI;
using Terraria.UI;
using Terraria.UI.Chat;
using static System.Net.Mime.MediaTypeNames;

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
             base.LeftClick(evt);

            Active = !Active;

            // Open client log
            // Log.OpenClientLog();
        }

        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);

            Conf.C.Open();
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

        public override void Draw(SpriteBatch sb)
        {
            //Top.Set(-5, 0);
            //Height.Set(20 * 3+10, 0);

            //DrawHelper.DrawDebugHitbox(this, Color.Green);

            if (!Active)
            {
                return;
            }

            // also, if chat is open, hide the text
            if (Main.drawingPlayerChat)
            {
                //return;
            }

            base.Draw(sb);

            if (IsMouseHovering)
            {
                //Vector2 pos = new(Main.MouseScreen.X-16, Main.MouseScreen.Y-24);
                CalculatedStyle dims = this.GetDimensions();
                Vector2 posHigh = dims.Position() + new Vector2(0,-18);

                DrawHelper.DrawOutlinedStringOnMenu(sb, FontAssets.MouseText.Value, "Click to hide debug info", posHigh, Color.White,
                    rotation: 0f, origin: Vector2.Zero, scale: 0.8f, effects: SpriteEffects.None, layerDepth: 0f,
                    alphaMult: 0.8f);

                Vector2 posLow = dims.Position() + new Vector2(0,3);

                DrawHelper.DrawOutlinedStringOnMenu(sb, FontAssets.MouseText.Value, "Right click to open config", posLow, Color.White,
                    rotation: 0f, origin: Vector2.Zero, scale: 0.8f, effects: SpriteEffects.None, layerDepth: 0f,
                    alphaMult: 0.8f);
            }
        }
    }
}