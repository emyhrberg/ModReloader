using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    public class CustomItemCountText : UIText
    {
        public CustomItemCountText(string text, float textScale = 1f) : base(text, textScale, large: true)
        {
            HAlign = 0.5f;
            VAlign = 1f;
        }
    }
}