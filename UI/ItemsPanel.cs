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
        private UISearchBox searchTextBox;

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
            searchTextBox = CreateSearchTextBox();
            Append(searchTextBox);
            Append(scrollbar);
            Append(grid);

            // Log height of searchtextbox
            Log.Info($"SearchTextBox height outer: {searchTextBox.GetOuterDimensions().Height}");
            Log.Info($"SearchTextBox height inner: {searchTextBox.GetInnerDimensions().Height}");

            // Debugging
            LogInnerOuterDimensions(this);
            LogInnerOuterDimensions(searchTextBox);
            LogInnerOuterDimensions(scrollbar);
            LogInnerOuterDimensions(grid);

            // Create the item panels.
            CreateItemPanels(grid);
        }

        private static UISearchBox CreateSearchTextBox()
        {
            return new UISearchBox("Search for items")
            {
                Width = { Percent = 1f, Pixels = -30 },
                Height = { Pixels = 40 }, // minimum height
                Top = { Pixels = 0 },
                Left = { Pixels = 0 },
                BackgroundColor = Color.DarkCyan,
                BorderColor = Color.DarkBlue,
            };
        }

        private static void CreateItemPanels(UIGrid grid)
        {
            int maxItems = TextureAssets.Item.Length - 1;

            for (int i = 1; i <= 100; i++)
            {
                UIPanel panel = new()
                {
                    Width = { Pixels = 50 },
                    Height = { Pixels = 50 },
                    BackgroundColor = Color.Blue,
                    BorderColor = Color.Red,
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
            var texture = TextureAssets.Item[itemIndex];
            if (texture != null)
            {
                UIImage itemImage = new(texture)
                {
                    HAlign = 0.5f,
                    VAlign = 0.5f,
                    ScaleToFit = true,
                    OverflowHidden = true,
                };
                return itemImage;
            }
            return null;
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
                ListPadding = 10f, // distance between items
                Top = { Pixels = 30 },
                Left = { Pixels = -10 },
                OverflowHidden = true,
            };
        }

        private void SetupPanelDimensions()
        {
            Width.Set(340f, 0f);
            Height.Set(340f, 0f);
            HAlign = 0.5f;
            VAlign = 0.5f;
            BackgroundColor = new Color(63, 82, 151) * 0.8f;
        }

        #region helpers
        private static void LogInnerOuterDimensions(UIElement element)
        {
            element.Recalculate();
            Log.Info($"{element.GetType().Name} Outer: {element.GetOuterDimensions().Width} x {element.GetOuterDimensions().Height} Inner: {element.GetInnerDimensions().Width} x {element.GetInnerDimensions().Height}");
        }
        #endregion
    }
}