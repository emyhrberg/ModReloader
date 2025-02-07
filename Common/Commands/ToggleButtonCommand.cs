using System;
using System.Diagnostics;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Commands
{
    public class ToggleButtonCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "toggle";
        public override string Description => "Show the buttons UI.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            // literally enables the entire state
            sys.ShowUI();
            // update config state
            ModContent.GetInstance<Config>().ShowToggleButton = true;
        }
    }
}