using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModReloader.UI.Elements.MainMenuElements
{
    internal class SpacerMainMenuElement : UIElement
    {
        public SpacerMainMenuElement(float height = 20f)
        {
            Width = StyleDimension.Fill;
            Height = new StyleDimension(height, 0f);
        }
    }
}
