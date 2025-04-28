using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    /// <summary>
    /// A base class for panels for item and NPC spawner.
    /// This can be extended to add scrollbar, searchbox, itemcounts, itemgrid, etc.
    /// This class makes it easier to focus on only filter functions for the item/npc panels
    /// </summary>
    public abstract class SpawnerPanel : DraggablePanel
    {
        // UI Elements
        protected CustomGrid ItemsGrid;
        protected UIScrollbar Scrollbar;
        protected CustomTextBox SearchTextBox;
        protected UIText ItemCountText;
        public CustomTextBox GetCustomTextBox() => SearchTextBox;
        private float padding = 12f; // padding between elements

        // Store item slots
        protected List<CustomItemSlot> allItemSlots = [];

        public SpawnerPanel(string header) : base(header)
        {
            // Set the panel properties
            // Width.Set(Conf.C.PanelWidth, 0f);
            // Height.Set(Conf.C.PanelHeight, 0f);
            // HAlign = 0.0f;
            // VAlign = 1.0f;
            // Top.Set(-70, 0f);
            // Left.Set(20, 0f);

            // Create all content in the panel
            ItemCountText = new UIText("0 Items", textScale: 0.4f, true)
            {
                HAlign = 0.5f,
                VAlign = 1f,
            };

            SearchTextBox = new("Search")
            {
                Width = { Pixels = 200 },
                Height = { Pixels = 35 },
                HAlign = 1f,
                Top = { Pixels = -padding + 35 + padding },
                BackgroundColor = Color.White,
                BorderColor = Color.Black
            };
            SearchTextBox.OnTextChanged += FilterItems;

            Scrollbar = new UIScrollbar()
            {
                HAlign = 1f,
                Height = { Pixels = Height.Pixels - 130 - 10 }, // -10 because of scrollbarPadding=5 on top and bottom
                MaxHeight = { Pixels = 700 },
                Width = { Pixels = 20 },
                Top = { Pixels = -padding + 30 + padding + 35 + padding + 5 },
                Left = { Pixels = 0f },
            };

            ItemsGrid = new CustomGrid()
            {
                MaxHeight = { Pixels = 700 },
                Height = { Pixels = Height.Pixels - 130 },
                Width = { Percent = 1f, Pixels = -20 },
                ListPadding = 0f, // distance between items
                Top = { Pixels = -padding + 30 + padding + 35 + padding },
                Left = { Pixels = 0f },
                OverflowHidden = true, // hide items outside the grid
            };
            ItemsGrid.ManualSortMethod = (listUIElement) => { };
            ItemsGrid.SetScrollbar(Scrollbar);

            // Add all content in the panel
            Append(ItemCountText);
            Append(SearchTextBox);
            Append(Scrollbar);
            Append(ItemsGrid);
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (Scrollbar != null && Scrollbar.ContainsPoint(evt.MousePosition))
                return;

            if (!Active)
                return;

            mouseDownPos = evt.MousePosition;
            base.LeftMouseDown(evt);
            dragging = true;
            IsDragging = false;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);


            base.LeftMouseDown(evt);
        }

        protected virtual void FilterItems()
        {
            // Implement this in child classes
        }

        #region Update
        public override void Update(GameTime gameTime)
        {
            // if (IsMouseHovering)
            // {
            //     Main.LocalPlayer.mouseInterface = true;
            // }

            if (!Active)
                return;

            // height
            // float panelH = Conf.C.PanelHeight;

            //Height.Set(panelH, 0);
            //ItemsGrid.Height.Set(panelH - 130, 0);
            //Scrollbar.Height.Set(panelH - 140, 0);

            // width
            // Width.Set(Conf.C.PanelWidth, 0);

            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            base.Update(gameTime);

            //if (resizeButton.draggingResize)
            //{
            //    dragging = false;  // Prevent panel dragging while resizing
            //    IsDragging = false;
            //    return;
            //}

            // If we have buttons to the left, move the panel to the right
            // MainSystem sys = ModContent.GetInstance<MainSystem>();
            // if (Conf.C.ButtonPosition == "Left" && sys.mainState.AreButtonsShowing)
            // {
            //     Left.Set(sys.mainState.ButtonSize, 0f);
            //     // Log.SlowInfo("Moving panel to the right");
            //     Recalculate();
            //     return;
            // }

            if (dragging)
            {
                float dragDistance = Vector2.Distance(new Vector2(Main.mouseX, Main.mouseY), mouseDownPos);
                if (dragDistance > DragThreshold)
                {
                    IsDragging = true;
                    Left.Set(Main.mouseX - dragOffset.X, 0f);
                    Top.Set(Main.mouseY - dragOffset.Y, 0f);
                    Recalculate();
                }
            }
            else
            {
                IsDragging = false;
            }
        }
        #endregion

        #region Dragging
        public override bool ContainsPoint(Vector2 point)
        {
            if (!Active)
                return false;

            return base.ContainsPoint(point);
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false;
            IsDragging = false;
            Recalculate();
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (IsDragging)
                return;
            base.LeftClick(evt);
        }
        #endregion

        #region Reset position
        // When we click on a button, we toggle the active state of the panel.
        // This method is called to reset the position of the panel when it is toggled (when the panel is shown)
        public override bool SetActive(bool active)
        {
            if (active)
            {
                // Reset panel position
                Left.Set(20, 0f);
                Top.Set(-70, 0f);
                Recalculate();
            }

            Active = active;
            return Active;
        }
        #endregion
    }
}