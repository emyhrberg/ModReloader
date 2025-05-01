using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ReLogic.OS;
using Terraria;
using Terraria.GameContent.UI.Chat;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.Common.Systems.Hooks
{
    // moves the chatbox by 500 pixels to the right
    public class ChatHook : ModSystem
    {
        public override void Load()
        {
            //On_RemadeChatMonitor.DrawChat += DrawChatHook;
        }

        public override void Unload()
        {
            //On_RemadeChatMonitor.DrawChat -= DrawChatHook;
        }

        private static void DrawChatHook(On_RemadeChatMonitor.orig_DrawChat orig, RemadeChatMonitor self, bool drawingPlayerChat)
        {
            // Set the value of _startChatLine.
            // Log.Info("Setting _startChatLine to 0");
            // typeof(RemadeChatMonitor).GetField("_startChatLine", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(null, 3);

            // Call the original method
            orig(self, drawingPlayerChat);

            // Get the chatbox element
            // var chatBox = (UIElement)typeof(RemadeChatMonitor).GetField("_chatBox", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self);

            // Move the chatbox 500 pixels to the right
            // chatBox.Left.Set(chatBox.Left.Pixels + 500, 0f);
        }
    }
}