using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Configs;
using ModReloader.Common.Systems;
using ModReloader.UI.Elements.PanelElements;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModReloader.UI.Elements.ButtonElements
{
    /// <summary>
    /// The base class for the main buttons that are displayed.
    /// This class handles the general button logic, such as drawing the button, animations, and tooltip text.
    /// </summary>
    public abstract class BaseButton : UIImageButton
    {
        // General variables for a button
        protected Asset<Texture2D> Button;
        protected Asset<Texture2D> ButtonHighlight;
        protected Asset<Texture2D> ButtonNoOutline;
        protected Asset<Texture2D> Spritesheet { get; set; }
        public string HoverText = "";
        public string HoverTextDescription;
        protected float opacity = 0.8f;
        public ButtonText ButtonText;
        public bool Active = true;
        public bool ParentActive = false;

        // Animation frames
        protected int currFrame = 1; // the current frame
        protected int frameCounter = 0; // the counter for the frame speed

        // Associated panel for closing and managing multiple panels
        public BasePanel AssociatedPanel { get; set; } = null; // the panel associated with this button

        // Animations variables to be set by child classes. virtual means it can be overridden by child classes.
        protected virtual float BaseAnimScale => 1f;    // default
        private float Scale
            => BaseAnimScale
               * ModContent.GetInstance<MainSystem>().mainState.UIScale;
        protected virtual int StartFrame => 1;
        protected virtual int FrameCount => 1;
        protected virtual int FrameSpeed => 0; // the speed of the animation, lower is faster
        protected abstract int FrameWidth { get; } // abstract means force child classes to implement this
        protected abstract int FrameHeight { get; } // abstract means force child classes to implement this
        public float TextScale; // the scale of the text, set by  
        /// <see cref="MainState"/> 

        // Constructor
        #region Constructor
        protected BaseButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription = "") : base(spritesheet)
        {
            Button = Ass.Button;
            ButtonHighlight = Ass.ButtonHighlight;
            ButtonNoOutline = Ass.ButtonNoOutline;
            Spritesheet = spritesheet;
            HoverText = hoverText;
            HoverTextDescription = hoverTextDescription;
            SetImage(Button);
            currFrame = StartFrame;

            // Set textScale based on buttonscale.
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            var state = sys.mainState;

            // 1) drive size
            Width.Set(state.ButtonSize, 0f);
            Height.Set(state.ButtonSize, 0f);

            // 2) center and bottom-align
            VAlign = 1f;
            HAlign = 0.5f;

            // Add a UIText centered horizontally at the bottom of the button.
            // Set the scale; 70f seems to fit to 0.9f scale.
            ButtonText = new ButtonText(text: buttonText, textScale: 0.9f, large: false);
            Append(ButtonText);
        }
        #endregion

        public void UpdateHoverTextDescription()
        {
            // Based on ModsToReload, make the hovertext.
            string modsToReload = string.Join(", ", Conf.C.ModsToReload);

            if (string.IsNullOrEmpty(modsToReload))
            {
                modsToReload = "No mods to reload";
            }

            HoverTextDescription = $"{modsToReload}";
        }

        /// <summary>
        /// Draws the button with the specified image/animation and tooltip text
        /// </summary>
        #region Draw
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!Active || Button == null || Button.Value == null)
                return;

            // Get the button size from MainState (default to 70 if MainState is null)
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            // Update the scale based on the buttonsize. 70f means a scale of 1. For every 10 pixels, the scale is increased by 0.1f.
            // Get the dimensions based on the button size.
            CalculatedStyle dimensions = GetInnerDimensions();
            var state = ModContent.GetInstance<MainSystem>().mainState;
            float s = state.ButtonSize;
            Rectangle drawRect = new(
                (int)dimensions.X, (int)dimensions.Y,
                (int)s, (int)s
            );

            // Set the opacity based on mouse hover.
            opacity = IsMouseHovering ? 1f : 0.9f; // Determine opacity based on mouse hover.

            // Set UIText opacity
            ButtonText.TextColor = Color.White;

            // Draw the button with full opacity.
            spriteBatch.Draw(Button.Value, drawRect, Color.White);

            if (IsMouseHovering)
            {
                spriteBatch.Draw(ButtonNoOutline.Value, drawRect, Color.Black * 0.3f);
            }

            if (ParentActive)
            {
                // Scale down the highlight and center it
                float scale = 0.9f; // Scale factor for the highlight
                Rectangle scaledRect = new Rectangle(
                    (int)(drawRect.X + drawRect.Width * (1 - scale) / 2),
                    (int)(drawRect.Y + drawRect.Height * (1 - scale) / 2),
                    (int)(drawRect.Width * scale),
                    (int)(drawRect.Height * scale)
                );
                spriteBatch.Draw(ButtonHighlight.Value, scaledRect, Color.White * 0.7f);
            }

            // Draw the animation texture
            if (Spritesheet != null)
            {
                if (IsMouseHovering)
                {
                    frameCounter++;
                    if (frameCounter >= FrameSpeed)
                    {
                        currFrame++;
                        if (currFrame > FrameCount)
                            currFrame = StartFrame;
                        frameCounter = 0;
                    }
                }
                else
                {
                    currFrame = StartFrame;
                    frameCounter = 0;
                }

                // Use a custom scale and offset to draw the animated overlay.
                Vector2 position = dimensions.Position();
                Vector2 size = new(dimensions.Width, dimensions.Height);
                Vector2 centeredPosition = position + (size - new Vector2(FrameWidth, FrameHeight) * Scale) / 2f;
                Rectangle sourceRectangle = new(x: 0, y: (currFrame - 1) * FrameHeight, FrameWidth, FrameHeight);
                centeredPosition.Y -= 7; // magic offset to move it up a bit

                // Draw the spritesheet.
                spriteBatch.Draw(Spritesheet.Value, centeredPosition, sourceRectangle, Color.White * opacity, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
            }
        }
        #endregion

        //Disable button click if config window is open
        public override bool ContainsPoint(Vector2 point)
        {
            if (!Active) // Dont allow clicking if button is disabled.
                return false;

            return base.ContainsPoint(point);
        }

        #region LeftClick
        public override void LeftClick(UIMouseEvent evt)
        {
            // If buttons not showing, return
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (!sys.mainState.AreButtonsShowing) return;

            if (AssociatedPanel is null) return;

            if (AssociatedPanel.GetActive())
            {
                AssociatedPanel.SetActive(false);
                ParentActive = false;
            }
            else
            {
                ParentActive = true;
                AssociatedPanel.SetActive(true);

                // bring to front …
                if (AssociatedPanel.Parent is not null)
                {
                    UIElement parent = AssociatedPanel.Parent;
                    AssociatedPanel.Remove();
                    parent.Append(AssociatedPanel);
                }
            }
        }
        #endregion

        // Disable item use on click
        public override void Update(GameTime gameTime)
        {
            // base update
            base.Update(gameTime);

            // disable item use if the button is hovered
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            // center the buttontext
            //ButtonText.HAlign = 0.5f;
            //ButtonText.VAlign = 0.85f;
        }
    }
}