using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
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

        public NPCSpawner() : base("NPC Spawner")
        {
            // Add filter buttons
            AddFilterButton(Assets.FilterAll, "All NPCs", NPCFilter.All, 0);
            AddFilterButton(Assets.FilterTown, "Town NPCs", NPCFilter.Town, 25);
            AddFilterButton(Assets.FilterMob, "Bosses", NPCFilter.Bosses, 50);

            // Populate the grid with NPC slots
            AddItemSlotsToGrid();

            // Set default filter to All
            foreach (var (btn, flt) in filterButtons)
                btn.Active = flt == currentFilter;
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
    }
}
