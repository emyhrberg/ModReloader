using System;
using System.Reflection;
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
    public abstract class BaseButton : UIImageButton
    {
        protected Asset<Texture2D> _Texture;
        public string TooltipText = "";
        public float RelativeLeftOffset = 0f;
        public bool Active = true;
        protected float opacity = 0.4f;

        protected BaseButton(Asset<Texture2D> texture, string hoverText) : base(texture)
        {
            _Texture = texture;
            TooltipText = hoverText;
            SetImage(_Texture);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!Active)
                return;

            if (_Texture != null && _Texture.Value != null)
            {
                // Get the forced button size from MainState (default to 70 if not set)
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                float buttonSize = sys?.mainState?.ButtonSize ?? 70f;

                // Determine opacity based on mouse hover.
                opacity = IsMouseHovering ? 1f : 0.4f;

                // Get the dimensions based on the button size.
                CalculatedStyle dimensions = GetInnerDimensions();
                Rectangle drawRect = new((int)dimensions.X, (int)dimensions.Y, (int)buttonSize, (int)buttonSize);

                // Draw the texture with the calculated opacity.
                spriteBatch.Draw(_Texture.Value, drawRect, Color.White * opacity);
            }

            // Draw tooltip text if hovering.
            if (IsMouseHovering)
                UICommon.TooltipMouseText(TooltipText);
        }

        public virtual void UpdateTexture()
        {
            SetImage(_Texture);
        }

        #region Disable button click if config window is open
        public override bool ContainsPoint(Vector2 point)
        {
            if (!Active) // Dont allow clicking if button is disabled.
                return false;

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

            return base.ContainsPoint(point);
        }
        #endregion

        #region Disable item use on button click
        public override void LeftMouseDown(UIMouseEvent evt)
        {
            Main.LocalPlayer.mouseInterface = true;
            base.LeftMouseDown(evt);
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            Main.LocalPlayer.mouseInterface = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
        #endregion

        #region Disable hover effects (sound, opacity change when hovering)
        public override void MouseOver(UIMouseEvent evt)
        {
            if (!Conf.HoverEffectButtons)
                return;

            base.MouseOver(evt);
        }
        #endregion
    }
}