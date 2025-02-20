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
    public class ItemSpawnerButton(Asset<Texture2D> _image, string hoverText) : BaseButton(_image, hoverText)
    {
        public ItemSpawnerPanel itemsPanel;
        public bool isItemsPanelVisible = false;

        // private bool _needsToggle = false;

        public override void LeftClick(UIMouseEvent evt)
        {
            // force open inventory
            Main.playerInventory = true;

            ToggleItemsPanel();
        }

        public override void Update(GameTime gameTime)
        {
            // If we press escape, close the ItemsPanel.
            // TODO currently not implemented.
            // if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            // {
            // _needsToggle = true;
            // }

            base.Update(gameTime);
        }

        public void ToggleItemsPanel()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            MainState mainState = sys?.mainState;

            // Toggle the ItemsPanel flag.
            isItemsPanelVisible = !isItemsPanelVisible;

            if (isItemsPanelVisible)
            {
                // Create the panel if it doesn't already exist.
                if (itemsPanel == null)
                {
                    itemsPanel = new ItemSpawnerPanel();
                    Log.Info("Created new ItemsPanel.");
                }

                // Only append if not already present.
                if (!mainState.Children.Contains(itemsPanel))
                {
                    Log.Info("Appending ItemsPanel to parent state.");
                    mainState.Append(itemsPanel);
                    itemsPanel.SearchTextBox?.Focus();
                }
            }
            else
            {
                Log.Info("Removing ItemsPanel from parent state.");
                if (mainState.Children.Contains(itemsPanel))
                    mainState.RemoveChild(itemsPanel);
            }
            mainState.Recalculate();
        }
    }
}
