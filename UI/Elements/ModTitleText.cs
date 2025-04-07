using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    public class ModTitleText : UIText
    {
        public string hover = "";
        private string internalModName = "";

        public ModTitleText(string text, string hover = "", Action leftClick = null, Action rightClick = null, float textSize = 1f, string internalModName = "", bool large = false) : base(text, textSize, large)
        {
            this.hover = hover;
            this.internalModName = internalModName;
            Left.Set(0, 0);
            VAlign = 0.5f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (!string.IsNullOrEmpty(hover) && IsMouseHovering)
            {
                if (!Conf.C.ShowTooltips)
                {
                    return;
                }
                UICommon.TooltipMouseText(hover);
            }
        }
    }
}