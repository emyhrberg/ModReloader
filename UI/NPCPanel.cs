using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class NPCPanel : UIPanel
    {
        // UI Elements
        private UIGrid grid;
        private UIScrollbar scrollbar;
        public UIBetterTextBox searchBox;

        public NPCPanel()
        {
            SetupPanelDimensions();

            // Create the scrollbar and grid.
            scrollbar = CreateScrollbar();
            grid = CreateGrid();
            grid.SetScrollbar(scrollbar);
            Append(scrollbar);
            Append(grid);

            // Create the search box
            searchBox = CreateSearchTextBox();
            searchBox.OnTextChanged += FilterItems;
            Append(searchBox);

            // Create the item panels.
            CreateNPCSlots(grid);
        }

        private void FilterItems()
        {
            string searchText = searchBox.currentString.ToLower();
            Log.Info($"Search Text: {searchText}");
            grid.Clear();

            Config c = ModContent.GetInstance<Config>();
            int count = 0;

            int allNPCs = NPCID.Count;

            for (int i = 1; i < allNPCs; i++)  // NPC IDs generally start at 1
            {
                NPC npc = new NPC();
                npc.SetDefaults(i);

                // Use npc.FullName (or another property) to filter
                if (npc.FullName.ToLower().Contains(searchText))
                {
                    count++;
                    if (count > c.ItemBrowser.MaxItemsToDisplay)
                        break;

                    NPCSlot npcSlot = new(npc, Terraria.UI.ItemSlot.Context.ShopItem);
                    npcSlot.Width.Set(50, 0f);
                    npcSlot.Height.Set(50, 0f);
                    grid.Add(npcSlot);


                }
            }
            Log.Info($"Filtered {count} NPCs");
        }


        private void CreateNPCSlots(UIGrid grid)
        {
            int allNPCs = NPCID.Count;

            for (int i = 1; i < allNPCs; i++)
            {
                NPC npc = new NPC();
                npc.SetDefaults(i);

                NPCSlot npcSlot = new(npc, Terraria.UI.ItemSlot.Context.ShopItem);
                npcSlot.Width.Set(50, 0f);
                npcSlot.Height.Set(50, 0f);
                grid.Add(npcSlot);
            }
        }


        private static UIBetterTextBox CreateSearchTextBox()
        {
            return new UIBetterTextBox("")
            {
                focused = true,
                Width = { Percent = 1f, Pixels = -40 },
                Height = { Pixels = 30 }, // minimum height
                Top = { Pixels = 0 },
                Left = { Pixels = 5 },
                BackgroundColor = new Color(56, 58, 134), // inventory dark blue
                BorderColor = Color.Black * 0.8f,
            };
        }

        private static UIScrollbar CreateScrollbar()
        {
            return new UIScrollbar()
            {
                HAlign = 1f,
                Height = { Percent = 1f },
                Width = { Pixels = 20 },
            };
        }

        private static UIGrid CreateGrid()
        {
            return new UIGrid()
            {
                Height = { Percent = 1f, Pixels = -45 },
                Width = { Percent = 1f, Pixels = -40 },
                VAlign = 0.5f,
                HAlign = 0.5f,
                ListPadding = 3f, // distance between items
                Top = { Pixels = 30f },
                Left = { Pixels = -8f },
                OverflowHidden = true,
            };
        }

        private void SetupPanelDimensions()
        {
            Width.Set(340f, 0f);
            Height.Set(340f, 0f);
            HAlign = 0.5f;
            VAlign = 0.6f;
            BackgroundColor = new Color(63, 82, 151) * 0.8f; // light blue
        }

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
            if (scrollbar.ContainsPoint(evt.MousePosition))
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