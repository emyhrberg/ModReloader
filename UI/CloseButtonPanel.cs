using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class CloseButtonPanel : UIPanel
    {
        public Asset<Texture2D> closeTexture;

        public CloseButtonPanel()
        {
            closeTexture = Assets.X;
            Left.Set(3f, 0f);
            Top.Set(-3f, 0f);
            Width.Set(30, 0f);
            Height.Set(30, 0f);
            HAlign = 1f;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // draw the panel
            base.DrawSelf(spriteBatch);

            // draw the closeTexture with 20x20 size in the center of the panel
            CalculatedStyle dimensions = GetDimensions();
            float panelCenterX = dimensions.X + (dimensions.Width / 2f);
            float panelCenterY = dimensions.Y + (dimensions.Height / 2f);
            int closeButtonSize = 20;
            Rectangle closeButtonRect = new((int)(panelCenterX - closeButtonSize / 2), (int)(panelCenterY - closeButtonSize / 2), closeButtonSize, closeButtonSize);
            spriteBatch.Draw(closeTexture.Value, closeButtonRect, Color.White);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            // make the panel border color yellow when hovering
            BorderColor = Color.Yellow;
            // UICommon.TooltipMouseText("Close");
            Main.hoverItemName = "Close";
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            BorderColor = Color.Black;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // Close the item panel for now.
            // TODO close any parent panel.
            // sys.mainState.itemButton.ToggleItemsPanel();

            // Get parent
            UIElement parent = Parent;

            if (parent == null)
            {
                Log.Info($"parent {parent.GetType().Name} is null");
                return;
            }

            // If parent is itemspanel, close it.
            // If parent is npcpanel, close it.

            // Change flag
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            if (parent is ItemSpawnerPanel)
            {
                sys.mainState.itemButton.isItemsPanelVisible = false;
            }
            else if (parent is NPCSpawnerPanel)
            {
                sys.mainState.npcButton.isNPCPanelVisible = false;
            }

            // Remove the parent from the UI
            parent.Remove();
        }
    }
}