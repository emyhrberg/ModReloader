using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    public class CustomTitlePanel : UIPanel
    {
        public UIText HeaderText;
        private Color lightBlue = new(63, 82, 151);
        private int padding = 12;
        private int height = 35;

        public CustomTitlePanel(string header)
        {
            // Set up the panel
            MaxWidth.Set(padding * 2, 1f);
            Width.Set(padding * 2, 1f);
            Height.Set(height, 0f);
            Top.Set(-padding, 0f);
            Left.Set(-padding, 0f);
            BackgroundColor = lightBlue;

            // Create the header text, center it vertically (and horizontally if desired)
            HeaderText = new UIText(header, textScale: 0.6f, large: true);
            HeaderText.VAlign = 0.5f;
            HeaderText.HAlign = 0.5f;

            Append(HeaderText);
        }
    }
}
