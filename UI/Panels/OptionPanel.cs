using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    public class OptionPanel : UIPanel
    {
        // Variables
        private int padding = 12;
        private string hoverText;
        private CustomCheckbox checkbox;
        private int HEIGHT = 30;
        private UIText textElement;

        public OptionPanel(string text, string hoverText, bool hasCheckbox, Color bgColor)
        {
            MaxWidth.Set(padding * 2, 1f);
            Width.Set(padding * 2, 1f);
            Height.Set(HEIGHT, 0f);
            Left.Set(-padding, 0f);
            BackgroundColor = bgColor;
            this.hoverText = hoverText ?? "";

            if (hasCheckbox)
            {
                checkbox = new CustomCheckbox();
                checkbox.Top.Set((Height.Pixels - checkbox.Height.Pixels) / 2 - 15, 0f);
                Append(checkbox);
            }

            textElement = new UIText(text, 0.4f, true);
            // If there's a checkbox, leave enough space on the left; otherwise, use a smaller left offset.
            textElement.Left.Set(hasCheckbox ? 30 : 10, 0f);
            // Center the text vertically.
            textElement.Top.Set((Height.Pixels - textElement.MinHeight.Pixels) / 2 - 12, 0f);
            Append(textElement);
        }

        public void SetText(string text)
        {
            textElement.SetText(text);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            BorderColor = Color.Yellow;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            base.MouseOut(evt);
            BorderColor = Color.Black;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Draw hover text
            if (IsMouseHovering)
            {
                UICommon.TooltipMouseText(hoverText);
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // handle regular click event like acting on its event
            base.LeftClick(evt);

            if (checkbox != null)
            {
                checkbox.Toggle();
            }
        }
    }
}