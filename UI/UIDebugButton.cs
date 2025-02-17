using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class UIDebugButton : BaseButton
    {
        public bool IsUIDebugDrawing = false;

        public UIDebugButton(Asset<Texture2D> _image, string hoverText) : base(_image, hoverText)
        {
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            IsUIDebugDrawing = !IsUIDebugDrawing;
        }
    }
}