using Microsoft.Xna.Framework;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using ReLogic.Content;

namespace SquidTestingMod.UI
{
    public class ItemsPanel : UIPanel
    {
        public ItemsPanel()
        {
            OnInitialize();
        }

        public override void OnInitialize()
        {
            // Set up the main panel
            SetPadding(0);
            Width.Set(300f, 0f);
            Height.Set(300f, 0f);
            HAlign = 0.5f;
            VAlign = 0.5f;
            BackgroundColor = new Color(63, 82, 151) * 0.8f;

            int itemsPerRow = 5;
            float itemSize = 50f;
            int totalRows = 20; // 20 rows * 5 items = 100 panels

            // Create a UIList to contain row panels (each row panel holds 5 items)
            UIList list = new UIList();
            list.Width.Set(0f, 1f);
            list.Height.Set(0f, 1f);
            list.ListPadding = 0;
            Append(list);

            // Create and attach a scrollbar
            UIScrollbar scrollbar = new UIScrollbar();
            scrollbar.HAlign = 1f;
            scrollbar.Height.Set(0f, 1f);
            Append(scrollbar);
            list.SetScrollbar(scrollbar);

            // Build rows manually without a separate row container class
            for (int row = 0; row < totalRows; row++)
            {
                UIPanel rowPanel = new UIPanel();
                rowPanel.SetPadding(0);
                rowPanel.Width.Set(0f, 1f);
                rowPanel.Height.Set(itemSize, 0f);
                rowPanel.BackgroundColor = Color.Transparent;
                rowPanel.BorderColor = Color.Transparent;

                for (int col = 0; col < itemsPerRow; col++)
                {
                    UIPanel itemPanel = new UIPanel();
                    itemPanel.SetPadding(0);
                    itemPanel.Width.Set(itemSize, 0f);
                    itemPanel.Height.Set(itemSize, 0f);
                    itemPanel.BackgroundColor = new Color(40, 33, 82); // blueish inventory-like color
                    // Manually position each item in the row
                    itemPanel.Left.Set(col * itemSize, 0f);
                    rowPanel.Append(itemPanel);
                }
                list.Add(rowPanel);
            }

            Recalculate();
        }
    }
}
