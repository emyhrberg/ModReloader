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
        public float VisualScale { get; private set; }

        public string HoverText { get; private set; }
        public Asset<Texture2D> CurrentImage { get; set; }

        public BaseButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText)
            : base(buttonImgText) // Default to the text version
        {
            _buttonImgText = buttonImgText;
            _buttonImgNoText = buttonImgNoText;
            HoverText = hoverText;

            // Ensure the correct initial texture is set
            UpdateTexture();
        }

        public override bool ContainsPoint(Vector2 point)
        {
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
                            {
                                return false;
                            }
                        }
                        else
                        {
                            // Log.Info("Main.InGameUI.CurrentState is null.");
                        }
                    }
                    else
                    {
                        Log.Info("Property 'CurrentState' not found on Main.InGameUI.");
                    }
                }
                else
                {
                    // Log.Info("Main.InGameUI is null.");
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error checking UI state in ContainsPoint: " + ex.Message);
            }

            return base.ContainsPoint(point);
        }

        public virtual void UpdateTexture()
        {
            Config config = ModContent.GetInstance<Config>();
            bool showText = config?.General.ShowButtonText ?? true;

            // Set the current image asset based on showText.
            CurrentImage = showText ? _buttonImgText : _buttonImgNoText;
            SetImage(CurrentImage);

            // Set VisualScale based on config.ButtonSizes.
            // (Assuming config.ButtonSizes is one of "Small", "Medium", or "Big".)
            VisualScale = config.General.ButtonSizes switch
            {
                "Small" => 0.45f,
                "Medium" => 0.7f,
                _ => 1.0f,
            };
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetInnerDimensions();
            if (CurrentImage != null && CurrentImage.Value != null)
            {
                float opacity = IsMouseHovering ? 1f : 0.4f;

                // Calculate the drawn size based on VisualScale.
                float drawWidth = CurrentImage.Value.Width * VisualScale;
                float drawHeight = CurrentImage.Value.Height * VisualScale;
                // Center the image within the UI element.
                Vector2 drawPos = dimensions.Position() + new Vector2((dimensions.Width - drawWidth) / 2f, (dimensions.Height - drawHeight) / 2f);
                spriteBatch.Draw(CurrentImage.Value, drawPos, null, Color.White * opacity, 0f, Vector2.Zero, VisualScale, SpriteEffects.None, 0f);
            }

            if (IsMouseHovering)
            {
                Config c = ModContent.GetInstance<Config>();
                if (c.General.ShowTooltips)
                    UICommon.TooltipMouseText(HoverText);
            }
        }

        // Abstract HandleClick = force children (all buttons) to implement this method
        public abstract void HandleClick();

    }
}