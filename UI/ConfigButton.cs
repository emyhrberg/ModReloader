using System;
using System.Linq;
using System.Reflection;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ConfigButton(Asset<Texture2D> texture, string hoverText) : BaseButton(texture, hoverText)
    {
        public override void HandleClick(UIMouseEvent evt, UIElement listeningElement)
        {
            Config c = ModContent.GetInstance<Config>();
            c.Open();
        }
    }
}