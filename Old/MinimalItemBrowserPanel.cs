// using System;
// using System.Collections.Generic;
// using Microsoft.Xna.Framework;
// using SquidTestingMod.Helpers;
// using Terraria;
// using Terraria.GameContent.UI.Elements;
// using Terraria.ModLoader.UI.Elements;
// using Terraria.UI;

// namespace SquidTestingMod.UI
// {
//     /// <summary>
//     /// Creates a panel that displays all items in the game in a grid with a scrollbar.
//     /// </summary>
//     public class MinimalItemBrowserPanel : UIPanel
//     {
//         // Constructor
//         public MinimalItemBrowserPanel()
//         {
//             // Set the panel properties
//             Width.Set(40 * 5 + 24, 0f);
//             Height.Set(40 * 5 + 24, 0f);
//             HAlign = 0.6f;
//             VAlign = 0.6f;
//             BackgroundColor = new Color(40, 47, 82) * 0.8f;

//             // Create the grid to hold the items
//             MinimalGrid ItemsGrid = new();
//             ItemsGrid.Width.Set(40 * 5, 0f);
//             ItemsGrid.Height.Set(40 * 5, 0f);
//             ItemsGrid.ListPadding = 0f;
//             ItemsGrid.ManualSortMethod = (e) => { };
//             Append(ItemsGrid);

//             // Add 50 items to the grid
//             for (int i = 1; i <= 50; i++)
//             {
//                 Item item = new();
//                 item.SetDefaults(i);

//                 UIItemSlot itemSlot = new([item], 0, ItemSlot.Context.ChestItem);
//                 itemSlot.Width.Set(40, 0f);
//                 itemSlot.Height.Set(40, 0f);
//                 ItemsGrid.Add(itemSlot);
//                 ItemsGrid.UpdateOrder();
//                 ItemsGrid.Recalculate();

//                 Log.Info("Added item: " + item.Name);
//             }

//             // Add a scrollbar to the panel
//             UIScrollbar scrollbar = new();
//             scrollbar.HAlign = 1f;
//             scrollbar.Height.Set(0, 1f);
//             ItemsGrid.SetScrollbar(scrollbar);
//             Append(scrollbar);
//         }
//     }
// }