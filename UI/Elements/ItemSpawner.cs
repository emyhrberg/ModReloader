using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using EliteTestingMod.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace EliteTestingMod.UI.Elements
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
            Misc // everything except all the other categories
        }

        // Filtering fields
        private ItemFilter currentFilter = ItemFilter.All;
        private List<(FilterButton button, ItemFilter filter)> filterButtons = new();

        // Mod sorting fields
        public List<FilterModsButton> modSortButtons = new();
        private string currentModFilter = "";

        // Call this when user selects a particular mod (e.g., from a ModSortButton).
        private void FilterByMod(string modName)
        {
            currentModFilter = modName; // or set to "" to disable mod filtering
            FilterItems();
        }

        public ItemSpawner() : base(header: "Item Spawner")
        {
            // Add filter buttons
            AddFilterButton(Ass.FilterAll, "Filter All", ItemFilter.All, 0);
            AddFilterButton(Ass.FilterMelee, "Filter All Weapons", ItemFilter.AllWeapons, 25);
            AddFilterButton(Ass.FilterArmor, "Filter Armor", ItemFilter.Armor, 50);
            AddFilterButton(Ass.FilterVanity, "Filter Vanity", ItemFilter.Vanity, 75);
            AddFilterButton(Ass.FilterAccessories, "Filter Accessories", ItemFilter.Accessories, 100);
            AddFilterButton(Ass.FilterPotion, "Filter Potions", ItemFilter.Potions, 125);
            AddFilterButton(Ass.FilterPlaceables, "Filter Placeables", ItemFilter.Placeables, 150);
            AddFilterButton(Ass.FilterMisc, "Filter Misc", ItemFilter.Misc, 175);

            // Activate the correct buttons for filters and sorts
            foreach (var (btn, flt) in filterButtons)
                btn.Active = flt == currentFilter;

            AddModSortButtons();

            // Add items to the grid
            AddItemSlotsToGrid();
        }

        private void AddModSortButtons()
        {
            // Add "All Mods" button first:
            FilterModsButton allMods = new(
                texture: Ass.FilterAll,
                hoverText: "All Mods",
                internalModName: null,
                left: 0
            );
            allMods.OnLeftClick += (evt, ele) =>
            {
                // Use empty filter for all mods.
                FilterByMod("");
                // Set only the All Mods button active.
                foreach (var btn in modSortButtons)
                    btn.Active = btn == allMods;
                allMods.Active = true;
            };
            Append(allMods);
            modSortButtons.Add(allMods);

            // Add other ModSortButtons:
            Asset<Texture2D> defaultIcon = Main.Assets.Request<Texture2D>("Images/UI/DefaultResourcePackIcon", AssetRequestMode.ImmediateLoad);
            var mods = ModLoader.Mods.Skip(1); // ignore the built in Modloader mod
            float left = 25;
            foreach (Mod mod in mods)
            {
                FilterModsButton modSortButton = new(
                    texture: defaultIcon,
                    hoverText: mod.DisplayNameClean,
                    internalModName: mod.Name,
                    left: left
                );
                modSortButton.OnLeftClick += (evt, ele) =>
                {
                    // When clicking a mod button, use its DisplayNameClean as the filter.
                    FilterByMod(mod.Name);
                    // Update active state for all buttons
                    foreach (var btn in modSortButtons)
                        btn.Active = (btn == modSortButton);
                    modSortButton.Active = true;
                };
                Append(modSortButton);
                modSortButtons.Add(modSortButton);
                left += 25f;
            }
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

            for (int i = 0; i <= allItems2; i++)
            {
                if (!ContentSamples.ItemsByType.ContainsKey(i))
                {
                    // Log.Warn($"Item ID {i} not found in ContentSamples.ItemsByType");
                    continue;
                }


                Item item = new();
                item.SetDefaults(i, true); // true needed to load modded items?

                // check if air or invalid item
                if (item.type == ItemID.None || item.type == ItemID.Count)
                {
                    // Log.Warn($"Item ID {i} is invalid");
                    continue;
                }

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
                    // 1) Optional search by name.
                    if (!item.Name.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                        return false;

                    // 2) Mod-based filter if set (skip if item.ModItem == null or different mod).
                    if (!string.IsNullOrEmpty(currentModFilter))
                    {
                        if (item.ModItem == null || item.ModItem.Mod?.Name != currentModFilter)
                            return false;
                    }

                    // 3) Standard type-based filter.
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
                        ItemFilter.Misc => !((item.damage > 0) ||
                          (item.defense > 0 && (item.legSlot > 0 || item.bodySlot > 0 || item.headSlot > 0)) ||
                          item.vanity ||
                          item.accessory ||
                          (item.consumable && (item.buffType > 0 || item.potion)) ||
                          item.createTile >= TileID.Dirt || item.createWall >= 0
                       ),
                        _ => false
                    };
                }).ToList();

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

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // draw icon hovering
            foreach (FilterModsButton modsButton in modSortButtons)
            {
                if (modsButton is FilterModsButton filterModsButton)
                {
                    var icon = filterModsButton.updatedTex;
                    if (icon != null && filterModsButton.IsMouseHovering)
                    {
                        Vector2 mousePos = new(Main.mouseX, Main.mouseY - icon.Height);

                        spriteBatch.Draw(icon, mousePos, null, Color.White, 0f, Vector2.Zero, scale: 1f,
                        SpriteEffects.None, 0f);
                    }
                }
            }
        }
    }
}
