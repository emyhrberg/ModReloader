using Terraria.UI;

namespace ModReloader.Core.Features.MainMenuFeatures.UI
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
