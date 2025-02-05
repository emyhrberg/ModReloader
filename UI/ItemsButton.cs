using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ItemsButton : BaseButton
    {
        private readonly ILog logger;
        private ItemsPanel itemsPanel;
        private bool isPanelVisible = false;

        public ItemsButton(Asset<Texture2D> texture, string hoverText)
            : base(texture, hoverText)
        {
            logger = ModContent.GetInstance<SquidTestingMod>().Logger;
        }

        public void HandleClick(UIMouseEvent evt, UIElement listeningElement)
        {
            // Toggle the panel's visibility flag.
            isPanelVisible = !isPanelVisible;

            // Ensure the button is part of a UIState. (ButtonState in our case right now)
            if (Parent is not UIState state)
            {
                logger.Warn("ItemsButton has no parent UIState!");
                return;
            }

            if (isPanelVisible)
            {
                // Create the panel if it doesn't already exist.
                if (itemsPanel == null)
                {
                    itemsPanel = new ItemsPanel();
                    logger.Info("Created new ItemsPanel.");
                }

                logger.Info("Appending ItemsPanel to parent state.");
                state.Append(itemsPanel);

                // Force recalculation of the layout.
                itemsPanel.Recalculate();
                state.Recalculate();
            }
            else
            {
                logger.Info("Removing ItemsPanel from parent state.");
                state.RemoveChild(itemsPanel);
                state.Recalculate();
            }
        }
    }
}
