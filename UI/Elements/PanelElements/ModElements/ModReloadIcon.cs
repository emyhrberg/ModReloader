using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Configs;
using ModReloader.Helpers;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModReloader.UI.Elements.PanelElements.ModElements
{
    public class ModReloadIcon : UIImage
    {
        private Texture2D tex;
        private string hover;
        private string internalModName;

        public ModReloadIcon(Texture2D texture, string modName, string hover = "") : base(texture)
        {
            tex = texture;
            this.hover = hover;
            internalModName = modName;

            // size and pos
            float size = 23f;
            MaxHeight.Set(size, 0f);
            MaxWidth.Set(size, 0f);
            Width.Set(size, 0f);
            Height.Set(size, 0f);
            VAlign = 1.0f;
            Top.Set(6, 0);
        }

        public async override void LeftClick(UIMouseEvent evt)
        {
            Conf.C.ModsToReload = [internalModName];
            Conf.Save();
            await ReloadUtilities.SinglePlayerReload();
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