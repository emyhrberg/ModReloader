using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModReloader.Core.Features.Reload
{
    public class LoadWorldState : UIState
    {
        private static UIText loadingWorldText;
        private static UIText loadingPlayerText;

        public override void OnInitialize()
        {
            // Create a centered UIText element
            loadingWorldText = new UIText(Loc.Get("MainMenu.LoadingWorld", ""), 0.7f, true)
            {
                Top = { Pixels = 5 }, // Position it 30 pixels above the world loading text
                HAlign = 0.5f, // Center horizontally
                VAlign = 0.33f,  // Approx random vertical position, approx where the vanilla loading screen is
                TextColor = new(237, 246, 255) // Set text color to white
            };
            Append(loadingWorldText);

            // Create a centered UIText element for player loading above it (top.set -30 pixels above)
            loadingPlayerText = new UIText(Loc.Get("MainMenu.LoadingPlayer", ""), 0.7f, true)
            {
                Top = { Pixels = -35 }, // Position it 30 pixels above the world loading text
                HAlign = 0.5f, // Center horizontally
                VAlign = 0.33f,  // Approx random vertical position, approx where the vanilla loading screen is
                TextColor = new(237, 246, 255) // Set text color to white
            };
            Append(loadingPlayerText);
        }

        public static LoadWorldState Show(string worldName, string playerName)
        {
            // Null check for worldName and playerName
            if (worldName == null)
                worldName = "?";
            if (playerName == null)
                playerName = "?";

            // Activate this UI
            var screen = new LoadWorldState();
            Main.menuMode = 888;
            Main.MenuUI.SetState(screen);

            // Set the text for loading world and player
            loadingWorldText.SetText($"{Loc.Get("MainMenu.LoadingWorld", worldName)}...");
            loadingPlayerText.SetText($"{Loc.Get("MainMenu.LoadingPlayer", playerName)}...");

            return screen;
        }
    }
}
