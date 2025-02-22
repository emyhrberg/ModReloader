using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using SquidTestingMod.PacketHandlers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class TestButton(Asset<Texture2D> buttonImgText, string hoverText, bool animating) : BaseButton(buttonImgText, hoverText, animating)
    {
        private int aaa = 0;
        public override void LeftClick(UIMouseEvent evt)
        {
            Main.NewText($"Terraria title before change:{Main.instance.Window.Title}");
            Main.instance.Window.Title = $"aaaa{aaa}";
            aaa++;
            Main.NewText($"Terraria title after change:{Main.instance.Window.Title}");
        }

        public override void RightClick(UIMouseEvent evt)
        {

        }

    }
}