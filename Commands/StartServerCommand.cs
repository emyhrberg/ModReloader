using System;
using System.Diagnostics;
using Terraria.ModLoader;

namespace SquidTestingMod.Commands
{
    public class StartServerCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "startserver";
        public override string Description => "Launches the server shortcut file";

        public override void Action(CommandCaller caller, string input, string[] args)
        {




        }
    }
}
