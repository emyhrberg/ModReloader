using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    public class NPCSpawnerPanel : SpawnerPanel
    {
        // Filtering fields
        private enum NPCFilter
        {
            All,
            Town,
            Mobs
        }
        private NPCFilter currentFilter = NPCFilter.All;

        #region Constructor
        public NPCSpawnerPanel() : base("NPC Spawner")
        {
            SearchTextBox.OnTextChanged += FilterItems;

            // Filter: All NPCs
            BaseFilterButton allNPCsButton = new(Assets.FilterAll, "All NPCs");
            allNPCsButton.Left.Set(0, 0);
            allNPCsButton.OnLeftClick += (evt, element) =>
            {
                currentFilter = NPCFilter.All;
                FilterItems();
            };
            Append(allNPCsButton);

            // Filter: Town NPCs
            BaseFilterButton townNPCButton = new(Assets.FilterMelee, "Town NPCs");
            townNPCButton.Left.Set(25, 0);
            townNPCButton.OnLeftClick += (evt, element) =>
            {
                currentFilter = NPCFilter.Town;
                FilterItems();
            };
            Append(townNPCButton);

            // Filter: Mobs (Enemies/Bosses)
            BaseFilterButton mobsButton = new(Assets.FilterRanged, "Mobs");
            mobsButton.Left.Set(50, 0);
            mobsButton.OnLeftClick += (evt, element) =>
            {
                currentFilter = NPCFilter.Mobs;
                FilterItems();
            };
            Append(mobsButton);

            // Populate the grid with NPC slots
            AddItemSlotsToGrid();
        }
        #endregion

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
                catch (Exception ex)
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

        private void FilterItems()
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
