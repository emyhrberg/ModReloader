using System.Linq;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ModHelper.UI;
using Terraria;
using Terraria.ModLoader;

namespace ModHelper.Common.Commands
{
    public class OpenConfigCommand : ModCommand
    {
        public override string Command => "c";

        public override string Description => "OPEN CONFIG!!!";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Conf.C.Open();
        }
    }
}