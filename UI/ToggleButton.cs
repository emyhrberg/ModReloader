using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ToggleButton : BaseButton
    {

        // Variables
        private bool needsTextureUpdate = true;
        private bool isSmall = false; // Track if the button is in small mode

        // Constructor
        public ToggleButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText) : base(buttonImgText, buttonImgNoText, hoverText)
        {
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }

        public override void UpdateTexture()
        {
            // First update the base (which sets VisualScale and the default image asset).
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
            string t = "Hide Toggle Button. Type /toggle to show again!";
            CombatText.NewText(Main.LocalPlayer.getRect(), Color.Orange, "t");
            Main.NewText(t, Color.Orange);

            // update config state
            ModContent.GetInstance<Config>().General.ShowToggleButton = false;
            ModContent.GetInstance<MainSystem>().SetUIStateToNull();
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

                // update the position of this ToggleButton
                Left.Set(Main.mouseX - dragOffset.X, 0f);
                Top.Set(Main.mouseY - dragOffset.Y, 0f);

                // move all other buttons too
                if (sys?.mainState?.AreButtonsVisible ?? false)
                {
                    // ConfigButton
                    sys.mainState.configButton.Left.Set(Main.mouseX - dragOffset.X + 100, 0f);
                    sys.mainState.configButton.Top.Set(Main.mouseY - dragOffset.Y, 0f);

                    // ItemsButton
                    sys.mainState.itemButton.Left.Set(Main.mouseX - dragOffset.X + 200, 0f);
                    sys.mainState.itemButton.Top.Set(Main.mouseY - dragOffset.Y, 0f);

                    // NPCButton
                    sys.mainState.npcButton.Left.Set(Main.mouseX - dragOffset.X + 300, 0f);
                    sys.mainState.npcButton.Top.Set(Main.mouseY - dragOffset.Y, 0f);

                    // RefreshButton
                    sys.mainState.refreshButton.Left.Set(Main.mouseX - dragOffset.X + 400, 0f);
                    sys.mainState.refreshButton.Top.Set(Main.mouseY - dragOffset.Y, 0f);
                }

                Recalculate();
            }
        }
        #endregion
    }
}