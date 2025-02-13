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
        public override string Command => "show";
        public override string Description => "Show the buttons UI.";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // update config state
            ModContent.GetInstance<Config>().General.OnlyShowWhenInventoryOpen = true;
        }
    }
}