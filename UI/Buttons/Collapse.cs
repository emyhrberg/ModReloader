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

        // Constructor
        public Collapse(Asset<Texture2D> collapseDown, Asset<Texture2D> collapseUp) : base(collapseDown) // Start with Down texture
        {
            // Set textures
            CollapseDown = collapseDown;
            CollapseUp = collapseUp;

            // Size and position
            Width.Set(37, 0);
            Height.Set(15, 0);
            Left.Set(0, 0);
            Top.Set(-70, 0); // Start at normal position for Expanded state

            // Alignment bottom center
            VAlign = 1f;
            HAlign = 0.5f;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // Update button visibility
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
                    Log.Info("Collapse: Disabling MP button");
                    mpBtn.Active = false;
                    mpBtn.buttonUIText.Active = false;
                }

            }

            UpdateCollapseImage();
        }

        // Update visuals based on state
        private void UpdateCollapseImage()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            if (sys?.mainState != null)
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
        }

        public override void Update(GameTime gameTime)
        {
            if (Conf.HideCollapseButton && !Main.playerInventory)
                return;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Conf.HideCollapseButton && !Main.playerInventory)
                return;

            base.Draw(spriteBatch);
        }
    }
}