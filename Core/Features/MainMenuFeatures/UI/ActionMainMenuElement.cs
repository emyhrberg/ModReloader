using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModReloader.Core.Features.MainMenuFeatures.UI
{
    internal class ActionMainMenuElement : UIElement
    {
        private static readonly Color MainMenuWhite = new(237, 246, 255);
        private static readonly Color MainMenuGrey = new(173, 173, 198);

        public ActionMainMenuElement(Action action, string text, Func<string> tooltip, TooltipPanel tooltipPanel)
        {
            // pos
            Width.Set(0f, 1f);
            Height.Set(25f, 0f);

            // text ele
            var textElement = new UIText(text, textScale: 1.0f, large: false)
            {
                VAlign = 0.5f,
                HAlign = 0f,
                Left = { Pixels = 6f },
                Width = StyleDimension.Fill,
                Height = StyleDimension.Fill,
                TextColor = MainMenuGrey,
                TextOriginX = 0,
                TextOriginY = 0
            };
            Append(textElement);

            OnMouseOver += (_, _) =>
            {
                textElement.TextColor = MainMenuWhite;

                // Show tooltip
                tooltipPanel.Text = tooltip?.Invoke();
                tooltipPanel.Hidden = false;
            };

            OnMouseOut += (_, _) =>
            {
                textElement.TextColor = MainMenuGrey;

                // Hide tooltip
                tooltipPanel.Hidden = true;
            };

            OnLeftClick += (_, _) =>
            {
                action?.Invoke();
            };
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
