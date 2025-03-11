using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class Collapse : UIImage
    {
        Asset<Texture2D> collapseDown;
        Asset<Texture2D> collapseUp;

        public override void LeftClick(UIMouseEvent evt)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            sys?.mainState?.ToggleCollapse();

            // Toggle texture
            if (sys.mainState.AreButtonsShowing)
            {
                SetImage(collapseDown.Value);
            }
            else
            {
                SetImage(collapseUp.Value);
            }
        }

        public Collapse(Asset<Texture2D> collapseDown, Asset<Texture2D> collapseUp) : base(collapseDown)
        {
            // set textures
            this.collapseDown = collapseDown;
            this.collapseUp = collapseUp;

            // size
            Width.Set(37, 0);
            Height.Set(15, 0);
            Left.Set(0, 0);
            Top.Set(0, 0);
            VAlign = 0.98f; // todo change
            HAlign = 0.5f; // todo change
        }
    }
}