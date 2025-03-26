using ErkysModdingUtilities.Common.Configs;
using ErkysModdingUtilities.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ErkysModdingUtilities.UI.Buttons
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

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            MainState mainState = sys?.mainState;
            float buttonSize = mainState?.ButtonSize ?? 0;

            // Size and position
            Width.Set(37, 0);
            Height.Set(15, 0);
            Left.Set(-20, 0); // CUSTOM CUSTOM CUSTOM -20!
            Top.Set(-buttonSize, 0); // Start at normal position for Expanded state

            // Alignment bottom center
            VAlign = 1f;
            HAlign = 0.5f;

            if (Conf.ButtonsPosition == "left")
            {
                HAlign = 0f;
                VAlign = 0.8f;
                Top.Set(-buttonSize - 15 / 2, 0);
                Left.Set(buttonSize, 0);
                SetImage(CollapseLeft.Value); // unsure if this works
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            MainState mainState = sys?.mainState;
            if (mainState != null)
            {
                mainState.AreButtonsShowing = !mainState.AreButtonsShowing;
                UpdateButtonVisibility();
                UpdateCollapseImage();
            }
        }

        public void SetCollapsed(bool isCollapsed)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            MainState mainState = sys?.mainState;

            if (mainState != null)
            {
                mainState.AreButtonsShowing = !isCollapsed;
                UpdateButtonVisibility();
                UpdateCollapseImage();
            }
        }

        private void UpdateButtonVisibility()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys == null)
            {
                Log.Info("MainSystem is null");
                return;
            }

            foreach (BaseButton btn in sys.mainState.AllButtons)
            {
                btn.Active = sys.mainState.AreButtonsShowing;

                // force MP button to disable when expanded
                if (btn is ReloadMPButton mpBtn)
                {
                    mpBtn.Active = false;
                    mpBtn.ButtonText.Active = false;
                }
            }
        }

        // Update visuals based on state
        public void UpdateCollapseImage()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            MainState mainState = sys?.mainState;
            float buttonSize = mainState?.ButtonSize ?? 0;

            if (mainState != null)
            {
                if (Conf.ButtonsPosition == "bottom")
                {
                    if (mainState.AreButtonsShowing)
                    {
                        SetImage(CollapseDown.Value);
                        Top.Set(-buttonSize, 0); // Expanded
                    }
                    else
                    {
                        SetImage(CollapseUp.Value);
                        Top.Set(0, 0); // Collapsed
                    }
                }
                else if (Conf.ButtonsPosition == "left")
                {
                    if (mainState.AreButtonsShowing)
                    {
                        SetImage(CollapseLeft.Value);
                        Left.Set(buttonSize, 0); // Expanded
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
            // disable item use on click
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}