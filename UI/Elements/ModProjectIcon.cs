using System;
using System.Diagnostics;
using System.IO;
using ErkysModdingUtilities.Common.Configs;
using ErkysModdingUtilities.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ErkysModdingUtilities.UI.Elements
{
    public class ModProjectIcon : UIImage
    {
        private Texture2D tex;
        private string hover;
        private string modPath;

        public ModProjectIcon(Texture2D texture, string modPath, string hover = "") : base(texture)
        {
            tex = texture;
            this.hover = hover;
            this.modPath = modPath;

            float size = 23f;
            MaxHeight.Set(size, 0f);
            MaxWidth.Set(size, 0f);
            Width.Set(size, 0f);
            Height.Set(size, 0f);
            VAlign = 0.5f;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            try
            {
                string modName = Path.GetFileName(modPath);
                if (Conf.LogToChat) Main.NewText("Opening mod project: " + modPath);
                string csprojFile = Path.Combine(modPath + "/" + modName + ".csproj");
                Process.Start(new ProcessStartInfo($@"{csprojFile}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Log.Error("Error opening mod project: " + ex.Message);
            }
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