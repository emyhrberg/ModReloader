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
    public class GodButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText) : BaseButton(buttonImgText, buttonImgNoText, hoverText)
    {
        // Fast player
        public bool IsFastMode = false;

        public override void LeftClick(UIMouseEvent evt)
        {
            GodModePlayer.GodMode = !GodModePlayer.GodMode;
            Log.Info("God mode clicked. God mode is now " + GodModePlayer.GodMode);
            UpdateTexture();
        }

        public override void RightClick(UIMouseEvent evt)
        {
            IsFastMode = !IsFastMode;
            Log.Info("Fast mode clicked. Fast mode is now " + IsFastMode);
            UpdateTexture();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (IsFastMode)
            {
                // get button center
                CalculatedStyle dimensions = GetInnerDimensions();
                Vector2 buttonPos = dimensions.Position();
                Vector2 buttonCenter = buttonPos + new Vector2(dimensions.Width / 2f, dimensions.Height / 2f);

                // measure string and center it
                Vector2 textSize = FontAssets.MouseText.Value.MeasureString("Fast") * 1.0f;
                Vector2 textPos = buttonCenter - textSize / 2f;

                // draw string with opacity
                float opacity = 0.4f;
                if (IsMouseHovering)
                    opacity = 1f;

                Utils.DrawBorderString(spriteBatch, "Fast", textPos, Color.White * opacity);
            }
        }

        public override void UpdateTexture()
        {
            base.UpdateTexture();

            bool isGodModeOn = GodModePlayer.GodMode;
            Config c = ModContent.GetInstance<Config>();
            bool hideText = c?.General.HideButtonText ?? true;

            // Now update the current image asset based on the toggle state.
            if (isGodModeOn)
            {
                CurrentImage = hideText ? Assets.ButtonGodNoText : Assets.ButtonGod;
            }
            else
            {
                CurrentImage = hideText ? Assets.ButtonGodOffNoText : Assets.ButtonGodOff;
            }

            SetImage(CurrentImage);
        }
    }
}
