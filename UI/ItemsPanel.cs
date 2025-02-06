using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ItemsPanel : UIPanel
    {
        private UIGrid _itemsGrid;
        private UITextBox _searchBox;
        private UIScrollbar _scrollbar;

        private const float PanelWidth = 300f;
        private const float PanelHeight = 400f;

        public ItemsPanel()
        {
            // Usually, you do NOT call OnInitialize() in the constructor;
            // the UI system calls it for you when this panel is activated/added.
        }

        public override void OnInitialize()
        {
            // Configure main panel
            SetPadding(6f); // Add some padding for scrollbar
            Width.Set(300f, 0f);
            Height.Set(400f, 0f); // Increased height to accommodate search bar
            HAlign = 0.5f;
            VAlign = 0.5f;
            BackgroundColor = new Color(63, 82, 151) * 0.8f;

            // Create search bar
            _searchBox = new UITextBox("", 0.8f)
            {
                Width = new StyleDimension(-60f, 1f), // Leave space for the search button
                Height = new StyleDimension(30f, 0f),
                HAlign = 0f,
                VAlign = 0f,
                PaddingTop = 4f,
                PaddingBottom = 4f,
                PaddingLeft = 4f,
                PaddingRight = 4f
            };
            _searchBox.OnLeftClick += (evt, element) => _searchBox. = true;
            Append(_searchBox);

            // Create grid that will hold our items
            _itemsGrid = new UIGrid
            {
                Width = new StyleDimension(-20f, 1f), // Account for scrollbar width
                Height = new StyleDimension(-40f, 1f), // Leave space for search bar
                Top = new StyleDimension(40f, 0f), // Position below search bar
                ListPadding = 4f,
                OverflowHidden = true
            };
            Append(_itemsGrid);

            // Configure scrollbar
            UIScrollbar scrollbar = new UIScrollbar
            {
                Height = new StyleDimension(-12f, 1f),
                HAlign = 1f,
                VAlign = 0.5f
            };
            Append(scrollbar);
            _itemsGrid.SetScrollbar(scrollbar);

            // Populate grid with items
            PopulateItemsGrid();

            // Force proper layout
            _itemsGrid.UpdateOrder();
            Recalculate();
        }

        private void PopulateItemsGrid()
        {
            _itemsGrid.Clear();
            int totalItems = TextureAssets.Item.Length - 1;
            const float itemSize = 50f;
            for (int i = 1; i <= totalItems; i++)
            {
                var itemPanel = CreateItemPanel(i, itemSize);
                _itemsGrid.Add(itemPanel);
            }
            _itemsGrid.UpdateOrder();
        }

        private UIPanel CreateItemPanel(int itemID, float size)
        {
            UIPanel panel = new UIPanel
            {
                Width = new StyleDimension(size, 0f),
                Height = new StyleDimension(size, 0f),
                BackgroundColor = new Color(40, 33, 82),
                PaddingTop = 0f,
                PaddingBottom = 0f,
                PaddingLeft = 0f,
                PaddingRight = 0f
            };

            try
            {
                Main.instance.LoadItem(itemID);
                Texture2D texture = TextureAssets.Item[itemID].Value;

                UIImage img = new UIImage(texture)
                {
                    HAlign = 0.5f,
                    VAlign = 0.5f,
                    ScaleToFit = true,
                    MaxWidth = new StyleDimension(size - 8f, 0f),
                    MaxHeight = new StyleDimension(size - 8f, 0f)
                };
                panel.Append(img);
            }
            catch (Exception ex)
            {
                ModContent.GetInstance<SquidTestingMod>()
                    .Logger.Warn($"Could not load item {itemID}: {ex.Message}");
            }

            // Setup interactions
            Item dummyItem = new();
            dummyItem.SetDefaults(itemID);

            panel.OnMouseOver += (_, _) => UICommon.TooltipMouseText(dummyItem.Name);
            panel.OnLeftClick += (_, _) => SafeClick(dummyItem);

            return panel;
        }

        /// <summary>
        /// Called whenever the text in the search box changes, 
        /// e.g. each character typed or deleted.
        /// </summary>
        private void FilterItems(string searchTerm)
        {
            _itemsGrid.Clear();
            const float itemSize = 50f;
            int totalItems = TextureAssets.Item.Length - 1;

            for (int i = 1; i <= totalItems; i++)
            {
                Item dummyItem = new();
                dummyItem.SetDefaults(i);

                if (dummyItem.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    var itemPanel = CreateItemPanel(i, itemSize);
                    _itemsGrid.Add(itemPanel);
                }
            }

            _itemsGrid.UpdateOrder();
            Recalculate();
        }

        public void SafeClick(Item item)
        {
            if (Main.keyState.PressingShift())
            {
                Main.LocalPlayer.QuickSpawnItem(
                    Main.LocalPlayer.GetSource_FromThis(),
                    item.type,
                    9999
                );
            }
            else if (Main.mouseItem.IsAir)
            {
                if (!Main.playerInventory)
                    Main.playerInventory = true;

                Main.mouseItem = item.Clone();

                if (!ItemID.Sets.Deprecated[item.type])
                    Main.mouseItem.SetDefaults(item.type);

                Main.mouseItem.stack = 9999;
            }
        }
    }
}
