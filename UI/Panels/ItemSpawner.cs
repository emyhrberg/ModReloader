using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel containing a grid of all items in the game that can be spawned.
    /// </summary>
    public class ItemSpawner : SpawnerPanel
    {
        // Timing variables
        private Stopwatch _updateTimer = new Stopwatch();
        private bool _pendingUpdate = false;

        private int _hideTimer = 0;

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

        public ItemSpawner() : base("Item Spawner")
        {
            // Add filter buttons
            FilterButton all = AddFilterButton(Assets.FilterAll, "Filter All", ItemFilter.All, 0);
            AddFilterButton(Assets.FilterMelee, "Filter All Weapons", ItemFilter.AllWeapons, 25);
            AddFilterButton(Assets.FilterMelee, "Filter Melee Weapons", ItemFilter.Melee, 50);
            AddFilterButton(Assets.FilterRanged, "Filter Ranged Weapons", ItemFilter.Ranged, 75);
            AddFilterButton(Assets.FilterMagic, "Filter Magic Weapons", ItemFilter.Magic, 100);
            AddFilterButton(Assets.FilterSummon, "Filter Summon Weapons", ItemFilter.Summon, 125);
            AddFilterButton(Assets.FilterArmor, "Filter Armor", ItemFilter.Armor, 150);
            AddFilterButton(Assets.FilterVanity, "Filter Vanity", ItemFilter.Vanity, 175);
            AddFilterButton(Assets.FilterAccessories, "Filter Accessories", ItemFilter.Accessories, 200);
            AddFilterButton(Assets.FilterPotion, "Filter Potions", ItemFilter.Potions, 225);
            AddFilterButton(Assets.FilterPlaceables, "Filter Placeables", ItemFilter.Placeables, 250);

            // Add sort buttons
            SortButton id = AddSortButton(Assets.SortID, "Sort by ID", ItemSort.ID, 0);
            id.Active = true; // Set "ID" to active sort by default
            AddSortButton(Assets.SortValue, "Sort by Value", ItemSort.Value, 25);
            AddSortButton(Assets.SortName, "Sort by Name", ItemSort.Name, 50);
            AddSortButton(Assets.SortRarity, "Sort by Rarity", ItemSort.Rarity, 75);
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
            int allItems = TextureAssets.Item.Length - 1;
            Log.Info("Total items: " + allItems);

            Stopwatch s = Stopwatch.StartNew();

            for (int i = 1; i <= allItems; i++)
            {
                Item item = new();
                item.SetDefaults(i, true); // true needed to load modded items?

                if (item.IsAir || item.type == ItemID.None)
                {
                    Log.Warn($"Skipping invalid item ID {i}: '{item.Name}'");
                    continue;
                }

                CustomItemSlot itemSlot = new([item], 0, ItemSlot.Context.ChestItem);
                allItemSlots.Add(itemSlot);
            }

            // Add all item slots at once
            ItemsGrid.AddRange(allItemSlots);

            s.Stop();
            ItemCountText.SetText($"{ItemsGrid.Count} Items in {Math.Round(s.Elapsed.TotalSeconds, 3)} seconds");
        }

        protected override void FilterItems()
        {
            string searchText = SearchTextBox.currentString.ToLower();
            ItemsGrid.Clear();

            // Start the timer and mark update as pending
            _updateTimer.Restart();
            _pendingUpdate = true;

            Stopwatch s = Stopwatch.StartNew(); // For debugging if needed

            // 1) Filter the items
            var filteredSlots = allItemSlots.Where(slot =>
            {
                Item item = slot.GetDisplayItem();

                if (!item.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                    return false;

                bool passesFilter = currentFilter switch
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

                return passesFilter;
            });

            // 2) Sort the items
            filteredSlots = currentSort switch
            {
                ItemSort.Value => ascending ? filteredSlots.OrderBy(slot => slot.GetDisplayItem().value)
                                             : filteredSlots.OrderByDescending(slot => slot.GetDisplayItem().value),
                ItemSort.Rarity => ascending ? filteredSlots.OrderBy(slot => slot.GetDisplayItem().rare)
                                              : filteredSlots.OrderByDescending(slot => slot.GetDisplayItem().rare),
                ItemSort.Name => ascending ? filteredSlots.OrderBy(slot => slot.GetDisplayItem().Name)
                                            : filteredSlots.OrderByDescending(slot => slot.GetDisplayItem().Name),
                ItemSort.Damage => ascending ? filteredSlots.OrderBy(slot => slot.GetDisplayItem().damage)
                                              : filteredSlots.OrderByDescending(slot => slot.GetDisplayItem().damage),
                ItemSort.Defense => ascending ? filteredSlots.OrderBy(slot => slot.GetDisplayItem().defense)
                                               : filteredSlots.OrderByDescending(slot => slot.GetDisplayItem().defense),
                _ => ascending ? filteredSlots.OrderBy(slot => slot.GetDisplayItem().type)
                               : filteredSlots.OrderByDescending(slot => slot.GetDisplayItem().type)
            };

            List<UIElement> filteredList = filteredSlots.Cast<UIElement>().ToList();

            // 3) Add the sorted/filtered slots in one go
            ItemsGrid.AddRange(filteredList);

            // Force recalculation of the grid layout
            // ItemsGrid.Recalculate();

            // Set hide timer to delay drawing the grid for a few frames after repopulation
            _hideTimer = 3;

            s.Stop(); // Debug timing (if needed)
        }

        // Override with the same access modifier (protected)
        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            if (_hideTimer > 0)
            {
                _hideTimer--;

                // Manually draw every child except ItemsGrid
                foreach (UIElement element in Elements)
                {
                    if (element == ItemsGrid)
                        continue;
                    element.Draw(spriteBatch);
                }
                return;
            }

            // Draw normally
            base.DrawChildren(spriteBatch);

            // Now that drawing is complete, stop the update timer if pending
            if (_pendingUpdate)
            {
                _updateTimer.Stop();
                double elapsedSeconds = _updateTimer.Elapsed.TotalSeconds;
                ItemCountText.SetText($"{ItemsGrid.Count} Items in {Math.Round(elapsedSeconds, 3)} seconds");
                _pendingUpdate = false;
            }
        }
    }
}
