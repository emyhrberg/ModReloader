using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel containing a grid of all items in the game that can be spawned.
    /// </summary>
    public class ItemSpawner : SpawnerPanel
    {

        // Filter items
        FilterButton all;

        private enum ItemFilter
        {
            All,
            AllWeapons,
            Melee,
            Ranged,
            Magic,
            Summon
        }

        private ItemFilter currentFilter = ItemFilter.All;

        public ItemSpawner() : base("Item Spawner")
        {
            // Add filter buttons
            all = AddFilterButton(Assets.FilterAll, "Filter All", ItemFilter.All, 0);
            AddFilterButton(Assets.FilterAll, "Filter All", ItemFilter.All, 0);
            AddFilterButton(Assets.FilterMelee, "Filter All Weapons", ItemFilter.AllWeapons, 25);
            AddFilterButton(Assets.FilterMelee, "Filter Melee Weapons", ItemFilter.Melee, 50);
            AddFilterButton(Assets.FilterRanged, "Filter Ranged Weapons", ItemFilter.Ranged, 75);
            AddFilterButton(Assets.FilterMagic, "Filter Magic Weapons", ItemFilter.Magic, 100);
            AddFilterButton(Assets.FilterSummon, "Filter Summon Weapons", ItemFilter.Summon, 125);

            // Add items to the grid
            AddItemSlotsToGrid();

            // Set all to active
            Log.Info("Done setting up ItemSpawnerPanel");
        }

        public override void OnInitialize()
        {
            ItemFilterClicked(ItemFilter.All, all);
            Log.Info("ItemFilter Clicked on All initialized");
        }

        private FilterButton AddFilterButton(Asset<Texture2D> texture, string hoverText, ItemFilter filter, float left)
        {
            FilterButton button = new FilterButton(texture, hoverText);
            button.Left.Set(left, 0);
            button.OnLeftClick += (evt, element) => ItemFilterClicked(filter, button);
            Append(button);
            return button;
        }

        private void ItemFilterClicked(ItemFilter filter, FilterButton button)
        {
            // Set current filter to active
            FilterButton.ActiveButton = button;

            currentFilter = filter;
            FilterItems();
        }

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

        protected override void FilterItems()
        {

            string searchText = SearchTextBox.currentString.ToLower();
            ItemsGrid.Clear();
            Stopwatch s = Stopwatch.StartNew();

            foreach (var itemSlot in allItemSlots)
            {
                Item item = itemSlot.GetDisplayItem();

                // Check the search text filter.
                if (!item.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                // Determine if the item passes the current filter.
                bool passesFilter = false;
                switch (currentFilter)
                {
                    case ItemFilter.All:
                        passesFilter = true;
                        break;
                    case ItemFilter.AllWeapons:
                        passesFilter = item.damage > 0;
                        break;
                    case ItemFilter.Melee:
                        passesFilter = item.damage > 0 && item.DamageType == DamageClass.Melee;
                        break;
                    case ItemFilter.Ranged:
                        passesFilter = item.damage > 0 && item.DamageType == DamageClass.Ranged;
                        break;
                    case ItemFilter.Magic:
                        passesFilter = item.damage > 0 && item.DamageType == DamageClass.Magic;
                        break;
                    case ItemFilter.Summon:
                        passesFilter = item.damage > 0 && item.DamageType == DamageClass.Summon;
                        break;
                }

                if (passesFilter)
                {
                    ItemsGrid.Add(itemSlot);
                }
            }

            s.Stop();
            ItemCountText.SetText($"{ItemsGrid.Count} Items in {Math.Round(s.ElapsedMilliseconds / 1000.0, 3)} seconds");
        }
    }
}