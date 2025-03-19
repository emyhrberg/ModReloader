using System;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using SquidTestingMod.Reload;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    public class ModReloadIcon : UIImage
    {
        private Texture2D tex;
        private string hover;
        private string modName;

        public ModReloadIcon(Texture2D texture, string modName, string hover = "") : base(texture)
        {
            tex = texture;
            this.hover = hover;
            this.modName = modName;

            // size and pos
            float size = 23f;
            MaxHeight.Set(size, 0f);
            MaxWidth.Set(size, 0f);
            Width.Set(size, 0f);
            Height.Set(size, 0f);
            VAlign = 0.5f;
        }

        public async override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            // Pre-step:
            // Set config to reload this mod
            Conf.C.ModToReload = modName;
            Conf.ForceSaveConfig(Conf.C);

            await ReloadUtils.ReloadEverything();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            DrawHelper.DrawProperScale(spriteBatch, this, tex, scale: 1.0f);

            if (!string.IsNullOrEmpty(hover) && IsMouseHovering)
            {
                UICommon.TooltipMouseText(hover);
            }
        }
    }
}