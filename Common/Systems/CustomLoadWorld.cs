using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModHelper.Common.Systems
{
    public class CustomLoadWorld : UIState
    {
        private static UIText centeredText;

        public override void OnInitialize()
        {
            // Create a centered UIText element
            centeredText = new UIText("Loading world...", 1f, true)
            {
                HAlign = 0.5f, // Center horizontally
                VAlign = 0.5f  // Center vertically
            };
            Append(centeredText);
        }

        public static CustomLoadWorld Show(string worldName = "")
        {
            // Create instance
            var screen = new CustomLoadWorld();
            // Switch to a custom menuMode
            Main.menuMode = 888;
            // Activate this UI
            Main.MenuUI.SetState(screen);
            centeredText.SetText($"Loading world: {worldName}...");
            return screen;
        }
    }
}
