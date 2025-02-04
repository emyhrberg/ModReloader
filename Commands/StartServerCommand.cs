using Terraria.ModLoader;
using Terraria;

namespace ServerPortals.Commands
{
    public class StartServerCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "server";
        public override string Description => "Start a server";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // get the current world data
            // Main.ActiveWorldFileData = new WorldFileData("world1", false);

            WorldGen.SaveAndQuit(() =>
            {
                // go to main menu
                Main.menuMode = 10;

                // set the IP AND PORT (the two necessary fields) for the server
                Netplay.SetRemoteIP("localhost");
                Netplay.ListenPort = 7777;

                // enter the server as a client
                Netplay.StartTcpClient();
            });
        }
    }
}