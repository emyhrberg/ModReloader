using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ToggleButton(Asset<Texture2D> texture, string hoverText) : BaseButton(texture, hoverText)
    {
        public override void HandleClick()
        {
            Log.Info("ToggleButton clicked.");

            // get buttonsstate
            ButtonsSystem sys = ModContent.GetInstance<ButtonsSystem>();

            // toggle all buttons visibility if the sys and state is not null
            sys?.myState?.ToggleAllButtonsVisibility();

            // toggle on or off texturing
            SetImage(sys?.myState?.AreButtonsVisible ?? false ? Assets.ToggleButtonOn : Assets.ToggleButtonOff);
        }

        public override void RightClick(UIMouseEvent evt)
        {
            // Hide this button
        }

        #region dragging
        private bool dragging;
        private Vector2 dragOffset;
        public override void RightMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);
            dragging = true;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
            Main.LocalPlayer.mouseInterface = true;
        }

        public override void RightMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false;
            Main.LocalPlayer.mouseInterface = false;
            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (dragging)
            {
                Left.Set(Main.mouseX - dragOffset.X, 0f);
                Top.Set(Main.mouseY - dragOffset.Y, 0f);

                // move all other buttons too
                ButtonsSystem sys = ModContent.GetInstance<ButtonsSystem>();
                if (sys?.myState?.AreButtonsVisible ?? false)
                {
                    sys.myState.itemBrowserButton.Left.Set(Main.mouseX - dragOffset.X + 100, 0f);
                    sys.myState.itemBrowserButton.Top.Set(Main.mouseY - dragOffset.Y, 0f);
                    sys.myState.refreshButton.Left.Set(Main.mouseX - dragOffset.X + 200, 0f);
                    sys.myState.refreshButton.Top.Set(Main.mouseY - dragOffset.Y, 0f);
                    sys.myState.configButton.Left.Set(Main.mouseX - dragOffset.X + 300, 0f);
                    sys.myState.configButton.Top.Set(Main.mouseY - dragOffset.Y, 0f);
                }

                Recalculate();
            }
        }
        #endregion
    }
}