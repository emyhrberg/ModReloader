using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class CustomTitlePanel : UIPanel
    {
        public CustomTitlePanel(int padding, Color bgColor, int height)
        {
            MaxWidth.Set(padding * 2, 1f);
            Width.Set(padding * 2, 1f);
            Height.Set(height, 0f);
            Top.Set(-padding, 0f);
            Left.Set(-padding, 0f);
            BackgroundColor = bgColor;
        }
    }
}