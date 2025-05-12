using ModHelper.Helpers;

namespace ModHelper.Common.Commands
{
    public class ReloadCommand : ModCommand
    {
        public override string Command => "r";

        public override string Description => "Reload currently selected mods.";

        public override CommandType Type => CommandType.Chat;

        public override async void Action(CommandCaller caller, string input, string[] args)
        {
            await ReloadUtilities.SinglePlayerReload();
        }
    }
}