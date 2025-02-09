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
        public ItemsPanel itemsPanel;
        public bool isItemsPanelVisible = false;

        public override void LeftClick(UIMouseEvent evt)
        {
            ToggleItemsPanel();

            // open inv if its not already open
            if (!Main.playerInventory)
                Main.playerInventory = true;

            // close inv if we close the panel
            if (!isItemsPanelVisible)
                Main.playerInventory = false;

        }

        public void ToggleItemsPanel()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (Parent is not UIState state)
            {
                Log.Warn("ItemsButton has no parent UIState!");
                return;
            }

            // If the NPC panel is open, remove it first.
            if (sys.mainState.npcButton != null && sys.mainState.npcButton.npcPanel != null && state.Children.Contains(sys.mainState.npcButton.npcPanel))
            {
                Log.Info("Removing NPCPanel because ItemsPanel is being toggled.");
                state.RemoveChild(sys.mainState.npcButton.npcPanel);
                sys.mainState.npcButton.npcPanel = null;
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
