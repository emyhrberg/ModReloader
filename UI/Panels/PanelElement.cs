using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// Superclass for options inside player, debug, world panel
    /// Contains a text element and some size and positioning only
    /// Is overriden in child classes for slider elements, header elements, etc
    /// </summary>
    public abstract class PanelElement : UIElement
    {
        // Variables
        private int panelPadding = 12;
        public int panelElementHeight = 25;
        protected UIText textElement;

        public PanelElement(string title)
        {
            MaxWidth.Set(panelPadding * 2, 1f);
            Width.Set(panelPadding * 2, 1f);
            Height.Set(panelElementHeight, 0f);
            Left.Set(-panelPadding, 0f);

            // Text element
            textElement = new UIText(text: title, textScale: 1.0f, large: false);
            textElement.TextColor = Color.White;
            textElement.HAlign = 0.5f;
            textElement.VAlign = 0.5f;
            Append(textElement);
        }

        public void UpdateText(string text)
        {
            textElement.SetText(text);
        }
    }
}