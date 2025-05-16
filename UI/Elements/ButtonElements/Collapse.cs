using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Systems;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace ModReloader.UI.Elements.ButtonElements
{
    public class Collapse : UIImage
    {
        private readonly Asset<Texture2D> CollapseDown;
        private readonly Asset<Texture2D> CollapseUp;

        // Constructor
        public Collapse(Asset<Texture2D> down, Asset<Texture2D> up) : base(down) // Start with Down texture
        {
            // Set textures
            CollapseDown = down;
            CollapseUp = up;
            SetImage(CollapseDown.Value);

            // Size and position
            Width.Set(37, 0);
            Height.Set(15, 0);
            //Left.Set(-35, 0); // CUSTOM CUSTOM CUSTOM -20!
            Top.Set(-70 + 10, 0); // Start at normal position for Expanded state

            // Alignment bottom center
            VAlign = 0.99f;
            HAlign = 0.5f;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            ToggleCollapse();
        }

        public void ToggleCollapse()
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

        public void RecalculateSizeAndPosition()
        {
            var state = ModContent.GetInstance<MainSystem>().mainState;
            float bs = state.ButtonSize;

            // pick whatever aspect‐ratio you like for the little arrow
            float w = bs * 0.5f;
            float h = bs * 0.2f;

            Width.Set(w, 0f);
            Height.Set(h, 0f);

            // always centre horizontally & hug bottom
            HAlign = 0.5f;
            VAlign = 0.99f;

            if (state.AreButtonsShowing)
            {
                // up
                Top.Set(-70 * state.UIScale + 5 * state.UIScale, 0);
            }
            else
            {
                Top.Set(0, 0);
            }

            // custom x for smaller scales
            if (state.UIScale < 1.0)
            {
                Left.Set(-2, 0);
            }

            Recalculate(); // push updates through immediately
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

                // force MP button to disable when expanded if in SP
                if (Main.netMode == NetmodeID.SinglePlayer && btn is ReloadMPButton spBtn)
                {
                    spBtn.Active = false;
                    spBtn.ButtonText.Active = false;
                }
            }
        }

        // Update visuals based on state
        public void UpdateCollapseImage()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            MainState mainState = sys?.mainState;

            if (mainState != null)
            {
                // if (Conf.C.ButtonPosition == "Bottom")
                // {
                if (mainState.AreButtonsShowing)
                {
                    SetImage(CollapseDown.Value);
                    Top.Set(-70 + 10, 0); // Expanded
                }
                else
                {
                    SetImage(CollapseUp.Value);
                    Top.Set(0, 0); // Collapsed
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