using Microsoft.Xna.Framework.Graphics;
using ModReloader.Helpers;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace ModReloader.UI.Elements.PanelElements
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
            UIText text = new("X", 0.4f, true);
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
            // 1. find the first BasePanel up the hierarchy
            BasePanel panel = null;
            for (UIElement cur = Parent; cur != null && panel == null; cur = cur.Parent)
                panel = cur as BasePanel;

            // 2. if we found an active panel, deactivate it
            if (panel?.GetActive() == true)
            {
                Log.Info($"CloseButtonPanel: Deactivated panel {panel.GetType().Name}");
                panel.SetActive(false);

                // 3. deactivate the toggle button if there is one
                if (panel.AssociatedButton != null)
                    panel.AssociatedButton.ParentActive = false;
            }
        }
    }
}