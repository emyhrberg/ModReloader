using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ToggleButton : BaseButton
    {
        // Variables
        private bool needsTextureUpdate = true;

        // Constructor
        public ToggleButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText) : base(buttonImgText, buttonImgNoText, hoverText)
        {
        }

        public override void UpdateTexture()
        {
            // First update the base (which sets ButtonScale and the default image asset).
            base.UpdateTexture();

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys == null || sys.mainState == null)
            {
                Log.Info("MainSystem or MainState is null. Cannot update texture.");
                return;
            }

            Config c = ModContent.GetInstance<Config>();
            bool showText = c?.General.ShowButtonText ?? true;
            bool isOn = sys.mainState.AreButtonsVisible;

            // Now update the current image asset based on the toggle state.
            if (isOn)
                CurrentImage = showText ? Assets.ButtonOn : Assets.ButtonOnNoText;
            else
                CurrentImage = showText ? Assets.ButtonOff : Assets.ButtonOffNoText;

            SetImage(CurrentImage);
        }

        public override void HandleClick()
        {
            Log.Info("ToggleButton clicked.");

            MainSystem sys = ModContent.GetInstance<MainSystem>();

            // Toggle visibility of all buttons
            sys?.mainState?.ToggleAllButtonsVisibility();

            // Update textures for ON/OFF and text on buttons
            UpdateTexture();
        }

        public override void RightClick(UIMouseEvent evt)
        {
            // flip the switch
            Config c = ModContent.GetInstance<Config>();
            if (c == null) return;
            c.General.OnlyShowWhenInventoryOpen = !c.General.OnlyShowWhenInventoryOpen;

            string t = c.General.OnlyShowWhenInventoryOpen ? "Buttons will only show when inventory is open" : "Buttons will always show";
            Main.NewText(t, Color.White);
        }

        #region dragging
        private bool dragging;
        private Vector2 dragOffset;
        private Vector2 mouseDownPos;
        private bool isDrag;
        private const float DragThreshold = 10f; // you can tweak the threshold
        private bool clickStartedOutsideButton;

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            dragging = true;
            isDrag = false;
            mouseDownPos = evt.MousePosition; // store mouse down location
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
            clickStartedOutsideButton = !ContainsPoint(evt.MousePosition);
            if (!clickStartedOutsideButton)
                Main.LocalPlayer.mouseInterface = true;
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false;
            Main.LocalPlayer.mouseInterface = false;
            Recalculate();

            if (isDrag && !clickStartedOutsideButton)
            {
                HandleClick();
            }
        }
        #endregion

        #region update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // fix update texture
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (needsTextureUpdate && sys?.mainState != null)
            {
                UpdateTexture();
                needsTextureUpdate = false; // Run once
            }

            if (dragging || (ContainsPoint(Main.MouseScreen) && !clickStartedOutsideButton))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            // update the position of all the buttons
            if (dragging)
            {
                // disable mouse interface
                Main.LocalPlayer.mouseInterface = true;

                // Check if we moved the mouse more than the threshold
                var currentPos = new Vector2(Main.mouseX, Main.mouseY);
                if (Vector2.Distance(currentPos, mouseDownPos) > DragThreshold)
                {
                    isDrag = true;
                }

                // get anchor pos of this button based on what we have dragOffset it with
                Vector2 newAnchorPosition = new(Main.mouseX - dragOffset.X, Main.mouseY - dragOffset.Y);

                // update the position of all the buttons
                sys.mainState.UpdateButtonsPositions(newAnchorPosition);

                Recalculate();
            }
        }
        #endregion
    }
}