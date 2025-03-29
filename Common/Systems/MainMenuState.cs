using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModHelper.Common.Systems
{
    public class MainMenuState : UIState
    {
        private UIText skipSelectButton;

        public MainMenuState()
        {
            // Constructor logic here
            skipSelectButton = new UIText("Skip Select", textScale: 0.5f);
            Append(skipSelectButton);

        }
    }
}