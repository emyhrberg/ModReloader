using Terraria.GameContent.UI.Elements;

namespace ModReloader.Core.Features.ModToggler.UI
{
    public class ModsFoundPanel : UIPanel
    {
        public ModsFoundPanel(string text)
        {
            // size of panel
            // size and position
            Width.Set(-35f, 1f);
            Height.Set(30, 0);
            Left.Set(5, 0);

            Append(new UIText(text, 1.0f, false)
            {
                Width = { Pixels = 0 },
                Height = { Pixels = 0 },
                HAlign = 0.5f,
                VAlign = 0.5f,
                TextColor = Color.White,
            });
        }
    }
}