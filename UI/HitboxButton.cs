using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class HitboxButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText) : BaseButton(buttonImgText, buttonImgNoText, hoverText)
    {
        public bool DrawHitboxFlag = false;

        public override void LeftClick(UIMouseEvent evt)
        {
            DrawHitboxFlag = !DrawHitboxFlag;
        }
    }
}