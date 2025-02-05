using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using ReLogic.Content;
using System.Linq;
using Terraria.GameContent;

namespace SquidTestingMod.UI
{
    public class ItemsPanel : UIPanel
    {
        public ItemsPanel()
        {
            // Initialize the panel.
            OnInitialize();
        }

        public override void OnInitialize()
        {
            // Configure the panel.
            SetPadding(10);
            Width.Set(300f, 0f);
            Height.Set(300f, 0f);
            HAlign = 0.5f;
            VAlign = 0.5f;
            BackgroundColor = new Color(63, 82, 151) * 0.8f;

            // Create a UIList to hold rows.
            UIList list = new UIList();
            list.Width.Set(0f, 1f);
            list.Height.Set(0f, 1f);
            list.ListPadding = 5f;
            Append(list);

            // Create and attach a scrollbar.
            UIScrollbar scrollbar = new UIScrollbar();
            scrollbar.HAlign = 1f;
            scrollbar.Height.Set(0f, 1f);
            Append(scrollbar);
            list.SetScrollbar(scrollbar);

            // We'll build the grid row‐by‐row.
            int itemsPerRow = 5;
            int currentItemInRow = 0;
            UIPanel rowPanel = CreateNewRowPanel();

            // Loop through all available item textures.
            // (TextureAssets.Item is an array of Asset<Texture2D> for every vanilla item.)
            for (int i = 1; i < TextureAssets.Item.Length; i++)
            {
                Asset<Texture2D> texture = TextureAssets.Item[i];
                if (texture == null)
                    continue;

                // Create an image button for this item.
                UIImageButton button = new UIImageButton(texture);
                button.Width.Set(50f, 0f);
                button.Height.Set(50f, 0f);
                // Manually position the button inside the row.
                float spacing = 5f;
                button.Left.Set((50f + spacing) * currentItemInRow, 0f);
                // Optionally, you can attach click handlers to 'button' here.
                rowPanel.Append(button);

                currentItemInRow++;
                // When we've added itemsPerRow buttons, add the row panel to the list.
                if (currentItemInRow >= itemsPerRow)
                {
                    list.Add(rowPanel);
                    // Reset for the next row.
                    rowPanel = CreateNewRowPanel();
                    currentItemInRow = 0;
                }
            }
            // If the last row is not empty, add it.
            if (rowPanel.Children.Any())
                list.Add(rowPanel);

            Recalculate();
        }

        /// <summary>
        /// Helper method to create a new row panel for a grid row.
        /// </summary>
        /// <returns>A UIPanel configured as a row container.</returns>
        private UIPanel CreateNewRowPanel()
        {
            UIPanel row = new UIPanel();
            row.SetPadding(0);
            row.Width.Set(0f, 1f);
            // Fix the row height to 50 (the height of our item buttons).
            row.Height.Set(50f, 0f);
            // Remove the background for rows so only the main panel’s background shows.
            row.BackgroundColor = Color.Transparent;
            return row;
        }
    }
}
