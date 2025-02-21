using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using XPT.Core.Audio.MP3Sharp.Decoding;

namespace SquidTestingMod.UI
{
    /// <summary>
    /// Main function to create the item panel, 
    /// containing all the items in the game.
    /// </summary>
    public class ItemSpawnerPanel : UIPanel
    {
        // Panel values
        private bool Active = false; // true for visible and update, false for not visible and not update
        private const int padding = 12;
        private const int W = 530 + padding; // Width of the panel
        private const int H = 570; // Height of the panel

        // Colors
        private Color lightBlue = new(63, 82, 151);
        private Color darkBlue = new(40, 47, 82);

        // UI Elements
        private CustomGrid ItemsGrid;
        private UIScrollbar Scrollbar;
        private UIPanel CloseButtonPanel;
        private UIPanel TitlePanel;
        private CustomTextBox SearchTextBox;
        private UIText ItemCountText;
        private UIText HeaderText;

        #region Constructor
        public ItemSpawnerPanel()
        {
            // Set the panel properties
            Width.Set(W, 0f);
            Height.Set(H, 0f);
            HAlign = 0.0f;
            Left.Set(pixels: 20f, precent: 0.0f);
            VAlign = 0.9f;
            BackgroundColor = darkBlue * 1f;

            // Create all content in the panel
            TitlePanel = new CustomTitlePanel(padding: padding, bgColor: lightBlue, height: 35);
            HeaderText = new UIText(text: "Item Spawner", textScale: 0.4f, large: true);
            CloseButtonPanel = new CloseButtonPanel();
            ItemCountText = new CustomItemCountText("0 Items", textScale: 0.4f);

            SearchTextBox = new("Search for items")
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
                Height = { Pixels = 440 - 10 }, // -10 because of scrollbarPadding=5 on top and bottom
                Width = { Pixels = 20 },
                Top = { Pixels = -padding + 30 + padding + 35 + padding + 5 },
                Left = { Pixels = 0f },
            };

            ItemsGrid = new CustomGrid()
            {
                Height = { Pixels = 440 },
                Width = { Percent = 1f, Pixels = -20 },
                ListPadding = 0f, // distance between items
                Top = { Pixels = -padding + 30 + padding + 35 + padding },
                Left = { Pixels = 0f },
                OverflowHidden = true, // hide items outside the grid
            };
            ItemsGrid.ManualSortMethod = (listUIElement) => { };
            ItemsGrid.SetScrollbar(Scrollbar);

            // Add all content in the panel
            Append(TitlePanel);
            Append(HeaderText);
            Append(CloseButtonPanel);
            Append(ItemCountText);
            Append(SearchTextBox);
            Append(Scrollbar);
            Append(ItemsGrid);

            // Add items to the grid
            AddItemSlotsToGrid();
        }
        #endregion

        #region Adding initial slots

        private void AddItemSlotsToGrid()
        {
            int allItems = TextureAssets.Item.Length - 1;
            int count = 0;
            Config c = ModContent.GetInstance<Config>();

            Stopwatch s = Stopwatch.StartNew();

            for (int i = 1; i <= allItems; i++)
            {
                Item item = new();
                if (item == null)
                {
                    Log.Warn("Item is null");
                    continue;
                }
                item.SetDefaults(i);
                CustomItemSlot itemSlot = new([item], 0, ItemSlot.Context.ChestItem);
                ItemsGrid.Add(itemSlot);

                count++;
                if (count >= c.MaxItemsToDisplay)
                    break;
            }

            // update item count text
            s.Stop();
            ItemCountText.SetText(ItemsGrid.Count + " Items in " + Math.Round(s.ElapsedMilliseconds / 1000.0, 3) + " seconds");
        }

        #endregion

        #region FilterItems
        private void FilterItems()
        {
            string searchText = SearchTextBox.currentString.ToLower();
            Config c = ModContent.GetInstance<Config>();

            ItemsGrid.Clear();

            int allItems = TextureAssets.Item.Length - 1;
            int count = 0;

            Stopwatch s = Stopwatch.StartNew();
            for (int i = 1; i <= allItems; i++)
            {
                Item item = new();
                item.SetDefaults(i);

                if (item.Name.Contains(searchText, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    count++;
                    if (count >= c.MaxItemsToDisplay)
                        break;

                    CustomItemSlot itemSlot = new([item], 0, ItemSlot.Context.ChestItem);
                    ItemsGrid.Add(itemSlot);
                }
            }
            s.Stop();
            ItemCountText.SetText(ItemsGrid.Count + " Items in " + Math.Round(s.ElapsedMilliseconds / 1000.0, 3) + " seconds");
        }
        #endregion

        #region dragging and update
        public bool IsDraggingItemPanel;
        private bool dragging;
        private Vector2 dragOffset;
        private const float DragThreshold = 3f; // very low threshold for dragging
        private Vector2 mouseDownPos;

        public override bool ContainsPoint(Vector2 point)
        {
            if (!Active)
                return false;

            return base.ContainsPoint(point);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Active)
                return;

            base.Update(gameTime);

            if (dragging)
            {
                float dragDistance = Vector2.Distance(new Vector2(Main.mouseX, Main.mouseY), mouseDownPos);
                if (dragDistance > DragThreshold)
                {
                    IsDraggingItemPanel = true;
                    Left.Set(Main.mouseX - dragOffset.X, 0f);
                    Top.Set(Main.mouseY - dragOffset.Y, 0f);
                    Recalculate();
                    Main.LocalPlayer.mouseInterface = true;
                }
            }
            else
            {
                IsDraggingItemPanel = false;
            }

            if (dragging || ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            if (!Active || (Scrollbar != null && Scrollbar.ContainsPoint(evt.MousePosition)))
                return;

            mouseDownPos = evt.MousePosition;
            base.LeftMouseDown(evt);
            dragging = true;
            IsDraggingItemPanel = false;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
            Main.LocalPlayer.mouseInterface = true;
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false;
            IsDraggingItemPanel = false;
            Main.LocalPlayer.mouseInterface = false;
            Recalculate();
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            if (IsDraggingItemPanel)
                return;
            base.LeftClick(evt);
            Main.LocalPlayer.mouseInterface = true;
        }
        #endregion

        #region toggle visibility
        // also see update() for more visibility toggling
        // we modify both update() and draw() when active is false
        public bool GetActive() => Active;
        public bool SetActive(bool active) => Active = active;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
                return;
            base.Draw(spriteBatch);
        }
        #endregion

        #region usefulcode
        // https://github.com/ScalarVector1/DragonLens/blob/1b2ca47a5a4d770b256fdffd5dc68c0b4d32d3b2/Content/Tools/Spawners/ItemSpawner.cs#L14
        #endregion
    }
}