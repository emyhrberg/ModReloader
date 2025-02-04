using Terraria.ModLoader;
using Terraria;

namespace ServerPortals.Commands
{
    public class JoinMPCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "mp";
        public override string Description => "Join multiplayer server";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // get the current world data
            // Main.ActiveWorldFileData = new WorldFileData("world1", false);

            WorldGen.SaveAndQuit(() =>
            {
                Main.menuMode = 10; // go to main menu
                Netplay.SetRemoteIP("localhost");
                Netplay.ListenPort = 7777;
                Netplay.StartTcpClient(); // enter the server as a client
            });
        }
    }
}