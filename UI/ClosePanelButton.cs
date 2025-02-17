using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ClosePanelButton : UIImageButton
    {
        public Asset<Texture2D> texture;

        public ClosePanelButton() : base(Assets.X)
        {
            texture = Assets.X;
            Left.Set(0, 0f);
            Top.Set(0, 0f);
            Width.Set(32, 0f);
            Height.Set(32, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // adjust opacity based on mouse hover
            float opacity = IsMouseHovering ? 1f : 0.7f;

            // draw the texture
            spriteBatch.Draw(texture.Value, GetDimensions().ToRectangle(), Color.White * opacity);
        }


        public override void LeftClick(UIMouseEvent evt)
        {
            // Close the item panel for now.
            // TODO close any parent panel.
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            sys.mainState.itemButton.ToggleItemsPanel();
        }
    }
}