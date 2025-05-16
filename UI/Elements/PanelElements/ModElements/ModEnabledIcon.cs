using System;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Helpers;
using Terraria.GameContent.UI.Elements;

namespace ModReloader.UI.Elements.PanelElements.ModElements
{
    public class ModEnabledIcon : UIImage
    {
        private string internalModName;
        public Texture2D updatedTex;
        public bool IsHovered => IsMouseHovering;

        private Texture2D icon;

        public ModEnabledIcon(Texture2D tex, string internalModName = "", Texture2D icon = null, bool large = false) : base(tex)
        {
            this.icon = icon;
            this.internalModName = internalModName;

            float size = 25f;
            if (large)
            {
                size *= 2f; // double size for large icon
            }
            MaxHeight.Set(size, 0f);
            MaxWidth.Set(size, 0f);
            Width.Set(size, 0f);
            Height.Set(size, 0f);
            VAlign = 0.5f;

            // custom top
            Top.Set(-1, 0);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw disabled icons v2.
            if (icon != null)
            {
                updatedTex = icon;
                DrawHelper.DrawProperScale(spriteBatch, this, updatedTex);
                return;
            }

            // Draw enabled icons.
            string path = $"{internalModName}/icon";

            try
            {
                updatedTex = ModContent.Request<Texture2D>(path).Value;
            }
            catch (Exception e)
            {
                Log.SlowInfo("Failed to get updatedTex:" + e);
            }

            if (updatedTex != null)
            {
                DrawHelper.DrawProperScale(spriteBatch, this, updatedTex);
            }
        }
    }
}