using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ItemsPanel : UIPanel
    {
        // UI Elements
        private UIGrid grid;
        private UIScrollbar scrollbar;
        public UIBetterTextBox searchBox;

        public ItemsPanel()
        {
            OnInitialize();
        }

        /// <summary>
        /// Main function to create the item panel, 
        /// containing all the items in the game.
        /// </summary>
        public override void OnInitialize()
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

            // log dimensions of searchbox
            LogInnerOuterDimensions(searchBox);

            // Create the item panels.
            CreateItemPanels(grid);
        }

        private void FilterItems()
        {
            string searchText = searchBox.currentString.ToLower();
            Log.Info($"Search Text: {searchText}");

            grid.Clear();
            List<string> visibleItems = [];

            for (int i = 1; i <= 1000; i++)
            {
                Item item = new();
                item.SetDefaults(i);
                if (item.Name.ToLower().Contains(searchText))
                {
                    UIPanel panel = new()
                    {
                        Width = { Pixels = 50 },
                        Height = { Pixels = 50 },
                        BackgroundColor = new Color(56, 58, 134), // inventory dark blue
                        BorderColor = Color.Black * 0.8f,
                        OverflowHidden = true,
                    };

                    // Attempt to load and draw the item texture
                    UIImage itemImage = CreateItemImage(i);
                    panel.Append(itemImage);

                    // when hovering the panel, draw a red rectangle at center with size 50,50 to indicate.
                    panel.OnMouseOver += (evt, element) =>
                    {
                        Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)element.GetDimensions().X, (int)element.GetDimensions().Y, 50, 50), Color.Red);
                        Log.Info($"Hovering over {item.Name}");
                    };

                    // Add the panel to the grid
                    grid.Add(panel);

                    // Add item name to visible items list
                    visibleItems.Add(item.Name);
                }
            }

            // Log visible items
            Log.Info($"Visible Items: {string.Join(", ", visibleItems)}");
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

        private static void CreateItemPanels(UIGrid grid)
        {
            int maxItems = TextureAssets.Item.Length - 1;

            for (int i = 1; i <= 1000; i++)
            {
                UIPanel panel = new()
                {
                    Width = { Pixels = 50 },
                    Height = { Pixels = 50 },
                    BackgroundColor = new Color(56, 58, 134), // inventory dark blue
                    BorderColor = Color.Black * 0.8f,
                    OverflowHidden = true,
                };

                // Attempt to load and draw the item texture
                UIImage itemImage = CreateItemImage(i);
                panel.Append(itemImage);

                // Add the panel to the grid
                grid.Add(panel);
            }
        }

        private static UIImage CreateItemImage(int itemIndex)
        {
            Main.instance.LoadItem(itemIndex);
            Asset<Texture2D> texture = TextureAssets.Item[itemIndex];
            UIImage itemImage = new(texture)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                ScaleToFit = true,
                OverflowHidden = true,
            };

            // Full vanilla tooltip implementation
            itemImage.OnUpdate += element =>
            {
                if (element.IsMouseHovering)
                {
                    Item item = new Item();
                    item.SetDefaults(itemIndex);

                    Main.LocalPlayer.mouseInterface = true;
                    Main.HoverItem = item.Clone();
                    Main.hoverItemName = "a";
                    // Main.hoverItemName = Main.HoverItem.Name;
                }
            };

            itemImage.OnLeftClick += (evt, element) =>
            {
                Item item = new();
                item.SetDefaults(itemIndex);
                // put item in the players hand
                if (Main.mouseItem.IsAir)
                {
                    if (!Main.playerInventory)
                        Main.playerInventory = true;

                    Item clone = item.Clone();
                    Main.mouseItem.stack = item.maxStack;
                    Main.mouseItem = clone;

                    if (!ItemID.Sets.Deprecated[item.type])
                        Main.mouseItem.SetDefaults(item.type);

                    // stackDelay = 30;
                }
            };
            return itemImage;
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
                Width = { Percent = 1f, Pixels = -20 },
                VAlign = 0.5f,
                HAlign = 0.5f,
                ListPadding = 5f, // distance between items
                Top = { Pixels = 30 },
                Left = { Pixels = -2 },
                OverflowHidden = true,
            };
        }

        private void SetupPanelDimensions()
        {
            Width.Set(340f, 0f);
            Height.Set(340f, 0f);
            HAlign = 0.5f;
            VAlign = 0.5f;
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
        private bool dragging;
        private Vector2 dragOffset;

        private bool closeRequested = false;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // close panel if inventory is closed
            if (!Main.playerInventory)
            {
                closeRequested = true;
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

            // close request
            if (closeRequested)
            {
                closeRequested = false;
                Main.QueueMainThreadAction(() =>
                {
                    MainSystem sys = ModContent.GetInstance<MainSystem>();
                    sys?.mainState?.itemBrowserButton?.ToggleItemsPanel();
                });
            }
        }

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            // prevent dragging if mouse is over scrollbar
            if (scrollbar.ContainsPoint(evt.MousePosition))
                return;

            base.LeftMouseDown(evt);
            dragging = true;
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
            Main.LocalPlayer.mouseInterface = true;
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false;
            Main.LocalPlayer.mouseInterface = false;
            Recalculate();
        }

        // CLICK
        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            Main.LocalPlayer.mouseInterface = true; // block item usage
        }

        #endregion

        #region usefulcode
        // https://github.com/ScalarVector1/DragonLens/blob/1b2ca47a5a4d770b256fdffd5dc68c0b4d32d3b2/Content/Tools/Spawners/ItemSpawner.cs#L14
        //     // Allows for "Hold RMB to get more
        // 		if (IsMouseHovering && Main.mouseRight && Main.mouseItem.type == item.type)
        // 		{
        // 			if (stackDelay > 0)
        // 				stackDelay--;
        // 			else if (Main.mouseItem.stack<Main.mouseItem.maxStack)
        //                 Main.mouseItem.stack++;
        // }
        #endregion
    }
}