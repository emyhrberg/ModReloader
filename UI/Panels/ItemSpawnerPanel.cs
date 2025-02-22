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
    public class ItemSpawnerPanel : SpawnerPanel
    {

        // Filter items
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

        #region Constructor
        public ItemSpawnerPanel() : base("Item Spawner")
        {
            // For "All"
            BaseFilterButton all = new(Assets.FilterAll, "Filter All");
            all.Left.Set(0, 0);
            all.OnLeftClick += (evt, element) => FilterItemsByType(ItemFilter.All);
            Append(all);

            // For "All Weapons":
            BaseFilterButton allWeps = new(Assets.FilterMelee, "Filter All Weapons");
            allWeps.Left.Set(25, 0);
            allWeps.OnLeftClick += (evt, element) => FilterItemsByType(ItemFilter.AllWeapons);
            Append(allWeps);

            // For "Melee Weapons":
            BaseFilterButton melee = new(Assets.FilterMelee, "Filter Melee Weapons");
            melee.Left.Set(50, 0);
            melee.OnLeftClick += (evt, element) => FilterItemsByType(ItemFilter.Melee);
            Append(melee);
            // (and similarly for Ranged, Magic, and Summon)

            // For "Ranged Weapons":
            BaseFilterButton ranged = new(Assets.FilterRanged, "Filter Ranged Weapons");
            ranged.Left.Set(75, 0);
            ranged.OnLeftClick += (evt, element) => FilterItemsByType(ItemFilter.Ranged);
            Append(ranged);

            // For "Magic Weapons":
            BaseFilterButton magic = new(Assets.FilterMagic, "Filter Magic Weapons");
            magic.Left.Set(100, 0);
            magic.OnLeftClick += (evt, element) => FilterItemsByType(ItemFilter.Magic);
            Append(magic);

            // For "Summon Weapons":
            BaseFilterButton summon = new(Assets.FilterSummon, "Filter Summon Weapons");
            summon.Left.Set(125, 0);
            summon.OnLeftClick += (evt, element) => FilterItemsByType(ItemFilter.Summon);
            Append(summon);

            // Setup searchtextbox
            SearchTextBox.OnTextChanged += FilterItems;

            // Add items to the grid
            AddItemSlotsToGrid();
        }
        #endregion

        private void FilterItemsByType(ItemFilter filter)
        {
            currentFilter = filter;
            FilterItems();
        }

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
        #endregion
    }
}