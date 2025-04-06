using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.Elements
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
            this.internalModName = modName;

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
            ReloadUtilities.ModsToReload.Clear();
            if (!ReloadUtilities.ModsToReload.Contains(internalModName))
            {
                ReloadUtilities.ModsToReload.Add(internalModName);
            }

            // Set config.ModToReload to the mod name.
            // Conf.C.ModToReload = internalModName;
            // Conf.Save();

            await ReloadUtilities.MainReload();
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