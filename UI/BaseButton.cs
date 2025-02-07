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

        public virtual void UpdateTexture()
        {
            Config config = ModContent.GetInstance<Config>();
            bool showText = config?.ShowButtonText ?? true;

            // Set the current image asset based on showText.
            CurrentImage = showText ? _buttonImgText : _buttonImgNoText;
            SetImage(CurrentImage);

            // Set VisualScale based on config.ButtonSizes.
            // (Assuming config.ButtonSizes is one of "Small", "Medium", or "Big".)
            switch (config.ButtonSizes)
            {
                case "Small":
                    VisualScale = 0.35f;
                    break;
                case "Medium":
                    VisualScale = 0.7f;
                    break;
                case "Big":
                default:
                    VisualScale = 1.0f;
                    break;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetInnerDimensions();
            if (CurrentImage != null && CurrentImage.Value != null)
            {
                // Calculate the drawn size based on VisualScale.
                float drawWidth = CurrentImage.Value.Width * VisualScale;
                float drawHeight = CurrentImage.Value.Height * VisualScale;
                // Center the image within the UI element.
                Vector2 drawPos = dimensions.Position() + new Vector2((dimensions.Width - drawWidth) / 2f, (dimensions.Height - drawHeight) / 2f);
                spriteBatch.Draw(CurrentImage.Value, drawPos, null, Color.White, 0f, Vector2.Zero, VisualScale, SpriteEffects.None, 0f);
            }

            if (IsMouseHovering)
            {
                Config c = ModContent.GetInstance<Config>();
                if (c.ShowTooltips)
                    UICommon.TooltipMouseText(HoverText);
            }
        }

        public override bool ContainsPoint(Vector2 point)
        {
            // Define a custom clickable area, e.g., make it 20 pixels bigger in all directions
            float extraSize = 20f;

            Rectangle customArea = new Rectangle(
                (int)(GetDimensions().X - extraSize),
                (int)(GetDimensions().Y - extraSize),
                (int)(Width.Pixels + extraSize * 2),
                (int)(Height.Pixels + extraSize * 2)
            );

            return customArea.Contains(point.ToPoint());
        }

        // Abstract HandleClick = force children (all buttons) to implement this method
        public abstract void HandleClick();

        #region dragging
        private bool DRAG_ENABLED = false;
        private bool dragging;
        private Vector2 dragOffset;
        // Track original mouse down position so we know if we moved
        private Vector2 mouseDownPos;
        private bool isDrag;
        private const float DragThreshold = 10f; // you can tweak the threshold
        // Hotfix click started outside the button
        private bool clickStartedOutsideButton;

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            Main.LocalPlayer.mouseInterface = true;

            if (!DRAG_ENABLED)
                return;

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
            Main.LocalPlayer.mouseInterface = false;

            if (!DRAG_ENABLED)
                return;
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
            if (!DRAG_ENABLED)
                return;
            base.Update(gameTime);

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

                MainSystem sys = ModContent.GetInstance<MainSystem>();

                // move all other buttons too
                if (sys?.mainState?.AreButtonsVisible ?? false)
                {
                    // ToggleButton
                    sys.mainState.toggleButton.Left.Set(Main.mouseX - dragOffset.X, 0f);
                    sys.mainState.toggleButton.Top.Set(Main.mouseY - dragOffset.Y, 0f);

                    // ConfigButton
                    sys.mainState.configButton.Left.Set(Main.mouseX - dragOffset.X + 100, 0f);
                    sys.mainState.configButton.Top.Set(Main.mouseY - dragOffset.Y, 0f);

                    // ItemsButton
                    sys.mainState.itemButton.Left.Set(Main.mouseX - dragOffset.X + 200, 0f);
                    sys.mainState.itemButton.Top.Set(Main.mouseY - dragOffset.Y, 0f);

                    // RefreshButton
                    sys.mainState.refreshButton.Left.Set(Main.mouseX - dragOffset.X + 300, 0f);
                    sys.mainState.refreshButton.Top.Set(Main.mouseY - dragOffset.Y, 0f);

                }

                Recalculate();
            }
        }
        #endregion
    }
}