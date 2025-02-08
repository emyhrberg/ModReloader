using System;
using System.Reflection;
using log4net.Repository.Hierarchy;
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

namespace SquidTestingMod.UI
{
    public abstract class BaseButton : UIImageButton
    {
        private readonly Asset<Texture2D> _buttonImgText;
        private readonly Asset<Texture2D> _buttonImgNoText;

        public string HoverText { get; private set; }
        public Asset<Texture2D> CurrentImage { get; set; }
        public float RelativeLeftOffset { get; set; }

        public BaseButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText)
            : base(buttonImgText) // Default to the text version
        {
            _buttonImgText = buttonImgText;
            _buttonImgNoText = buttonImgNoText;
            HoverText = hoverText;
        }

        /// <summary>
        /// My function that updates 1) CurrentImage and 2) ButtonScale.
        /// </summary>
        public virtual void UpdateTexture()
        {
            // 1) CurrentImage.
            Config config = ModContent.GetInstance<Config>();
            // set showtext to config setting. if config is null. default to true
            bool hideText = config?.General.HideButtonText ?? true;
            if (hideText)
                CurrentImage = _buttonImgNoText;
            else
                CurrentImage = _buttonImgText;

            SetImage(CurrentImage);

            // 2) ButtonScale.
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys == null || sys.mainState == null)
            {
                Log.Error("MainSystem or MainState is null. Cannot update texture.");
                return;
            }

            sys.mainState.ButtonScale = config.General.ButtonSize;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetInnerDimensions();
            if (CurrentImage != null && CurrentImage.Value != null)
            {
                float opacity = IsMouseHovering ? 1f : 0.4f;

                // Get ButtonScale from MainState
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                float ButtonScale = sys.mainState.ButtonScale;

                // Calculate the drawn size based on ButtonScale.
                float drawWidth = CurrentImage.Value.Width * ButtonScale;
                float drawHeight = CurrentImage.Value.Height * ButtonScale;

                // Center the image within the UI element.
                Vector2 drawPos = dimensions.Position() + new Vector2((dimensions.Width - drawWidth) / 2f, (dimensions.Height - drawHeight) / 2f);
                spriteBatch.Draw(CurrentImage.Value, drawPos, null, Color.White * opacity, 0f, Vector2.Zero, ButtonScale, SpriteEffects.None, 0f);
            }

            if (IsMouseHovering)
            {
                Config c = ModContent.GetInstance<Config>();
                if (c.General.HideButtonTooltips)
                    UICommon.TooltipMouseText(HoverText);
            }
        }

        // Abstract HandleClick = force children (all buttons) to implement this method
        public abstract void HandleClick();

        #region fix dont use items when clicking buttons
        public override void LeftClick(UIMouseEvent evt)
        {
            // Don't use items when clicking buttons
            Main.LocalPlayer.mouseInterface = true;
            base.LeftClick(evt);
            Main.LocalPlayer.mouseInterface = false;
        }

        #endregion

        #region ContainsPoint
        public override bool ContainsPoint(Vector2 point)
        {
            // First, if you have any special logic for not accepting clicks (such as the UIModConfig workaround),
            // perform that check first. (This part is identical to your current code.)
            try
            {
                if (Main.InGameUI != null)
                {
                    var currentStateProp = Main.InGameUI.GetType().GetProperty("CurrentState", BindingFlags.Public | BindingFlags.Instance);
                    if (currentStateProp != null)
                    {
                        var currentState = currentStateProp.GetValue(Main.InGameUI);
                        if (currentState != null)
                        {
                            string stateName = currentState.GetType().Name;
                            if (stateName.Contains("Config"))
                                return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error checking UI state in ContainsPoint: " + ex.Message);
            }

            // Now calculate the clickable area based on the drawn image.
            CalculatedStyle dimensions = GetInnerDimensions();

            // Get the current ButtonScale from your MainState.
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            float buttonScale = (sys != null && sys.mainState != null)
                ? sys.mainState.ButtonScale
                : 1f;

            // Ensure we have a valid image.
            if (CurrentImage != null && CurrentImage.Value != null)
            {
                // Calculate the scaled width and height.
                float drawWidth = CurrentImage.Value.Width * buttonScale;
                float drawHeight = CurrentImage.Value.Height * buttonScale;

                // Determine the position where the image is drawn.
                // (This should match the calculation in DrawSelf.)
                Vector2 drawPos = dimensions.Position() +
                                  new Vector2((dimensions.Width - drawWidth) / 2f, (dimensions.Height - drawHeight) / 2f);

                // Build a rectangle for the clickable area.
                Rectangle hitbox = new Rectangle((int)drawPos.X, (int)drawPos.Y, (int)drawWidth, (int)drawHeight);

                // Return true if the point falls inside this hitbox.
                return hitbox.Contains(point.ToPoint());
            }

            // If there's no image, fall back to the base method.
            return base.ContainsPoint(point);
        }
        #endregion
    }
}