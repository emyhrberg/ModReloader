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
    public class FastButton : BaseButton
    {
        // Fast player
        public bool IsFastMode = false;

        public FastButton(Asset<Texture2D> _image, string hoverText) : base(_image, hoverText)
        {
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            IsFastMode = !IsFastMode;
            Log.Info("Fast mode clicked. Fast mode is now " + IsFastMode);
            UpdateTexture();
        }

        public override void UpdateTexture()
        {
            base.UpdateTexture();

            // Now update the current image asset based on the toggle state.
            if (IsFastMode)
                _Texture = Assets.ButtonFastOn;
            else
                _Texture = Assets.ButtonFastOff;

            SetImage(_Texture);
        }

        // some useless code for drawing a string on the button
        // public override void Draw(SpriteBatch spriteBatch)
        // {
        //     base.Draw(spriteBatch);

        //     if (IsFastMode)
        //     {
        //         // get button center
        //         CalculatedStyle dimensions = GetInnerDimensions();
        //         Vector2 buttonPos = dimensions.Position();
        //         Vector2 buttonCenter = buttonPos + new Vector2(dimensions.Width / 2f, dimensions.Height / 2f);

        //         // measure string and center it
        //         Vector2 textSize = FontAssets.MouseText.Value.MeasureString("Fast") * 1.0f;
        //         Vector2 textPos = buttonCenter - textSize / 2f;

        //         // draw string with opacity
        //         float opacity = 0.4f;
        //         if (IsMouseHovering)
        //             opacity = 1f;

        //         Utils.DrawBorderString(spriteBatch, "Fast", textPos, Color.White * opacity);
        //     }
        // }
    }
}
