using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace ModReloader.UI.Elements.PanelElements.ModElements
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
                UICommon.TooltipMouseText(hover);
            }
        }
    }
}