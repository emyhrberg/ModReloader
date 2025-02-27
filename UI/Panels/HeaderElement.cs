using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    public class HeaderElement : PanelElement
    {
        public HeaderElement(string title) : base(title)
        {
            IsHoverEnabled = false;
            textElement.TextColor = Color.White;
        }
    }
}