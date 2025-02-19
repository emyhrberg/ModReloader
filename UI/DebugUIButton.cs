using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class DebugUIButton
    (Asset<Texture2D> _image, string hoverText) : BaseButton(_image, hoverText)
    {
        public bool IsUIDebugDrawing = false;

        public override void LeftClick(UIMouseEvent evt)
        {
            IsUIDebugDrawing = !IsUIDebugDrawing;

            if (IsUIDebugDrawing)
            {
                Main.NewText("UIElements:", Color.Green);
            }
        }
    }
}