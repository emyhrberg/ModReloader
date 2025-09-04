using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModReloader.Core.Features.MainMenuFeatures.UI
{
    internal class HeaderMainMenuElement : UIElement
    {
        private UIText textElement;
        private static readonly Color HeaderMainMenuWhite = new(237, 246, 255);

        public HeaderMainMenuElement(string text, Func<string> tooltip, TooltipPanel tooltipPanel)
        {
            // pos
            Width.Set(0f, 1f);
            Height.Set(25f, 0f);

            // text ele
            textElement = new UIText(text, textScale: 1.15f, large: false)
            {
                VAlign = 0.5f,
                HAlign = 0f,
                Left = { Pixels = 6f },
                Width = StyleDimension.Fill,
                Height = StyleDimension.Fill,
                TextColor = HeaderMainMenuWhite,
                TextOriginX = 0,
                TextOriginY = 0
            };
            Append(textElement);

            OnMouseOver += (_, _) =>
            {
                if (tooltip != null)
                {
                    tooltipPanel.Text = tooltip?.Invoke();
                    tooltipPanel.Hidden = false;
                }
            };
            OnMouseOut += (_, _) =>
            {
                // Hide tooltip
                tooltipPanel.Hidden = true;
            };
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
