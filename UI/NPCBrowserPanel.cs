using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
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

namespace SquidTestingMod.UI
{
    /// <summary>
    /// Main function to create the item panel, 
    /// containing all the items in the game.
    /// </summary>
    public class NPCBrowserPanel : UIPanel
    {
        // Colors
        private Color lighterBlue = new(69, 89, 162);
        private Color lightBlue = new(63, 82, 151);
        private Color darkBlue = new(40, 47, 82);
        private Color darkerBlue = new(22, 25, 55);

        // UI Elements
        private MinimalGrid ItemsGrid;
        private UIScrollbar Scrollbar;
        private UIPanel ItemBackgroundPanel;
        private UIPanel CloseButtonPanel;
        public UIPanel TitlePanel;
        public SquidTextBox SearchTextBox;

        public NPCBrowserPanel()
        {
            // Set the panel properties
            Width.Set(370f, 0f);
            Height.Set(370f, 0f);
            HAlign = 0.6f;
            VAlign = 0.6f;
            BackgroundColor = darkBlue * 0.5f;

            // Add all content in the panel
            AddHeaderTextPanel();
            AddClosePanel();
            AddSearchTextBox();
            AddItemsBackgroundPanel();
            AddScrollbar();
            AddItemsGrid();

            // Add items to the grid
            AddItemSlotsToGrid();
        }

        #region adding content

        private void AddClosePanel()
        {
            CloseButtonPanel = new CloseButtonPanel();
            Append(CloseButtonPanel);
        }

        private void AddHeaderTextPanel()
        {
            int h = 45;
            TitlePanel = new UIPanel()
            {
                Width = { Percent = 0f, Pixels = 250 + 5 * 5 + 12 * 2 },
                Height = { Pixels = h },
                Top = { Pixels = -12 - h / 2 },
                Left = { Pixels = 0 },
                BackgroundColor = darkBlue,
            };
            Append(TitlePanel);

            // add UIText to TitlePanel
            UIText text = new(text: "NPC Spawner", textScale: 0.6f, large: true)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
            };
            TitlePanel.Append(text);
        }

        private void AddItemSlotsToGrid()
        {
            int allItems = NPCID.Count;
            int count = 0;
            Config c = ModContent.GetInstance<Config>();

            for (int i = 1; i <= allItems - 1; i++)
            {
                NPC npc = new();
                npc.SetDefaults(i);

                // if (npc.FullName != "Ogre" && npc.FullName != "Zombie")
                // continue; // only show ogres and zombies

                // note: you can use BankItem for red color, ChestItem for blue color, etc.
                // UIItemSlot itemSlot = new([item], 0, Terraria.UI.ItemSlot.Context.ChestItem);
                SquidNPCSlot npcSlot = new(npc: npc, slotContext: ItemSlot.Context.ShopItem);
                ItemsGrid.Add(npcSlot);

                Log.Info("Added " + npc.FullName + ", total: " + count);

                count++;
                if (count >= c.MaxItemsToDisplay)
                    break;
            }
        }

        private void AddItemsBackgroundPanel()
        {
            ItemBackgroundPanel = new UIPanel()
            {
                Width = { Percent = 0f, Pixels = 250 + 5 * 5 + 12 * 2 },
                Height = { Percent = 0f, Pixels = 250 + 5 * 5 + 12 },
                HAlign = 0.5f,
                Top = { Pixels = 60 },
                Left = { Pixels = -24 },
                BackgroundColor = darkBlue * 0.8f,
            };
            Append(ItemBackgroundPanel);
        }

        private void AddScrollbar()
        {
            Scrollbar = new UIScrollbar()
            {
                HAlign = 1f,
                Height = { Pixels = 275 },
                Width = { Pixels = 20 },
                Top = { Pixels = 60 + 5 },
                Left = { Pixels = -5 },
            };
            Scrollbar.OnLeftMouseUp += (evt, element) => SearchTextBox.Focus();
            Append(Scrollbar);
        }

        private void AddSearchTextBox()
        {
            SearchTextBox = new("")
            {
                Width = { Percent = 0f, Pixels = 250 + 5 * 5 + 12 * 2 },
                Height = { Pixels = 30 },
                Top = { Pixels = 25 },
                BackgroundColor = Color.White,
                BorderColor = Color.Black * 1f, // TODO 1f or 0.8 for the search box?
            };
            SearchTextBox.OnTextChanged += FilterItems;
            Append(SearchTextBox);
        }

        private void AddItemsGrid()
        {
            ItemsGrid = new MinimalGrid()
            {
                // MaxHeight = { Pixels = 250 + 5 * 5 + 12 * 2 }, // 250 pixels + 5 pixels padding + 12 pixels padding
                Height = { Percent = 0f, Pixels = 250 + 5 * 5 },
                Width = { Percent = 0f, Pixels = 250 + 5 * 5 + 12 * 2 },
                HAlign = 0.5f,
                ListPadding = 5f, // distance between items
                Top = { Pixels = 0f }, // (12 since cornerBox is 12 pixels)
                Left = { Pixels = 3f }, // weird custom offset
                OverflowHidden = true, // hide items outside the grid
            };
            ItemsGrid.ManualSortMethod = (listUIElement) => { };
            ItemsGrid.SetScrollbar(Scrollbar);
            ItemBackgroundPanel.Append(ItemsGrid);
        }
        #endregion

        #region filteritems
        private void FilterItems()
        {
            string searchText = SearchTextBox.currentString.ToLower();
            Log.Info($"Search Text: {searchText}");
            Config c = ModContent.GetInstance<Config>();

            ItemsGrid.Clear();

            int allItems = NPCID.Count;
            int count = 0;

            Stopwatch s = Stopwatch.StartNew();
            for (int i = 1; i <= allItems - 1; i++)
            {
                NPC npc = new();
                npc.SetDefaults(i);

                if (npc.FullName.ToLower().Contains(searchText))
                {
                    count++;
                    if (count >= c.MaxItemsToDisplay)
                        break;

                    SquidNPCSlot npcSlot = new(npc: npc, slotContext: ItemSlot.Context.ShopItem);
                    ItemsGrid.Add(npcSlot);
                }
            }
            s.Stop();
            Log.Info($"Filtering {count} items took {s.ElapsedMilliseconds} ms");
        }
        #endregion

        #region containspoint
        public override bool ContainsPoint(Vector2 point)
        {
            // if it contains the TitlePanel, also return true
            if (TitlePanel.ContainsPoint(point))
                return true;

            return base.ContainsPoint(point);
        }
        #endregion

        #region helpers
        private static void LogInnerOuterDimensions(UIElement element)
        {
            element.Recalculate();
            Log.Info($"{element.GetType().Name} Outer: {element.GetOuterDimensions().Width} x {element.GetOuterDimensions().Height} Inner: {element.GetInnerDimensions().Width} x {element.GetInnerDimensions().Height}");
        }
        #endregion

        #region dragging and update
        public bool IsDragging = false;
        private bool dragging;
        private Vector2 dragOffset;

        // more
        private Vector2 mouseDownPos;
        private const float DragThreshold = 10f; // you can tweak the threshold

        public override void Update(GameTime gameTime)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys?.mainState?.npcButton.isNPCPanelVisible == false)
                return;

            base.Update(gameTime);

            // Check if we moved the mouse more than the threshold
            Vector2 currentPos = new(Main.mouseX, Main.mouseY);
            if (dragging)
            {
                float dragDistance = Vector2.Distance(currentPos, mouseDownPos);

                if (dragDistance > DragThreshold)
                {
                    IsDragging = true; // Now we are officially dragging
                }
            }
            else
            {
                IsDragging = false; // Reset when not dragging
            }

            if (dragging)
            {
                Left.Set(Main.mouseX - dragOffset.X, 0f);
                Top.Set(Main.mouseY - dragOffset.Y, 0f);
                Recalculate();
            }

            // DISABLE CLICKS HOTFIX
            if (dragging)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            // Or if the mouse is currently over the panel, also block usage
            else if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            // prevent dragging if mouse is over scrollbar
            if (Scrollbar != null && Scrollbar.ContainsPoint(evt.MousePosition))
                return;

            mouseDownPos = evt.MousePosition; // store mouse down location

            base.LeftMouseDown(evt);
            dragging = true;
            IsDragging = false;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
            Main.LocalPlayer.mouseInterface = true;
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false;
            IsDragging = false;
            Main.LocalPlayer.mouseInterface = false;
            Recalculate();
        }

        // CLICK
        public override void LeftClick(UIMouseEvent evt)
        {
            if (IsDragging)
                return;
            base.LeftClick(evt);
            Main.LocalPlayer.mouseInterface = true; // block item usage
        }

        #endregion

        #region usefulcode
        // https://github.com/ScalarVector1/DragonLens/blob/1b2ca47a5a4d770b256fdffd5dc68c0b4d32d3b2/Content/Tools/Spawners/ItemSpawner.cs#L14
        #endregion
    }
}