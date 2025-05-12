using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Systems;
using Terraria.GameContent.UI.Elements;

namespace ModHelper.UI.Elements.ButtonElements
{
    public class ButtonText : UIText
    {
        public bool Active = true;

        public ButtonText(string text, float textScale = 0.9f, bool large = false) : base(text, textScale, large)
        {
            HAlign = 0.5f;
            VAlign = 0.85f;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
                return;

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys?.mainState.AreButtonsShowing ?? true)
            {
                base.Draw(spriteBatch);
            }
        }
    }
}