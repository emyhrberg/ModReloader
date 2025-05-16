using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Systems;
using Terraria.GameContent.UI.Elements;

namespace ModReloader.UI.Elements.ButtonElements
{
    public class ButtonText : UIText
    {
        public bool Active = true;
        private readonly string rawText;
        private readonly float baseTextScale;

        public ButtonText(string text, float textScale = 0.9f, bool large = false) : base(text, textScale, large)
        {
            HAlign = 0.5f;
            VAlign = 0.85f;
            baseTextScale = textScale;
            rawText = text;
        }

        public void ResizeText()
        {
            var state = ModContent.GetInstance<MainSystem>().mainState;

            //Log.SlowInfo("baseTextScale" + baseTextScale);
            //baseTextScale = 0.9f;

            SetText(rawText, 1.1f * state.UIScale, false);
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