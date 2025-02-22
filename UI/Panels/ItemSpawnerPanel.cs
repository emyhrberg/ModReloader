using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel containing a grid of all items in the game that can be spawned.
    /// </summary>
    public class ItemSpawnerPanel : UIPanel
    {
        // Panel values
        private bool Active = false; // true for visible and update, false for not visible and not update
        private const int padding = 12;
        private const int W = 530 + padding; // Width of the panel
        private const int H = 570; // Height of the panel

        // Colors
        private Color lightBlue = new(63, 82, 151);
        private Color darkBlue = new(73, 85, 186);

        // UI Elements
        private CustomGrid ItemsGrid;
        private UIScrollbar Scrollbar;
        private UIPanel CloseButtonPanel;
        private UIPanel TitlePanel;
        private CustomTextBox SearchTextBox;
        private UIText ItemCountText;
        private UIText HeaderText;
        public CustomTextBox GetCustomTextBox() => SearchTextBox;

        // Store item slots
        private List<CustomItemSlot> allItemSlots = [];

        // Add this field at the top of your class (outside any function)
        private Func<Item, bool> currentCategoryPredicate = item => true;

        #region Constructor
        public ItemSpawnerPanel()
        {
            // Set the panel properties
            Width.Set(W, 0f);
            Height.Set(H, 0f);
            HAlign = 0.0f;
            // Left.Set(pixels: 20f, precent: 0.0f);
            // VAlign = 0.9f;
            VAlign = 1.0f;
            BackgroundColor = lightBlue;

            // Create all content in the panel
            TitlePanel = new CustomTitlePanel(padding: padding, bgColor: darkBlue, height: 35);
            HeaderText = new UIText(text: "Item Spawner", textScale: 0.4f, large: true);
            CloseButtonPanel = new CloseButtonPanel();
            ItemCountText = new CustomItemCountText("0 Items", textScale: 0.4f);

            SearchTextBox = new("Search for items")
            {
                Width = { Pixels = 200 },
                Height = { Pixels = 35 },
                HAlign = 1f,
                Top = { Pixels = -padding + 35 + padding },
                BackgroundColor = Color.White,
                BorderColor = Color.Black
            };
            SearchTextBox.OnTextChanged += FilterItems;

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

            // Filter All Items (5000+ items)
            BaseFilterButton all = new(Assets.FilterAll, "Filter All");
            all.Left.Set(0, 0);
            all.OnLeftClick += (evt, element) =>
            {
                currentCategoryPredicate = item => true;
                FilterItems();
            };
            Append(all);

            // Filter All Weapons (no damage class filter)
            BaseFilterButton allWeps = new(Assets.FilterMelee, "Filter All Weapons");
            allWeps.Left.Set(25, 0);
            allWeps.OnLeftClick += (evt, element) => FilterItemsByType(null);
            Append(allWeps);

            // Filter Melee Weapons
            BaseFilterButton melee = new(Assets.FilterMelee, "Filter Melee Weapons");
            melee.Left.Set(50, 0);
            melee.OnLeftClick += (evt, element) => FilterItemsByType(DamageClass.Melee);
            Append(melee);

            // Filter Ranged Weapons
            BaseFilterButton ranged = new(Assets.FilterRanged, "Filter Ranged Weapons");
            ranged.Left.Set(75, 0);
            ranged.OnLeftClick += (evt, element) => FilterItemsByType(DamageClass.Ranged);
            Append(ranged);

            // Filter Magic Weapons
            BaseFilterButton magic = new(Assets.FilterMagic, "Filter Magic Weapons");
            magic.Left.Set(100, 0);
            magic.OnLeftClick += (evt, element) => FilterItemsByType(DamageClass.Magic);
            Append(magic);

            // Filter Summon Weapons
            BaseFilterButton summon = new(Assets.FilterSummon, "Filter Summon Weapons");
            summon.Left.Set(125, 0);
            summon.OnLeftClick += (evt, element) => FilterItemsByType(DamageClass.Summon);
            Append(summon);

            // Add all content in the panel
            Append(TitlePanel);
            Append(HeaderText);
            Append(CloseButtonPanel);
            Append(ItemCountText);
            Append(SearchTextBox);
            Append(Scrollbar);
            Append(ItemsGrid);

            // Add items to the grid
            AddItemSlotsToGrid();
        }
        #endregion

        #region FilterItemsByType
        private void FilterItemsByType(DamageClass damageClass = null)
        {
            // If damageClass is null, assume "All Weapons" button was clicked.
            // (For "All Items", you can directly call FilterItems or create a dedicated button that sets the predicate to always true.)
            if (damageClass == null)
            {
                currentCategoryPredicate = item => item.damage > 0;
            }
            else
            {
                currentCategoryPredicate = item => item.damage > 0 && item.DamageType == damageClass;
            }
            // Reapply the search text filter with the new category filter.
            FilterItems();
        }
        #endregion

        #region Adding initial slots

        private void AddItemSlotsToGrid()
        {
            int allItems = TextureAssets.Item.Length - 1;
            int count = 0;

            Stopwatch s = Stopwatch.StartNew();

            for (int i = 1; i <= allItems; i++)
            {
                Item item = new();
                item.SetDefaults(i);
                CustomItemSlot itemSlot = new([item], 0, ItemSlot.Context.ChestItem);
                allItemSlots.Add(itemSlot);

                count++;
                if (count >= Conf.MaxItemsToDisplay)
                    break;
            }

            foreach (var itemSlot in allItemSlots)
            {
                ItemsGrid.Add(itemSlot);
            }

            // update item count text
            s.Stop();
            ItemCountText.SetText(ItemsGrid.Count + " Items" + " in " + Math.Round(s.ElapsedMilliseconds / 1000.0, 3) + " seconds");
        }

        #endregion

        #region FilterItems
        private void FilterItems()
        {
            string searchText = SearchTextBox.currentString.ToLower();
            ItemsGrid.Clear();
            Stopwatch s = Stopwatch.StartNew();
            foreach (var itemSlot in allItemSlots)
            {
                Item item = itemSlot.GetDisplayItem();
                // Apply both the search filter and the current category filter.
                if (item.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase) &&
                    currentCategoryPredicate(item))
                {
                    ItemsGrid.Add(itemSlot);
                }
            }
            s.Stop();
            ItemCountText.SetText($"{ItemsGrid.Count} Items in {Math.Round(s.ElapsedMilliseconds / 1000.0, 3)} seconds");
        }
        #endregion

        #region dragging and update
        public bool IsDraggingItemPanel;
        private bool dragging;
        private Vector2 dragOffset;
        private const float DragThreshold = 3f; // very low threshold for dragging
        private Vector2 mouseDownPos;

        public override bool ContainsPoint(Vector2 point)
        {
            if (!Active)
                return false;

            return base.ContainsPoint(point);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Active = false;
                Main.playerInventory = false;
                return;
            }

            base.Update(gameTime);

            if (dragging)
            {
                float dragDistance = Vector2.Distance(new Vector2(Main.mouseX, Main.mouseY), mouseDownPos);
                if (dragDistance > DragThreshold)
                {
                    IsDraggingItemPanel = true;
                    Left.Set(Main.mouseX - dragOffset.X, 0f);
                    Top.Set(Main.mouseY - dragOffset.Y, 0f);
                    Recalculate();
                    Main.LocalPlayer.mouseInterface = true;
                }
            }
            else
            {
                IsDraggingItemPanel = false;
            }

            if (dragging || ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (!Active || (Scrollbar != null && Scrollbar.ContainsPoint(evt.MousePosition)))
                return;

            mouseDownPos = evt.MousePosition;
            base.LeftMouseDown(evt);
            dragging = true;
            IsDraggingItemPanel = false;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
            Main.LocalPlayer.mouseInterface = true;
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false;
            IsDraggingItemPanel = false;
            Main.LocalPlayer.mouseInterface = false;
            Recalculate();
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (IsDraggingItemPanel)
                return;
            base.LeftClick(evt);
            Main.LocalPlayer.mouseInterface = true;
        }
        #endregion

        #region toggle visibility
        // also see update() for more visibility toggling
        // we modify both update() and draw() when active is false
        public bool GetActive() => Active;
        public bool SetActive(bool active) => Active = active;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
                return;
            base.Draw(spriteBatch);
        }
        #endregion

        #region usefulcode
        // https://github.com/ScalarVector1/DragonLens/blob/1b2ca47a5a4d770b256fdffd5dc68c0b4d32d3b2/Content/Tools/Spawners/ItemSpawner.cs#L14
        #endregion
    }
}