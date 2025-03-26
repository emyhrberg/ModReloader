using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ErkysModdingUtilities.Common.Configs;
using ErkysModdingUtilities.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace ErkysModdingUtilities.UI.Elements
{
    /// <summary>
    /// NPC Spawner Panel – now structured to match the Item Spawner Panel in size, proportions, and behavior.
    /// </summary>
    public class NPCSpawner : SpawnerPanel
    {
        // Filtering fields
        private enum NPCFilter
        {
            All,
            Town,
            Bosses
        }
        private NPCFilter currentFilter = NPCFilter.All;
        private List<(FilterButton button, NPCFilter filter)> filterButtons = new();

        // Mod sorting fields
        public List<FilterModsButton> modSortButtons = new();
        private string currentModFilter = "";

        // Call this when user selects a particular mod (e.g., from a ModSortButton).
        private void FilterByMod(string modName)
        {
            currentModFilter = modName; // or set to "" to disable mod filtering
            FilterItems();
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

        public NPCSpawner() : base(header: "NPC Spawner")
        {
            // Add filter buttons
            AddFilterButton(Ass.FilterAll, "All NPCs", NPCFilter.All, 0);
            AddFilterButton(Ass.FilterTown, "Town NPCs", NPCFilter.Town, 25);
            AddFilterButton(Ass.FilterMob, "Bosses", NPCFilter.Bosses, 50);

            // Populate the grid with NPC slots
            AddItemSlotsToGrid();

            // Set default filter to All
            foreach (var (btn, flt) in filterButtons)
                btn.Active = flt == currentFilter;

            AddModSortButtons();
        }

        private FilterButton AddFilterButton(Asset<Texture2D> texture, string hoverText, NPCFilter filter, float left)
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

        #region Adding NPC Slots

        private void AddItemSlotsToGrid()
        {
            int allNPCs = NPCLoader.NPCCount; // Use total count including modded NPCs
            Stopwatch s = Stopwatch.StartNew();

            List<UIElement> npcSlots = new List<UIElement>();
            for (int i = 1; i < allNPCs; i++)
            {
                try
                {
                    NPC npc = new();
                    npc.SetDefaults(i);

                    CustomNPCSlot npcSlot = new(npc, ItemSlot.Context.ShopItem);
                    npcSlots.Add(npcSlot);
                }
                catch (Exception)
                {
                    // This happens for like 10 NPCs (ID 82, 105, 200, etc), when i add other mods?
                    // Fargos mutant mod made me add this code as a hotfix
                    // Just a catch but all npcs appear anyway?
                    // This will do for now
                    // Log.Warn($"Skipping NPC index {i} due to error: {ex.Message}");
                }
            }

            // Add all slots to the grid
            ItemsGrid.AddRange(npcSlots);

            s.Stop();
            ItemCountText.SetText(ItemsGrid.Count + " NPCs in " + Math.Round(s.ElapsedMilliseconds / 1000.0, 3) + " seconds");
        }

        #endregion

        #region Filtering NPCs

        protected override void FilterItems()
        {
            string searchText = SearchTextBox.currentString.ToLower();
            ItemsGrid.Clear();

            int allNPCs = NPCLoader.NPCCount;
            Stopwatch s = Stopwatch.StartNew();

            // list of NPCSlots
            List<CustomNPCSlot> npcSlots = new List<CustomNPCSlot>();

            for (int i = 1; i < allNPCs; i++)
            {
                NPC npc = new();
                npc.SetDefaults(i);

                // First, check the search text.
                if (!npc.FullName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                // 2) Mod-based filter
                if (!string.IsNullOrEmpty(currentModFilter))
                {
                    if (npc.ModNPC == null || npc.ModNPC.Mod == null)
                        continue;

                    Mod mod = ModLoader.GetMod(npc.ModNPC.Mod.Name);
                    if (mod == null || mod.Name != currentModFilter)
                        continue;
                }

                // Then, check against the selected filter.
                bool passesFilter = currentFilter switch
                {
                    NPCFilter.All => true,
                    NPCFilter.Town => npc.townNPC,
                    NPCFilter.Bosses => npc.boss,
                    _ => false
                };
                if (!passesFilter)
                    continue;

                CustomNPCSlot npcSlot = new(npc, ItemSlot.Context.ShopItem);
                npcSlots.Add(npcSlot);
            }
            // Add all slots to the grid
            ItemsGrid.AddRange(npcSlots);

            s.Stop();
            ItemCountText.SetText(ItemsGrid.Count + " NPCs in " + Math.Round(s.ElapsedMilliseconds / 1000.0, 3) + " seconds");
        }

        #endregion

        // mod icon drawing
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
