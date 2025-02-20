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
    public class MinimalItemBrowserButton(Asset<Texture2D> _image, string hoverText) : BaseButton(_image, hoverText)
    {
        public MinimalItemBrowserPanel minimalItemsPanel;
        public bool isMinimalPanelVisible = false;

        public override void LeftClick(UIMouseEvent evt)
        {
            Main.playerInventory = true; // force open inventory
            ToggleMinimalItemsPanel();
        }

        public void ToggleMinimalItemsPanel()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            MainState mainState = sys?.mainState;

            // Toggle the ItemsPanel flag.
            isMinimalPanelVisible = !isMinimalPanelVisible;

            if (isMinimalPanelVisible)
            {
                // Create the panel if it doesn't already exist.
                if (minimalItemsPanel == null)
                {
                    minimalItemsPanel = new MinimalItemBrowserPanel();
                    Log.Info("Created new MinimalItemsPanel.");
                }

                // Only append if not already present.
                if (!mainState.Children.Contains(minimalItemsPanel))
                {
                    Log.Info("Appending MinimalItemsPanel to parent state.");
                    mainState.Append(minimalItemsPanel);
                }
            }
            else
            {
                Log.Info("Removing ItemsPanel from parent state.");
                if (mainState.Children.Contains(minimalItemsPanel))
                    mainState.RemoveChild(minimalItemsPanel);
            }
            mainState.Recalculate();
        }
    }
}
