using System.Linq;
using ModHelper.Helpers;
using ModHelper.UI;
using Terraria;
using Terraria.ModLoader;

namespace ModHelper.Common.Commands
{
    public class ReloadCommand : ModCommand
    {
        public override string Command => "r";

        public override string Description => "RELOAD WITH HOT RELOAD!!!";

        public override CommandType Type => CommandType.Chat;

        public override async void Action(CommandCaller caller, string input, string[] args)
        {
            await ReloadHelper.SinglePlayerReload();
        }
    }
}