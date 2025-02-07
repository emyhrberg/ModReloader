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
    public class ItemsButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText) : BaseButton(buttonImgText, buttonImgNoText, hoverText)
    {
        private ItemsPanel itemsPanel;
        private bool isPanelVisible = false;

        public override void HandleClick()
        {
            ToggleItemsPanel();

            // open inv if its not already open
            if (!Main.playerInventory)
                Main.playerInventory = true;

            // close inv if we close the panel
            if (!isPanelVisible)
                Main.playerInventory = false;
        }

        public void ToggleItemsPanel()
        {
            // Toggle the panel's visibility flag.
            isPanelVisible = !isPanelVisible;

            // Ensure the button is part of a UIState. (ButtonState in our case right now)
            if (Parent is not UIState state)
            {
                Log.Warn("ItemsButton has no parent UIState!");
                return;
            }

            if (isPanelVisible)
            {
                // Create the panel if it doesn't already exist.
                if (itemsPanel == null)
                {
                    itemsPanel = new ItemsPanel();
                    Log.Info("Created new ItemsPanel.");
                }

                Log.Info("Appending ItemsPanel to parent state.");
                if (!state.Children.Contains(itemsPanel))
                    state.Append(itemsPanel);

                // Force recalculation of the layout.
                itemsPanel.Recalculate();
                state.Recalculate();
            }
            else
            {
                Log.Info("Removing ItemsPanel from parent state.");
                if (state.Children.Contains(itemsPanel))
                    state.RemoveChild(itemsPanel);
                state.Recalculate();
            }
        }
    }
}
