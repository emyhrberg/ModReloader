using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;

namespace ModHelper.UI.Buttons
{
    public class ButtonText : UIText
    {
        public bool Active = true;

        public ButtonText(string text, float textScale = 0.9f, bool large = false) : base(text, textScale, large)
        {
            // Place the buttontext at the bottom of the button.
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

            base.Draw(spriteBatch);
        }
    }
}