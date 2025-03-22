using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class ConfigButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, float textSize) : BaseButton(spritesheet, buttonText, hoverText, textSize)
    {
        private float _scale = 0.2f;
        protected override float Scale => _scale;
        protected override int FrameCount => 5;
        protected override int FrameSpeed => 10;
        protected override int FrameWidth => 640;
        protected override int FrameHeight => 640;


        public override void LeftClick(UIMouseEvent evt)
        {
            Conf.C.Open();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update Scale dynamically based on the size of the button
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            MainState mainState = sys?.mainState;
            float buttonSize = mainState?.ButtonSize ?? 70f;
            _scale = 0.2f + (buttonSize - 70f) * 0.005f;
        }
    }
}