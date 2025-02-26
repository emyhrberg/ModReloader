using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// Superclass for options inside player, debug, world panel
    /// Contains a text element and some size and positioning only
    /// Is overriden in child classes for slider elements, header elements, etc
    /// </summary>
    public class TestElement : UIElement
    {
        // Variables
        private int panelPadding = 12;
        public int panelElementHeight = 25;
        protected UIText textElement;

        public TestElement(string title = "Test Element")
        {
            MaxWidth.Set(panelPadding * 2, 1f);
            Width.Set(panelPadding * 2, 1f);
            Height.Set(panelElementHeight, 0f);
            Left.Set(-panelPadding, 0f);

            Top.Set(150f, 0f);

            // Text element
            textElement = new UIText(text: title, textScale: 1.0f, large: false);
            textElement.TextColor = Color.White;
            textElement.HAlign = 0.5f;
            textElement.VAlign = 0.5f;
            Append(textElement);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText("This is a test element.");
            }
        }
    }
}