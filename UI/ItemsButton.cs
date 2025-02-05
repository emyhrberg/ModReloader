using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ItemsButton : BaseButton
    {
        private ILog logger;
        private ItemsPanel itemsPanel;
        private bool IsPanelVisible = false;

        public ItemsButton(Asset<Texture2D> texture, string hoverText)
            : base(texture, hoverText)
        {
            logger = ModContent.GetInstance<SquidTestingMod>().Logger;
        }

        public void HandleClick(UIMouseEvent evt, UIElement listeningElement)
        {
            // Toggle the panel's visibility flag.
            IsPanelVisible = !IsPanelVisible;

            // Get the parent UIState.
            if (Parent is not UIState state)
            {
                logger.Warn("ItemsButton has no parent UIState!");
                return;
            }

            if (IsPanelVisible)
            {
                // Create the panel if it doesn't exist.
                if (itemsPanel == null)
                {
                    itemsPanel = new ItemsPanel();
                    itemsPanel.Width.Set(300f, 0f);
                    itemsPanel.Height.Set(300f, 0f);
                    itemsPanel.HAlign = 0.5f;
                    itemsPanel.VAlign = 0.5f;
                }

                logger.Info("Appending ItemsPanel to parent state.");
                state.Append(itemsPanel);
            }
            else
            {
                logger.Info("Removing ItemsPanel from parent state.");
                state.RemoveChild(itemsPanel);
            }
        }

    }
}
