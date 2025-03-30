using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Helpers;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    public class CloseButtonPanel : UIPanel
    {
        public Asset<Texture2D> closeTexture;

        public CloseButtonPanel()
        {
            // closeTexture = Assets.X;
            Left.Set(12f, 0f);
            Top.Set(-12f, 0f);
            Width.Set(35, 0f);
            Height.Set(35, 0f);
            MaxWidth.Set(35, 0f);
            MaxHeight.Set(35, 0f);
            HAlign = 1f;

            // create a UIText
            UIText text = new UIText("X", 0.4f, true);
            text.HAlign = 0.5f;
            text.VAlign = 0.5f;
            Append(text);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            BorderColor = Color.Yellow;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            BorderColor = Color.Black;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // Find the parent DraggablePanel containing this close button
            UIElement current = Parent;
            while (current != null && current is not DraggablePanel)
            {
                current = current.Parent;
            }

            // If we found the parent panel, deactivate it
            if (current is DraggablePanel panel && panel.GetActive())
            {
                Log.Info("CloseButtonPanel: Deactivated panel with name: " + panel.GetType().Name);
                panel.SetActive(false);
                panel.AssociatedButton.ParentActive = false; // deactivate the button
            }
        }
    }
}