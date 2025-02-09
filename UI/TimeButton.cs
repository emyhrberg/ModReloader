// EnemiesButton.cs
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class TimeButton : BaseButton
    {
        private EnemiesSlider slider = new EnemiesSlider();
        private bool isSliderVisible = false;

        public TimeButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText)
            : base(buttonImgText, buttonImgNoText, hoverText)
        {
            // Position slider right above the button, centered above it
            // slider.Left.Set(-30, 0.5f);
            // slider.Top.Set(-slider.Height.Pixels - 10, 0);
        }

        public override void LeftClick(Terraria.UI.UIMouseEvent evt)
        {
            isSliderVisible = !isSliderVisible;
            if (isSliderVisible)
            {
                // NOTE: WE ADD THE SLIDER TO THE BUTTON, NOT THE MAINSTATE, SO ITS KINDA WEIRD WHEN WE MOVE IT
                Append(slider);
                slider.Recalculate();
            }
            else
            {
                RemoveChild(slider);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
