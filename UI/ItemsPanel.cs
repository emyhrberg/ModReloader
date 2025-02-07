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
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ItemsPanel : UIPanel
    {
        // UI Elements
        private UIGrid grid;
        private UIScrollbar scrollbar;
        private UIBetterTextBox searchTextBox;

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
            searchTextBox = CreateSearchTextBox();
            Append(searchTextBox);

            // TEST box
            UITextBox simpleTextBox = CreateSimpleTextBox();
            Append(simpleTextBox);

            // Create the item panels.
            CreateItemPanels(grid);
        }

        private static UITextBox CreateSimpleTextBox()
        {
            return new UITextBox("Search for items")
            {
                Width = { Percent = 1f, Pixels = -40 },
                Height = { Pixels = 40 }, // minimum height
                Top = { Pixels = 20 },
                Left = { Pixels = 5 },
                BackgroundColor = new Color(56, 58, 134), // inventory dark blue
                BorderColor = Color.Black * 0.8f,
            };
        }

        private static UIBetterTextBox CreateSearchTextBox()
        {
            return new UIBetterTextBox("")
            {
                Width = { Percent = 1f, Pixels = -40 },
                Height = { Pixels = 40 }, // minimum height
                Top = { Pixels = 0 },
                Left = { Pixels = 5 },
                BackgroundColor = new Color(56, 58, 134), // inventory dark blue
                BorderColor = Color.Black * 0.8f,
            };
        }

        private static void CreateItemPanels(UIGrid grid)
        {
            int maxItems = TextureAssets.Item.Length - 1;

            for (int i = 1; i <= 60; i++)
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

                // Log the item’s name
                Item tempItem = new();
                tempItem.SetDefaults(i);
                Log.Info($"Added item ID: {i} - Name: {tempItem.Name}");

                // Add the panel to the grid
                grid.Add(panel);
            }
        }

        private static UIImage CreateItemImage(int itemIndex)
        {
            Main.instance.LoadItem(itemIndex);
            Asset<Texture2D> texture = TextureAssets.Item[itemIndex];
            return new UIImage(texture)
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                ScaleToFit = true,
                OverflowHidden = true,
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

        #region dragging
        private bool dragging;
        private Vector2 dragOffset;
        public override void LeftMouseDown(UIMouseEvent evt)
        {
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // close panel if inventory is closed
            if (!Main.playerInventory)
            {
                MainSystem sys = ModContent.GetInstance<MainSystem>();
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

        // CLICK
        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            Main.LocalPlayer.mouseInterface = true; // mouseinterface refers to whether the mouse is over a UI element
        }

        #endregion
    }
}