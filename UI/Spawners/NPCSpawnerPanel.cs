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

namespace SquidTestingMod.UI.Spawners
{
    /// <summary>
    /// NPC Spawner Panel – now structured to match the Item Spawner Panel in size, proportions, and behavior.
    /// </summary>
    public class NPCSpawnerPanel : UIPanel
    {
        // Panel constants
        private bool NPCPanelActive = false; // true for visible and update, false for not visible and not update
        private const int padding = 12;
        private const int W = 530 + padding; // same as ItemSpawnerPanel width
        private const int H = 570;           // same as ItemSpawnerPanel height

        // Colors
        private Color lightBlue = new(63, 82, 151);
        private Color darkBlue = new(40, 47, 82);

        // UI Elements
        private CustomGrid ItemsGrid;
        private UIScrollbar Scrollbar;
        private UIPanel CloseButtonPanel;
        private UIPanel TitlePanel;
        private CustomTextBox SearchTextBox;
        private UIText NPCCountText;
        private UIText HeaderText;

        // Dragging fields
        public bool IsDraggingNPCPanel;
        private bool dragging;
        private Vector2 dragOffset;
        private const float DragThreshold = 3f;
        private Vector2 mouseDownPos;

        public NPCSpawnerPanel()
        {
            // Set panel properties to mirror the ItemSpawnerPanel
            Width.Set(W, 0f);
            Height.Set(H, 0f);
            HAlign = 0.42f;
            VAlign = 0.6f;
            BackgroundColor = darkBlue * 1f;

            // Create header and title elements
            TitlePanel = new CustomTitlePanel(padding, lightBlue, 35);
            HeaderText = new UIText("NPC Spawner", 0.4f, true);
            CloseButtonPanel = new CloseButtonPanel();
            NPCCountText = new CustomItemCountText("0 NPCs", 0.4f);

            // Create the search text box
            SearchTextBox = new CustomTextBox("Search for NPCs")
            {
                Width = { Pixels = 200 },
                Height = { Pixels = 35 },
                HAlign = 1f,
                // Position matches ItemSpawnerPanel: -padding + 35 + padding = 35 pixels.
                Top = { Pixels = -padding + 35 + padding },
                BackgroundColor = Color.White,
                BorderColor = Color.Black
            };
            SearchTextBox.OnTextChanged += FilterItems;

            // Create the scrollbar
            Scrollbar = new UIScrollbar()
            {
                HAlign = 1f,
                // Height: 440 - 10 to account for padding (same as ItemSpawnerPanel)
                Height = { Pixels = 440 - 10 },
                Width = { Pixels = 20 },
                // Top: -padding + 30 + padding + 35 + padding + 5 (evaluates to 82 pixels with padding=12)
                Top = { Pixels = -padding + 30 + padding + 35 + padding + 5 },
                Left = { Pixels = 0f }
            };

            // Create the grid for NPC slots
            ItemsGrid = new CustomGrid()
            {
                Height = { Pixels = 440 },
                // Width takes full width minus scrollbar width (–20 pixels)
                Width = { Percent = 1f, Pixels = -20 },
                ListPadding = 0f,
                // Top: -padding + 30 + padding + 35 + padding (evaluates to 77 pixels with padding=12)
                Top = { Pixels = -padding + 30 + padding + 35 + padding },
                Left = { Pixels = 0f },
                OverflowHidden = true,
            };
            ItemsGrid.ManualSortMethod = (listUIElement) => { };
            ItemsGrid.SetScrollbar(Scrollbar);

            // Append all elements in order
            Append(TitlePanel);
            Append(HeaderText);
            Append(CloseButtonPanel);
            Append(NPCCountText);
            Append(SearchTextBox);
            Append(Scrollbar);
            Append(ItemsGrid);

            // Populate the grid with NPC slots
            AddItemSlotsToGrid();
        }

        #region Adding NPC Slots

        private void AddItemSlotsToGrid()
        {
            int allNPCs = NPCID.Count;
            int count = 0;

            Stopwatch s = Stopwatch.StartNew();

            for (int i = 1; i <= allNPCs - 1; i++)
            {
                NPC npc = new();
                npc.SetDefaults(i);

                CustomNPCSlot npcSlot = new(npc, ItemSlot.Context.ShopItem);
                ItemsGrid.Add(npcSlot);

                count++;
                if (count >= Conf.MaxItemsToDisplay)
                    break;
            }

            s.Stop();
            NPCCountText.SetText(ItemsGrid.Count + " NPCs in " + Math.Round(s.ElapsedMilliseconds / 1000.0, 3) + " seconds");
        }

        #endregion

        #region Filtering NPCs

        private void FilterItems()
        {
            string searchText = SearchTextBox.currentString.ToLower();

            ItemsGrid.Clear();

            int allNPCs = NPCID.Count;
            int count = 0;

            Stopwatch s = Stopwatch.StartNew();
            for (int i = 1; i <= allNPCs - 1; i++)
            {
                NPC npc = new();
                npc.SetDefaults(i);

                if (npc.FullName.Contains(searchText, StringComparison.CurrentCultureIgnoreCase))
                {
                    count++;
                    if (count >= Conf.MaxItemsToDisplay)
                        break;

                    CustomNPCSlot npcSlot = new(npc, ItemSlot.Context.ShopItem);
                    ItemsGrid.Add(npcSlot);
                }
            }
            s.Stop();
            NPCCountText.SetText(ItemsGrid.Count + " NPCs in " + Math.Round(s.ElapsedMilliseconds / 1000.0, 3) + " seconds");
        }

        #endregion

        #region Dragging & Update

        public override bool ContainsPoint(Vector2 point)
        {
            if (!NPCPanelActive)
                return false;

            return base.ContainsPoint(point);
        }

        public override void Update(GameTime gameTime)
        {
            if (!NPCPanelActive)
                return;

            base.Update(gameTime);

            if (dragging)
            {
                float dragDistance = Vector2.Distance(new Vector2(Main.mouseX, Main.mouseY), mouseDownPos);
                if (dragDistance > DragThreshold)
                {
                    IsDraggingNPCPanel = true;
                    Left.Set(Main.mouseX - dragOffset.X, 0f);
                    Top.Set(Main.mouseY - dragOffset.Y, 0f);
                    Recalculate();
                    Main.LocalPlayer.mouseInterface = true;
                }
            }
            else
            {
                IsDraggingNPCPanel = false;
            }

            if (dragging || ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (!NPCPanelActive)
                return;

            mouseDownPos = evt.MousePosition;
            base.LeftMouseDown(evt);
            dragging = true;
            IsDraggingNPCPanel = false;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
            Main.LocalPlayer.mouseInterface = true;
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false;
            IsDraggingNPCPanel = false;
            Main.LocalPlayer.mouseInterface = false;
            Recalculate();
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (IsDraggingNPCPanel)
                return;

            base.LeftClick(evt);
            Main.LocalPlayer.mouseInterface = true;
        }

        #endregion

        #region Toggle Visibility

        public bool GetNPCPanelActive() => NPCPanelActive;
        public bool SetNPCPanelActive(bool active) => NPCPanelActive = active;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!NPCPanelActive)
                return;
            base.Draw(spriteBatch);
        }

        #endregion
    }
}
