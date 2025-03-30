using Microsoft.Xna.Framework;
using Terraria;

namespace ModHelper.Helpers
{
    public static class ChatHelper
    {
        public static void NewText(string text, Color? color = null)
        {
            Main.NewText(text, color);
        }
    }
}
