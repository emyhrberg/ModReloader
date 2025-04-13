using System.Linq;
using ModHelper.UI;
using Terraria;
using Terraria.ModLoader;

namespace ModHelper.Common.Commands
{
    public class ToggleUIElement : ModCommand
    {
        public override string Command => "ui";

        public override string Description => "Toggle UIElement Panel.";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            // sys.mainState.uiPanel.SetActive(!sys.mainState.uiPanel.GetActive());
        }
    }
}