using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class HitboxButton : BaseButton
    {
        public bool DrawHitboxFlag = false;

        public HitboxButton(Asset<Texture2D> _image, string hoverText) : base(_image, hoverText)
        {
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            DrawHitboxFlag = !DrawHitboxFlag;
        }
    }
}