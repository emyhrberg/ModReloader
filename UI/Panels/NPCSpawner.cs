using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
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
            Mobs
        }
        private NPCFilter currentFilter = NPCFilter.All;
        private List<FilterButton> allFilterButtons = new();

        public NPCSpawner() : base("NPC Spawner")
        {
            // Add filter buttons
            FilterButton allButton = AddFilterButton(Assets.FilterAll, "All NPCs", NPCFilter.All, 0);
            AddFilterButton(Assets.FilterTown, "Town NPCs", NPCFilter.Town, 25);
            AddFilterButton(Assets.FilterMob, "Mobs", NPCFilter.Mobs, 50);

            // Populate the grid with NPC slots
            AddItemSlotsToGrid();

            // Set default filter to All
            // NPCFilterClicked(NPCFilter.All, allButton);
            // Somehow this causes NPC out of bounds crash.
        }

        private FilterButton AddFilterButton(Asset<Texture2D> texture, string hoverText, NPCFilter filter, float left)
        {
            FilterButton button = new FilterButton(texture, hoverText);
            button.Left.Set(left, 0);
            button.OnLeftClick += (evt, element) => NPCFilterClicked(filter, button);
            allFilterButtons.Add(button);
            Append(button);
            return button;
        }

        private void NPCFilterClicked(NPCFilter filter, FilterButton button)
        {
            // Set current filter to active
            FilterButton.ActiveButton = button;

            // Set the current filter and filter the items
            currentFilter = filter;
            FilterItems();
        }

        #region Adding NPC Slots

        private void AddItemSlotsToGrid()
        {
            int allNPCs = NPCLoader.NPCCount; // Use total count including modded NPCs
            int count = 0;
            Stopwatch s = Stopwatch.StartNew();

            for (int i = 1; i < allNPCs; i++)
            {
                try
                {
                    NPC npc = new();
                    npc.SetDefaults(i);

                    CustomNPCSlot npcSlot = new(npc, ItemSlot.Context.ShopItem);
                    ItemsGrid.Add(npcSlot);

                    count++;
                    if (count >= Conf.MaxItemsToDisplay)
                        break;
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
            int count = 0;
            Stopwatch s = Stopwatch.StartNew();

            for (int i = 1; i < allNPCs; i++)
            {
                NPC npc = new();
                npc.SetDefaults(i);

                // First, check the search text.
                if (!npc.FullName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                    continue;

                // Then, check against the selected filter.
                bool passesFilter = false;
                switch (currentFilter)
                {
                    case NPCFilter.All:
                        passesFilter = true;
                        break;
                    case NPCFilter.Town:
                        passesFilter = npc.townNPC; // Only include town NPCs.
                        break;
                    case NPCFilter.Mobs:
                        passesFilter = !npc.friendly; // Only include non-friendly (mob/boss) NPCs.
                        break;
                }
                if (!passesFilter)
                    continue;

                count++;
                if (count >= Conf.MaxItemsToDisplay)
                    break;

                CustomNPCSlot npcSlot = new(npc, ItemSlot.Context.ShopItem);
                ItemsGrid.Add(npcSlot);
            }
            s.Stop();
            ItemCountText.SetText(ItemsGrid.Count + " NPCs in " + Math.Round(s.ElapsedMilliseconds / 1000.0, 3) + " seconds");
        }

        #endregion
    }
}
