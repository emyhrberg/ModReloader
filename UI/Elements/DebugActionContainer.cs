using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    public class DebugActionContainer : UIElement
    {
        private UIText button1;
        private UIText button2;
        private UIText button3;
        private bool Active = true;

        public DebugActionContainer()
        {
            VAlign = 0.89f;
            HAlign = 0.04f;

            Top.Set(30, 0);

            Width.Set(220, 0);
            Height.Set(20 * 4 + 10, 0);

            // Create three clickable UIText elements
            string fileName = Path.GetFileName(Logging.LogPath);

            // button3 = new DebugAction("Open config", 60, () => Conf.C.Open());
            // Append(button3);

            button1 = new DebugAction($"Open log", 0, Log.OpenClientLog);
            Append(button1);

            button2 = new DebugAction($"Clear log", 30, Log.ClearClientLog);
            Append(button2);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active || Main.drawingPlayerChat)
                return;

            base.Draw(spriteBatch);
        }
    }
}