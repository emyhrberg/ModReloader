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
        private readonly Asset<Texture2D> CollapseDown;
        private readonly Asset<Texture2D> CollapseUp;
        private readonly Asset<Texture2D> CollapseLeft;
        private readonly Asset<Texture2D> CollapseRight;

        // Constructor
        public Collapse(Asset<Texture2D> down, Asset<Texture2D> up, Asset<Texture2D> left, Asset<Texture2D> right) : base(down) // Start with Down texture
        {
            // Set textures
            CollapseDown = down;
            CollapseUp = up;
            CollapseLeft = left;
            CollapseRight = right;

            // Size and position
            Width.Set(37, 0);
            Height.Set(15, 0);
            Left.Set(-20, 0); // CUSTOM CUSTOM CUSTOM -20!
            Top.Set(-70, 0); // Start at normal position for Expanded state

            // Alignment bottom center
            VAlign = 1f;
            HAlign = 0.5f;

            if (Conf.ButtonsPosition == "left")
            {
                HAlign = 0f;
                VAlign = 0.8f;
                Top.Set(-70 - 15 / 2, 0);
                Left.Set(70, 0);
                // SetImage(CollapseLeft.Value); // unsure if this works
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            UpdateButtonVisibility();
            UpdateCollapseImage();
        }

        private void UpdateButtonVisibility()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            sys.mainState.AreButtonsShowing = !sys.mainState.AreButtonsShowing;

            foreach (BaseButton btn in sys.mainState.AllButtons)
            {
                btn.Active = sys.mainState.AreButtonsShowing;
                if (btn.buttonUIText != null)
                {
                    btn.buttonUIText.Active = sys.mainState.AreButtonsShowing;
                }

                // force MP button to disable when expanded
                if (btn is ReloadMPButton mpBtn)
                {
                    mpBtn.Active = false;
                    mpBtn.buttonUIText.Active = false;
                }

            }
        }

        // Update visuals based on state
        public void UpdateCollapseImage()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            if (sys?.mainState != null)
            {
                if (Conf.ButtonsPosition == "bottom")
                {
                    if (sys.mainState.AreButtonsShowing)
                    {
                        SetImage(CollapseDown.Value);
                        Top.Set(-70, 0); // Expanded
                    }
                    else
                    {
                        SetImage(CollapseUp.Value);
                        Top.Set(0, 0); // Collapsed
                    }
                }
                else if (Conf.ButtonsPosition == "left")
                {
                    if (sys.mainState.AreButtonsShowing)
                    {
                        SetImage(CollapseLeft.Value);
                        Left.Set(70, 0); // Expanded
                    }
                    else
                    {
                        SetImage(CollapseRight.Value);
                        Left.Set(0, 0); // Collapsed
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Conf.HideCollapseButton && !Main.playerInventory)
                return;

            // disable item use on click
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Conf.HideCollapseButton && !Main.playerInventory)
                return;

            base.Draw(spriteBatch);
        }
    }
}