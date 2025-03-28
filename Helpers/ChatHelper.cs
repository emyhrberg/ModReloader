using Microsoft.Xna.Framework;
using ModHelper.Common.Configs;
using Terraria;

namespace ModHelper.Helpers
{
    public static class ChatHelper
    {
        public static void NewText(string text, Color? color = null)
        {
            if (Conf.C.LogToChat)
            {
                // Only log to chat if the config option is set to true
                Main.NewText(text, color);
            }
        }
    }
}
