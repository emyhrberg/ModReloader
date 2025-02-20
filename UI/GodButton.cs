using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class GodButton : BaseButton
    {
        public GodButton(Asset<Texture2D> _image, string hoverText) : base(_image, hoverText)
        {
            UpdateTexture();
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            GodModePlayer.IsGodModeOn = !GodModePlayer.IsGodModeOn;
            UpdateTexture();
        }

        public override void UpdateTexture()
        {
            base.UpdateTexture();

            bool isGodModeOn = GodModePlayer.IsGodModeOn;

            // Now update the current image asset based on the toggle state.
            if (isGodModeOn)
                _Texture = Assets.ButtonGodOn;
            else
                _Texture = Assets.ButtonGodOff;

            SetImage(_Texture);
        }
    }
}