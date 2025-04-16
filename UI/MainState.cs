using ModHelper.UI.Elements;
using Terraria.UI;

namespace ModHelper.UI
{
    public class MainState : UIState
    {
        public MainState()
        {
            // Add UI elements here
            DebugText debugText = new("");
            Append(debugText);
        }
    }
}