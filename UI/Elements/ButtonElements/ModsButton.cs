using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;

namespace ModHelper.UI.Elements.ButtonElements
{
    public class ModsButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        public float _scale = 0.45f;
        protected override float Scale => _scale;
        protected override int FrameWidth => 100;
        protected override int FrameHeight => 100;
        //protected override int FrameCount => 16;
        //protected override int FrameSpeed => 4;

        // whoa empty class but it works, because
        // BaseButton and MainState handles implementation
        /// <see cref="BaseButton"/>
        /// <see cref="MainState"/>
    }
}