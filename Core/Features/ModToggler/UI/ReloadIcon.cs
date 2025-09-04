using Microsoft.Xna.Framework.Graphics;
using ModReloader.Core.Features.Reload;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModReloader.Core.Features.ModToggler.UI
{
    public class ReloadIcon : UIImage
    {
        private Texture2D tex;
        private string hover;
        private string internalModName;

        public ReloadIcon(Texture2D texture, string modName, string hover = "") : base(texture)
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
            // Close fancy UI
            IngameFancyUI.Close();
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