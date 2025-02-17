using System.Linq;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ItemsButton : BaseButton
    {
        public ItemsPanel itemsPanel;
        public bool isItemsPanelVisible = false;

        public ItemsButton(Asset<Texture2D> _image, string hoverText) : base(_image, hoverText)
        {
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            ToggleItemsPanel();
        }

        public void ToggleItemsPanel()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (Parent is not UIState state)
            {
                Log.Warn("ItemsButton has no parent UIState!");
                return;
            }

            // Toggle the ItemsPanel flag.
            isItemsPanelVisible = !isItemsPanelVisible;

            if (isItemsPanelVisible)
            {
                // Create the panel if it doesn't already exist.
                if (itemsPanel == null)
                {
                    itemsPanel = new ItemsPanel();
                    Log.Info("Created new ItemsPanel.");
                }

                // Only append if not already present.
                if (!state.Children.Contains(itemsPanel))
                {
                    Log.Info("Appending ItemsPanel to parent state.");
                    state.Append(itemsPanel);
                    itemsPanel.searchBox.Focus();
                }
            }
            else
            {
                Log.Info("Removing ItemsPanel from parent state.");
                if (state.Children.Contains(itemsPanel))
                    state.RemoveChild(itemsPanel);
            }
            state.Recalculate();
        }
    }
}
