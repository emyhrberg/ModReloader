using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using Terraria.ModLoader;

namespace SquidTestingMod.UI
{
    public class ButtonGod(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText) : BaseButton(buttonImgText, buttonImgNoText, hoverText)
    {
        public override void HandleClick()
        {
            GodModePlayer.GodMode = !GodModePlayer.GodMode;
        }

        public override void UpdateTexture()
        {
            base.UpdateTexture();

            bool isGodModeOn = GodModePlayer.GodMode;
            Config c = ModContent.GetInstance<Config>();
            bool showText = c?.General.ShowButtonText ?? true;

            // Now update the current image asset based on the toggle state.
            // if (isGodModeOn)
            //     CurrentImage = showText ? Assets.ButtonGod : Assets.ButtonGodNoText;
            // else
            //     CurrentImage = showText ? Assets.ButtonGodOff : Assets.ButtonGodOffNoText;

            // SetImage(CurrentImage);
        }
    }
}