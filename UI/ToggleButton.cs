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

        public ToggleButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText) : base(buttonImgText, buttonImgNoText, hoverText)
        {
            UpdateTexture();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (IsMouseHovering)
            {
                Config c = ModContent.GetInstance<Config>();
                if (c.ShowTooltips)
                {
                    ButtonsSystem sys = ModContent.GetInstance<ButtonsSystem>();
                    string toggleText = sys.myState.AreButtonsVisible ? "Hide buttons \n Right click to drag" : "Show buttons \n Right click to drag";
                    UICommon.TooltipMouseText(toggleText);
                }
            }
        }

        public override void UpdateTexture()
        {
            // Get system state
            ButtonsSystem sys = ModContent.GetInstance<ButtonsSystem>();

            if (sys == null || sys.myState == null)
                return;

            // Get config state
            Config c = ModContent.GetInstance<Config>();
            bool showText = c?.ShowButtonText ?? true; // Default to true if config is null

            // Get whether buttons are currently visible
            bool isOn = sys.myState.AreButtonsVisible;

            // Set correct texture based on both ON/OFF state and text visibility
            SetImage(isOn ? (showText ? Assets.ButtonOn : Assets.ButtonOnNoText)
                          : (showText ? Assets.ButtonOff : Assets.ButtonOffNoText));
        }

        public override void HandleClick()
        {
            Log.Info("ToggleButton clicked.");

            // Get ButtonsSystem
            ButtonsSystem sys = ModContent.GetInstance<ButtonsSystem>();

            // Toggle visibility of all buttons
            sys?.myState?.ToggleAllButtonsVisibility();

            // Update textures for ON/OFF and text display
            UpdateTexture();
        }

        public override void RightDoubleClick(UIMouseEvent evt)
        {
            CombatText.NewText(Main.LocalPlayer.getRect(), Color.Orange, "Type /toggle to show again!");

            // update config state
            ModContent.GetInstance<Config>().ShowToggleButton = false;

            // literally disable the entire state
            ButtonsSystem sys = ModContent.GetInstance<ButtonsSystem>();
            sys.HideUI();
        }

        public override bool ContainsPoint(Vector2 point)
        {
            // Define the inner 50x50 area for the ON/OFF switch
            // var innerArea = new Rectangle((int)Left.Pixels + 25, (int)Top.Pixels + 25, 50, 50);
            // return innerArea.Contains(point.ToPoint());
            return base.ContainsPoint(point);
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
                    // ConfigButton
                    sys.myState.configButton.Left.Set(Main.mouseX - dragOffset.X + 100, 0f);
                    sys.myState.configButton.Top.Set(Main.mouseY - dragOffset.Y, 0f);

                    // ItemsButton
                    sys.myState.itemBrowserButton.Left.Set(Main.mouseX - dragOffset.X + 200, 0f);
                    sys.myState.itemBrowserButton.Top.Set(Main.mouseY - dragOffset.Y, 0f);

                    // RefreshButton
                    sys.myState.refreshButton.Left.Set(Main.mouseX - dragOffset.X + 300, 0f);
                    sys.myState.refreshButton.Top.Set(Main.mouseY - dragOffset.Y, 0f);

                }

                Recalculate();
            }
        }
        #endregion
    }
}