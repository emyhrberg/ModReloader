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

            // Ensure the correct initial texture is set
            // UpdateTexture();
        }

        /// <summary>
        /// My function that updates 1) CurrentImage and 2) ButtonScale.
        /// </summary>
        public virtual void UpdateTexture()
        {
            // 1) CurrentImage.
            Config config = ModContent.GetInstance<Config>();
            // set showtext to config setting. if config is null. default to true
            bool showText = config?.General.ShowButtonText ?? true;
            if (showText)
                CurrentImage = _buttonImgText;
            else
                CurrentImage = _buttonImgNoText;
            SetImage(CurrentImage);

            // 2) ButtonScale.
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys == null || sys.mainState == null)
            {
                Log.Error("MainSystem or MainState is null. Cannot update texture.");
                return;
            }

            sys.mainState.ButtonScale = config.General.ButtonSizes switch
            {
                "Small" => 0.45f,
                "Medium" => 0.7f,
                _ => 1.0f,
            };
            sys.mainState.ButtonScale = 1f;
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
                if (c.General.ShowTooltips)
                    UICommon.TooltipMouseText(HoverText);
            }
        }

        // public override void LeftClick(UIMouseEvent evt)
        // {
        //     // base.LeftClick(evt);
        //     // Mark the mouse click as used by the UI to disable item usage
        //     // Main.LocalPlayer.mouseInterface = true;

        //     // Execute your custom click behavior
        //     // HandleClick();
        // }

        // public override void LeftMouseDown(UIMouseEvent evt)
        // {
        //     Main.LocalPlayer.mouseInterface = true; // CANNOT use items.
        // }

        // public override void LeftMouseUp(UIMouseEvent evt)
        // {
        //     Main.LocalPlayer.mouseInterface = false; // ALLOW item usage again.
        // }

        // Abstract HandleClick = force children (all buttons) to implement this method
        public abstract void HandleClick();

        #region disable click
        public override bool ContainsPoint(Vector2 point)
        {
            // --- This is a workaround to prevent the buttons from being clicked when the UIModConfig is open. ---
            try
            {
                if (Main.InGameUI != null)
                {
                    // Use reflection to get the "CurrentState" property from Main.InGameUI.
                    var currentStateProp = Main.InGameUI.GetType().GetProperty("CurrentState", BindingFlags.Public | BindingFlags.Instance);
                    if (currentStateProp != null)
                    {
                        var currentState = currentStateProp.GetValue(Main.InGameUI);
                        if (currentState != null)
                        {
                            string stateName = currentState.GetType().Name;
                            // UIModConfig will be open when we click config button
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

            return base.ContainsPoint(point);
        }
        #endregion
    }
}