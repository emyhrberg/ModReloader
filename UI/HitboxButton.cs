using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class HitboxButton : BaseButton
    {
        public bool IsDrawingHitboxes = false;

        public HitboxButton(Asset<Texture2D> _image, string hoverText) : base(_image, hoverText)
        {
            UpdateTexture();
        }

        public override void UpdateTexture()
        {
            base.UpdateTexture();

            if (IsDrawingHitboxes)
                _Texture = Assets.ButtonHitboxOn;
            else
                _Texture = Assets.ButtonHitboxOff;

            SetImage(_Texture);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            IsDrawingHitboxes = !IsDrawingHitboxes;
            UpdateTexture();
        }
    }
}