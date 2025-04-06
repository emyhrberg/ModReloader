using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class ReloadMPButton : BaseButton
    {
        // Set custom animation dimensions
        private float _scale = 1.15f;
        protected override float Scale => _scale;
        protected override int FrameCount => 5;
        protected override int FrameSpeed => 12;
        protected override int FrameWidth => 65;
        protected override int FrameHeight => 65;

        // Constructor
        public ReloadMPButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : base(spritesheet, buttonText, hoverText, hoverTextDescription)
        {
        }


        public override void LeftClick(UIMouseEvent evt)
        {
            Main.NewText("Not implemented yet.");
        }
    }
}