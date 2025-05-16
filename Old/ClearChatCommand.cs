//using System.Linq;

//namespace ModReloader.Common.Commands
//{
//    public class ClearChatCommand : ModCommand
//    {
//        public override string Command => "clear";

//        public override string Description => "Clears the chat history.";

//        public override CommandType Type => CommandType.Chat;

//        public override void Action(CommandCaller caller, string input, string[] args)
//        {
//            // Clear the chat history
//            // Max visible chats at one point is 10, so we write 10 empty lines
//            foreach (var _ in Enumerable.Range(0, 10))
//            {
//                Main.NewText(string.Empty);
//            }
//        }
//    }
//}