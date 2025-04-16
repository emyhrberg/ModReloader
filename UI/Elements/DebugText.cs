using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModHelper.UI.Elements
{
    public class DebugText : UIText
    {
        public DebugText(string text, float textScale = 0.9f, bool large = false) : base(text, textScale, large)
        {
            TextColor = Color.White;
            VAlign = 0.95f;
            HAlign = 0.01f;

            // start text at top left corner of its element
            TextOriginX = 0f;
            TextOriginY = 0f;

            // Arbitrary size, should use ChatManager.GetStringSize() instead
            Width.Set(150, 0);
            Height.Set(20 * 4 + 10, 0); // 20 * 3 for 3 lines of text
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // get stuff to show
            string playerName = Main.LocalPlayer.name;
            int whoAmI = Main.myPlayer;
            int fps = Main.frameRate;
            int ups = Main.updateRate;

            string netmode = Main.netMode switch
            {
                NetmodeID.SinglePlayer => "SP",
                NetmodeID.MultiplayerClient => "MP",
                _ => "Unknown Mode"
            };

            string logFilePath = Path.GetFileName(Logging.LogPath);

            string text = $"Plr: {playerName}, Wld: {Main.ActiveWorldFileData.Name} \n";
            text += $"ID: {whoAmI}, Mode: {netmode}, Log: {logFilePath} \n";
            text += $"Debugger: {Debugger.IsAttached}, PID: {System.Environment.ProcessId} \n";
            text += $"{fps}fps {ups}ups ({Main.upTimerMax:0}ms)\n";
            text += "TEST999";

            SetText(text, 0.9f, large: false);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // if chat is open, dont draw the text
            if (Main.drawingPlayerChat)
            {
                return;
            }
            base.Draw(spriteBatch);
        }
    }
}