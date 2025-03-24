using System;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace SquidTestingMod.UI.Elements
{
    public class ModEnabledIcon : UIImage
    {
        private string internalModName;
        public Texture2D updatedTex;
        private bool hasIcon;

        public bool IsHovered => IsMouseHovering;

        public ModEnabledIcon(Texture2D tex, string internalModName = "", bool hasIcon=true) : base(tex)
        {
            this.internalModName = internalModName;
            this.hasIcon = hasIcon;

            float size = 25f;
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
            if (!hasIcon)
            {
                //base.Draw(spriteBatch);
                return;
            }

            string path = $"{internalModName}/icon";

            try
            {
                updatedTex = ModContent.Request<Texture2D>(path).Value;
            }
            catch (Exception e)
            {
                Log.Warn("Failed to get updatedTex:" + e);
            }

            if (updatedTex != null)
            {
                DrawHelper.DrawProperScale(spriteBatch, this, updatedTex);
            }
        }
    }
}