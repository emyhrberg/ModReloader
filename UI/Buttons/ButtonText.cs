using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace SquidTestingMod.UI.Buttons
{
    public class ButtonText : UIText
    {
        public ButtonText(string text, float textScale = 0.9f, bool large = false) : base(text, textScale, large)
        {
            HAlign = 0.5f;
            VAlign = 0.85f;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys?.mainState.AreButtonsShowing ?? true)
            {
                base.Draw(spriteBatch);
            }
        }
    }
}