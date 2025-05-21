using Microsoft.Xna.Framework.Graphics;
using ModReloader.Helpers;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace ModReloader.UI.Elements.ConfigElements
{
    /// <summary>
    /// A parent panel for config panels.
    /// </summary>
    public abstract class BasePanelConfig : UIPanel
    {
        public NestedUIList uiList;
        public UIScrollbar scrollbar;

        public BasePanelConfig(bool scrollbarEnabled = true)
        {
            // Panel settings
            Width.Set(-20, 1f);
            Height.Set(-40, 1f);
            Top.Set(0, 0);
            VAlign = 0f;
            HAlign = 0.5f;
            BackgroundColor = ColorHelper.SuperDarkBluePanel;

            // Create a new list
            uiList = new NestedUIList
            {
                Top = { Pixels = 0 },
                Width = { Percent = 1f },
                Height = { Percent = 1f },
                ManualSortMethod = (e) => { }
            };

            // Create a new scrollbar
            if (scrollbarEnabled)
            {
                scrollbar = new()
                {
                    Height = { Percent = 1f, Pixels = -10 },
                    HAlign = 1f,
                    VAlign = 0f,
                    Left = { Pixels = 5 },
                    Top = { Pixels = 0 },
                };
            }

            Append(uiList);

            // Set the scrollbar to the list
            if (scrollbarEnabled)
            {
                uiList.SetScrollbar(scrollbar);
                Append(scrollbar);
            }

            Recalculate();
        }

        public UIElement AddPadding(float padding)
        {
            // Create a basic UIElement to act as a spacer instead of using HeaderElement
            UIElement paddingElement = new();
            paddingElement.Height.Set(padding, 0f);
            paddingElement.Width.Set(0, 1f);
            uiList.Add(paddingElement);
            return paddingElement;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //uiList.Height.Set(0, 1);
            //uiList.MaxHeight.Set(0, 1);
            base.Draw(spriteBatch);
        }
    }
}
