using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModReloader.Common.Systems
{
    public class LoadWorldState : UIState
    {
        private static UIText centeredText;

        public override void OnInitialize()
        {
            // Create a centered UIText element
            centeredText = new UIText("Loading world...", 0.7f, true)
            {
                HAlign = 0.5f, // Center horizontally
                VAlign = 0.33f,  // Approx random vertical position, approx where the vanilla loading screen is
                TextColor = new(237, 246, 255) // Set text color to white
            };
            Append(centeredText);
        }

        public static LoadWorldState Show(string worldName = "")
        {
            // Create instance
            var screen = new LoadWorldState();
            // Switch to a custom menuMode
            Main.menuMode = 888;
            // Activate this UI
            Main.MenuUI.SetState(screen);
            centeredText.SetText($"Loading world: {worldName}..");
            return screen;
        }
    }
}
