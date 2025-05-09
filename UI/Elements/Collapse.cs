using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Systems;
using ModHelper.Helpers;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI.Elements
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

            // TODO maybe add config option
            // "Show by Default" ?
            SetImage(CollapseDown);

            // Size and position
            Width.Set(37, 0);
            Height.Set(15, 0);
            //Left.Set(-20, 0); // CUSTOM CUSTOM CUSTOM -20!
            Top.Set(-70+10, 0); // Start at normal position for Expanded state
            VAlign = 0.99f;
            HAlign = 0.5f;
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
                return;
            }

            foreach (BaseButton btn in sys.mainState.AllButtons)
            {
                btn.Active = sys.mainState.AreButtonsShowing;
                btn.ButtonText.Active = sys.mainState.AreButtonsShowing;
            }
        }

        // Update visuals based on state
        private void UpdateCollapseImage()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            MainState mainState = sys?.mainState;

            if (mainState != null)
            {
                // Swap image directions
                if (mainState.AreButtonsShowing)
                {
                    SetImage(CollapseUp.Value); // Up arrow when expanded
                    Top.Set(-mainState.ButtonSize + 10, 0);
                }
                else
                {
                    SetImage(CollapseDown.Value); // Down arrow when collapsed
                    Top.Set(0, 0);
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