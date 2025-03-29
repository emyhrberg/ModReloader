using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModHelper.Common.Systems
{
    public class MainMenuButton : UIText
    {
        bool isButtonFullyZoomedIn = false;

        public MainMenuButton(string text, float textScale = 1f) : base(text, textScale)
        {
            // Constructor logic here
            HAlign = 0.5f;
            VAlign = 0.5f;
            Width.Set(200, 0);
            Height.Set(50, 0);
            TextColor = Color.Gray;
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            // base.MouseOver(evt);
            TextColor = Color.Yellow;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            // base.MouseOut(evt);
            TextColor = Color.Gray;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // base.LeftClick(evt);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}