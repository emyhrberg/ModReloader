using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Common.Systems;
using ModHelper.Helpers;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.UI.Elements
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
        protected Asset<Texture2D> Image;
        public string HoverText = "";
        public string HoverTextDescription;
        protected float opacity = 0.8f;
        public ButtonText ButtonText;
        public bool Active = true;
        public bool ParentActive = false;

        // Associated panel for closing and managing multiple panels
        public BasePanel AssociatedPanel { get; set; } = null; // the panel associated with this button

        #region Constructor
        protected BaseButton(Asset<Texture2D> image, string buttonText, string hoverText, string hoverTextDescription = "") : base(image)
        {
            Image = image;
            Button = Ass.Button;
            ButtonHighlight = Ass.ButtonHighlight;
            ButtonNoOutline = Ass.ButtonNoOutline;
            HoverText = hoverText;
            HoverTextDescription = hoverTextDescription;
            SetImage(Button);

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            float TextScale = sys?.mainState?.TextSize ?? 0.9f;

            // Add a UIText centered horizontally at the bottom of the button.
            // Set the scale; 70f seems to fit to 0.9f scale.
            ButtonText = new(text: buttonText, textScale: TextScale, large: false)
            {
                HAlign = 0.5f,
                VAlign = 0.85f
            };
            Append(ButtonText);
        }
        #endregion

        #region Draw
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (!Active || Button?.Value == null || Image?.Value == null)
                return;

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            float btnPx = sys.mainState?.ButtonSize ?? 100f;          // your button square
            if (!sys.mainState.AreButtonsShowing) return;

            CalculatedStyle dims = GetInnerDimensions();
            Rectangle bgRect = new((int)dims.X, (int)dims.Y, (int)btnPx, (int)btnPx);

            // background & outline
            spriteBatch.Draw(Button.Value, bgRect, Color.White);
            if (IsMouseHovering)
                spriteBatch.Draw(ButtonNoOutline.Value, bgRect, Color.Black * 0.3f);
            if (ParentActive)
                spriteBatch.Draw(ButtonHighlight.Value, bgRect, Color.White * 0.7f);

            // raw‑size icon, centred (no scaling applied)
            int texW = Image.Value.Width;
            int texH = Image.Value.Height;

            Vector2 iconPos = new Vector2(
                dims.X + (btnPx - texW) / 2f,
                dims.Y + (btnPx - texH) / 2f - 2f        // optional tiny bump up
            );

            spriteBatch.Draw(
                Image.Value,
                position: iconPos,
                color: Color.White * (IsMouseHovering ? 1f : 0.9f)
            );
        }
        #endregion

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

        // Disable item use on click
        public override void Update(GameTime gameTime)
        {
            if (!Active)
            {
                return;
            }

            // base update
            base.Update(gameTime);

            // disable item use if the button is hovered
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

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
    }
}