using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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

        UIGrid grid = new();

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
            UIGrid grid = CreateGrid();
            UIScrollbar scrollbar = CreateScrollbar();
            grid.SetScrollbar(scrollbar);
            Append(scrollbar);
            Append(grid);
        }

        private void SetupPanelDimensions()
        {
            // Configure main panel
            SetPadding(0);
            Width.Set(300f, 0f);
            Height.Set(300f, 0f);
            HAlign = 0.5f;
            VAlign = 0.5f;
            BackgroundColor = new Color(63, 82, 151) * 0.8f;
        }

        private static UIScrollbar CreateScrollbar()
        {
            return new UIScrollbar()
            {
                HAlign = 1f,
                Height = { Percent = 1f },
            };
        }

        private static UIGrid CreateGrid()
        {
            return new UIGrid()
            {
                Width = { Percent = 1f },
                Height = { Pixels = -50 },
                VAlign = 0.5f,
                ListPadding = 5f,
                OverflowHidden = true,
            };
        }
    }
}