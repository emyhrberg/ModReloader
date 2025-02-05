using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using ReLogic.Content;
using System;

namespace SquidTestingMod.UI
{
    public class ItemsPanel : UIPanel
    {
        public ItemsPanel()
        {
            OnInitialize();
        }

        public override void OnInitialize()
        {
            // Configure main panel
            SetPadding(0);
            Width.Set(300f, 0f);
            Height.Set(300f, 0f);
            HAlign = 0.5f;
            VAlign = 0.5f;
            BackgroundColor = new Color(63, 82, 151) * 0.8f;

            int itemsPerRow = 5;
            float itemSize = 50f;
            // Use only the first two rows regardless of total available items.
            int totalRows = 2;
            int totalItems = Math.Min((TextureAssets.Item.Length - 1), itemsPerRow * totalRows); // skip index 0

            UIList list = new();
            list.Width.Set(0f, 1f);
            list.Height.Set(0f, 1f);
            list.ListPadding = 0;
            Append(list);

            // Add a scrollbar if desired.
            UIScrollbar scrollbar = new();
            scrollbar.HAlign = 1f;
            scrollbar.Height.Set(0f, 1f);
            Append(scrollbar);
            list.SetScrollbar(scrollbar);

            int currentItemIndex = 1;
            // Loop only for the first two rows.
            for (int row = 0; row < totalRows; row++)
            {
                UIPanel rowPanel = new UIPanel();
                rowPanel.SetPadding(0);
                rowPanel.Width.Set(0f, 1f);
                rowPanel.Height.Set(itemSize, 0f);
                rowPanel.BackgroundColor = Color.Transparent;
                rowPanel.BorderColor = Color.Transparent;

                for (int col = 0; col < itemsPerRow; col++)
                {
                    if (currentItemIndex > totalItems)
                        break;

                    // Debug log the current row, column, and item ID
                    ModContent.GetInstance<SquidTestingMod>().Logger.Debug($"Row {row}, Col {col}: Loading item ID {currentItemIndex}");

                    // Create an item panel for the individual item.
                    UIPanel itemPanel = new UIPanel();
                    itemPanel.SetPadding(0);
                    itemPanel.Width.Set(itemSize, 0f);
                    itemPanel.Height.Set(itemSize, 0f);
                    itemPanel.BackgroundColor = new Color(40, 33, 82);
                    itemPanel.Left.Set(col * itemSize, 0f);

                    try
                    {
                        Main.instance.LoadItem(currentItemIndex);
                        var tex = TextureAssets.Item[currentItemIndex];
                        if (tex != null)
                        {
                            UIImage img = new UIImage(tex);
                            img.HAlign = 0.5f;
                            img.VAlign = 0.5f;
                            itemPanel.Append(img);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModContent.GetInstance<SquidTestingMod>().Logger.Warn($"Could not load item {currentItemIndex}: {ex.Message}");
                    }

                    // Capture the current item id for use in the events.
                    int itemID = currentItemIndex;
                    // Create a new instance of Item to obtain its display name.
                    Item item = new Item();
                    item.SetDefaults(itemID);

                    // Add a hover event to display the tooltip (using Terraria's built-in hover text)
                    itemPanel.OnMouseOver += (evt, element) =>
                    {
                        Main.hoverItemName = item.Name;
                    };

                    // When clicked, give the player 9999 of this item.
                    itemPanel.OnLeftClick += (evt, element) =>
                    {
                        // Note: Adjust the source if needed.
                        Main.LocalPlayer.QuickSpawnItem(null, itemID, 9999);
                    };

                    rowPanel.Append(itemPanel);
                    currentItemIndex++;
                }
                list.Add(rowPanel);
            }
            Recalculate();
        }
    }
}
