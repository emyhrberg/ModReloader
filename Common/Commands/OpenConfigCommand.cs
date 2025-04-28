using ModHelper.Common.Configs;
using Terraria.ModLoader;

namespace ModHelper.Common.Commands
{
    public class OpenConfigCommand : ModCommand
    {
        public override string Command => "c";

        public override string Description => "Open ModHelper config.";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Conf.C.Open();
        }
    }
}