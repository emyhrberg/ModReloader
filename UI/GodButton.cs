using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class GodButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText) : BaseButton(buttonImgText, buttonImgNoText, hoverText)
    {

        public override void LeftClick(UIMouseEvent evt)
        {
            GodModePlayer.GodMode = !GodModePlayer.GodMode;
            Log.Info("God mode clicked. God mode is now " + GodModePlayer.GodMode);
            UpdateTexture();
        }

        public override void UpdateTexture()
        {
            base.UpdateTexture();

            // GodModePlayer godModePlayer = Main.LocalPlayer.GetModPlayer<GodModePlayer>();
            bool isGodModeOn = GodModePlayer.GodMode;
            Config c = ModContent.GetInstance<Config>();
            bool hideText = c?.General.HideButtonText ?? true;

            // Now update the current image asset based on the toggle state.
            if (isGodModeOn)
            {
                if (hideText)
                    CurrentImage = Assets.ButtonGodNoText;
                else
                    CurrentImage = Assets.ButtonGod;
            }
            else
            {
                if (hideText)
                    CurrentImage = Assets.ButtonGodOffNoText;
                else
                    CurrentImage = Assets.ButtonGodOff;
            }

            SetImage(CurrentImage);
        }
    }
}