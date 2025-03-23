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

        public bool IsHovered => IsMouseHovering;

        public ModEnabledIcon(Texture2D tex, string internalModName) : base(tex)
        {
            this.internalModName = internalModName;

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
            string path = $"{internalModName}/icon";

            updatedTex = ModContent.Request<Texture2D>(path).Value;

            if (updatedTex != null)
            {
                DrawHelper.DrawProperScale(spriteBatch, this, updatedTex);
            }
        }
    }
}