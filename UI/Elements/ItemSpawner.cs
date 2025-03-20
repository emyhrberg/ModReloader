using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Elements
{
    /// <summary>
    /// A panel containing a grid of all items in the game that can be spawned.
    /// </summary>
    public class ItemSpawner : SpawnerPanel
    {
        // Filter items
        private enum ItemFilter
        {
            All,
            AllWeapons,
            Melee,
            Ranged,
            Magic,
            Summon,
            Armor,
            Vanity,
            Accessories,
            Potions,
            Placeables,
        }

        private enum ItemSort
        {
            ID,
            Value,
            Rarity,
            Name,
            Damage,
            Defense
        }

        // Filtering fields
        private ItemFilter currentFilter = ItemFilter.All;
        private List<(FilterButton button, ItemFilter filter)> filterButtons = new();

        // Sorting fields
        private bool ascending = true;  // default ascending
        private ItemSort currentSort = ItemSort.ID;
        private List<(SortButton button, ItemSort sort)> sortButtons = new();

        public ItemSpawner() : base(header: "Item Spawner")
        {
            // Add filter buttons
            AddFilterButton(Ass.FilterAll, "Filter All", ItemFilter.All, 0);
            AddFilterButton(Ass.FilterMelee, "Filter All Weapons", ItemFilter.AllWeapons, 25);
            AddFilterButton(Ass.FilterMelee, "Filter Melee Weapons", ItemFilter.Melee, 50);
            // AddFilterButton(Ass.FilterRanged, "Filter Ranged Weapons", ItemFilter.Ranged, 75);
            // AddFilterButton(Ass.FilterMagic, "Filter Magic Weapons", ItemFilter.Magic, 100);
            // AddFilterButton(Ass.FilterSummon, "Filter Summon Weapons", ItemFilter.Summon, 125);
            AddFilterButton(Ass.FilterArmor, "Filter Armor", ItemFilter.Armor, 75);
            // AddFilterButton(Ass.FilterVanity, "Filter Vanity", ItemFilter.Vanity, 175);
            AddFilterButton(Ass.FilterAccessories, "Filter Accessories", ItemFilter.Accessories, 100);
            AddFilterButton(Ass.FilterPotion, "Filter Potions", ItemFilter.Potions, 125);
            AddFilterButton(Ass.FilterPlaceables, "Filter Placeables", ItemFilter.Placeables, 150);

            // Add sort buttons
            SortButton id = AddSortButton(Ass.SortID, "Sort by ID", ItemSort.ID, 0);
            id.Active = true; // Set "ID" to active sort by default
            AddSortButton(Ass.SortValue, "Sort by Value", ItemSort.Value, 25);
            AddSortButton(Ass.SortName, "Sort by Name", ItemSort.Name, 50);
            AddSortButton(Ass.SortRarity, "Sort by Rarity", ItemSort.Rarity, 75);
            // Additional sort buttons can be added here.

            // Activate the correct buttons for filters and sorts
            foreach (var (btn, flt) in filterButtons)
                btn.Active = flt == currentFilter;

            foreach (var (btn, srt) in sortButtons)
                btn.Active = srt == currentSort;

            // Add items to the grid
            AddItemSlotsToGrid();
        }

        private SortButton AddSortButton(Asset<Texture2D> texture, string hoverText, ItemSort sort, float left)
        {
            SortButton button = new SortButton(texture, hoverText);
            button.Left.Set(left, 0);
            button.Width.Set(21f, 0f);
            sortButtons.Add((button, sort));
            button.OnLeftClick += (evt, element) =>
            {
                ascending = !ascending; // flip sort order
                currentSort = sort; // set current sort
                foreach (var (btn, srt) in sortButtons)
                    btn.Active = srt == sort; // set only this sort button to active
                FilterItems(); // clear and re-add items to grid
            };
            Append(button);
            return button;
        }

        private FilterButton AddFilterButton(Asset<Texture2D> texture, string hoverText, ItemFilter filter, float left)
        {
            FilterButton button = new FilterButton(texture, hoverText);
            button.Left.Set(left, 0);
            filterButtons.Add((button, filter));
            button.OnLeftClick += (evt, element) =>
            {
                currentFilter = filter;
                foreach (var (btn, flt) in filterButtons)
                    btn.Active = flt == filter;
                FilterItems();
            };
            Append(button);
            return button;
        }

        private void AddItemSlotsToGrid()
        {
            int allItems = TextureAssets.Item.Length;
            int allItems2 = ItemLoader.ItemCount; // Use total count including modded items

            Log.Info("Total items (TextureAssets.Item.Length): " + allItems);
            Log.Info("Total items (ItemLoader.ItemCount): " + allItems2);

            Stopwatch s = Stopwatch.StartNew();

            for (int i = 1; i <= allItems2; i++)
            {
                if (!ContentSamples.ItemsByType.ContainsKey(i))
                    continue;

                Item item = new();
                item.SetDefaults(i, true); // true needed to load modded items?

                // if (item.IsAir)
                // {
                // Log.Warn($"Skipping invalid item ID {i}: '{item.Name}'");
                // continue;
                // }

                CustomItemSlot itemSlot = new([item], 0, ItemSlot.Context.ChestItem);
                allItemSlots.Add(itemSlot);
            }

            // Add all item slots at once
            ItemsGrid.Clear();
            ItemsGrid.AddRange(allItemSlots);

            s.Stop();
            ItemCountText.SetText($"{ItemsGrid.Count} Items in {Math.Round(s.Elapsed.TotalSeconds, 3)} seconds");
        }

        protected override void FilterItems()
        {
            Stopwatch s = Stopwatch.StartNew();
            string searchText = SearchTextBox.currentString.ToLower();

            // Clear UI immediately (optional: show "loading" text)
            ItemCountText.SetText("Filtering...");

            // Run the filtering and sorting in a background task
            Task.Run(() =>
            {
                var filteredSlots = allItemSlots.Where(slot =>
                {
                    Item item = slot.GetDisplayItem();
                    if (!item.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                        return false;

                    return currentFilter switch
                    {
                        ItemFilter.All => true,
                        ItemFilter.AllWeapons => item.damage > 0,
                        ItemFilter.Melee => item.damage > 0 && item.DamageType == DamageClass.Melee,
                        ItemFilter.Ranged => item.damage > 0 && item.DamageType == DamageClass.Ranged,
                        ItemFilter.Magic => item.damage > 0 && item.DamageType == DamageClass.Magic,
                        ItemFilter.Summon => item.damage > 0 && item.DamageType == DamageClass.Summon,
                        ItemFilter.Armor => item.defense > 0 && (item.legSlot > 0 || item.bodySlot > 0 || item.headSlot > 0),
                        ItemFilter.Vanity => item.vanity,
                        ItemFilter.Accessories => item.accessory,
                        ItemFilter.Potions => item.consumable && item.buffType > 0 || item.potion,
                        ItemFilter.Placeables => item.createTile >= TileID.Dirt || item.createWall >= 0,
                        _ => false
                    };
                }).ToList();

                // Sort the filtered items
                filteredSlots = currentSort switch
                {
                    ItemSort.Value => ascending ? filteredSlots.OrderBy(s => s.GetDisplayItem().value).ToList()
                                                : filteredSlots.OrderByDescending(s => s.GetDisplayItem().value).ToList(),
                    ItemSort.Rarity => ascending ? filteredSlots.OrderBy(s => s.GetDisplayItem().rare).ToList()
                                                 : filteredSlots.OrderByDescending(s => s.GetDisplayItem().rare).ToList(),
                    ItemSort.Name => ascending ? filteredSlots.OrderBy(s => s.GetDisplayItem().Name).ToList()
                                               : filteredSlots.OrderByDescending(s => s.GetDisplayItem().Name).ToList(),
                    ItemSort.Damage => ascending ? filteredSlots.OrderBy(s => s.GetDisplayItem().damage).ToList()
                                                 : filteredSlots.OrderByDescending(s => s.GetDisplayItem().damage).ToList(),
                    ItemSort.Defense => ascending ? filteredSlots.OrderBy(s => s.GetDisplayItem().defense).ToList()
                                                  : filteredSlots.OrderByDescending(s => s.GetDisplayItem().defense).ToList(),
                    _ => ascending ? filteredSlots.OrderBy(s => s.GetDisplayItem().type).ToList()
                                   : filteredSlots.OrderByDescending(s => s.GetDisplayItem().type).ToList()
                };

                // 3) Switch back to the main thread to update UI safely
                Main.QueueMainThreadAction(() =>
                {
                    ItemsGrid.Clear();  // UI modification must happen on the main thread
                    ItemsGrid.AddRange(filteredSlots);

                    s.Stop();
                    ItemCountText.SetText($"{ItemsGrid.Count} Items in {Math.Round(s.Elapsed.TotalSeconds, 3)} seconds");
                });
            });
        }
    }
}
