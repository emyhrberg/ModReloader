using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A base class for panels for item and NPC spawner.
    /// This can be extended to add scrollbar, searchbox, itemcounts, itemgrid, etc.
    /// This class makes it easier to focus on only filter functions for the item/npc panels
    /// </summary>
    public abstract class SpawnerPanel : DraggablePanel
    {
        // Panel size
        private const int W = 530 + padding; // Width of the panel
        private const int H = 570; // Height of the panel

        // UI Elements
        protected CustomGrid ItemsGrid;
        protected UIScrollbar Scrollbar;
        protected CustomTextBox SearchTextBox;
        protected UIText ItemCountText;
        public CustomTextBox GetCustomTextBox() => SearchTextBox;

        // Store item slots
        protected List<CustomItemSlot> allItemSlots = [];

        public SpawnerPanel(string header) : base(header)
        {
            // Set the panel properties
            Width.Set(W, 0f);
            Height.Set(H, 0f);
            HAlign = 0.0f;
            VAlign = 1.0f;

            // Create all content in the panel
            ItemCountText = new CustomItemCountText("0 Items", textScale: 0.4f);

            SearchTextBox = new("Search")
            {
                Width = { Pixels = 200 },
                Height = { Pixels = 35 },
                HAlign = 1f,
                Top = { Pixels = -padding + 35 + padding },
                BackgroundColor = Color.White,
                BorderColor = Color.Black
            };

            Scrollbar = new UIScrollbar()
            {
                HAlign = 1f,
                Height = { Pixels = 440 - 10 }, // -10 because of scrollbarPadding=5 on top and bottom
                Width = { Pixels = 20 },
                Top = { Pixels = -padding + 30 + padding + 35 + padding + 5 },
                Left = { Pixels = 0f },
            };

            ItemsGrid = new CustomGrid()
            {
                Height = { Pixels = 440 },
                Width = { Percent = 1f, Pixels = -20 },
                ListPadding = 0f, // distance between items
                Top = { Pixels = -padding + 30 + padding + 35 + padding },
                Left = { Pixels = 0f },
                OverflowHidden = true, // hide items outside the grid
            };
            ItemsGrid.ManualSortMethod = (listUIElement) => { };
            ItemsGrid.SetScrollbar(Scrollbar);

            // Add all content in the panel
            Append(ItemCountText);
            Append(SearchTextBox);
            Append(Scrollbar);
            Append(ItemsGrid);
        }
    }
}