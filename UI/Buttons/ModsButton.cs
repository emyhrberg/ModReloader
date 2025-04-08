using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace ModHelper.UI.Buttons
{
    public class ModsButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set button image size
        private float _scale = 0.9f;
        protected override float Scale => _scale;

        // OLD BUTTON, DO NOT DELETE
        // protected override int FrameWidth => 55;
        // protected override int FrameHeight => 70;
        // protected override int FrameCount => 10;
        // protected override int FrameSpeed => 4;

        protected override int FrameWidth => 60;
        protected override int FrameHeight => 58;
    }
}