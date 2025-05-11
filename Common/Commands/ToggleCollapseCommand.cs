using ModHelper.Common.Configs;
using ModHelper.Common.Systems;
using Terraria.ModLoader;

namespace ModHelper.Common.Commands
{
    public class ToggleCollapseCommand : ModCommand
    {
        public override string Command => "collapse";

        public override string Description => "Toggle ModHelper hotbar collapse.";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys != null)
            {
                sys.mainState.collapse.ToggleCollapse();
            }
        }
    }
}