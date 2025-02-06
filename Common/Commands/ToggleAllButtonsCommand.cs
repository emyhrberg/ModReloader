using SquidTestingMod.Helpers;
using SquidTestingMod.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.Common.Commands
{
    public class ToggleAllButtonsCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;
        public override string Command => "btns";
        public override string Description => "Toggles the buttons";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Main.NewText("Toggling btns...");

            // get button state
            ButtonsSystem sys = ModContent.GetInstance<ButtonsSystem>();

            // toggle the items panel
            sys?.myState?.ToggleAllButtons();
        }
    }
}