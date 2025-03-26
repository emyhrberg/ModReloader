using System.Collections.Generic;
using ErkysModdingUtilities.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ErkysModdingUtilities.UI.Elements
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

        // public override void MouseOver(UIMouseEvent evt)
        // {
        //     BorderColor = Color.Yellow;
        // }

        // public override void MouseOut(UIMouseEvent evt)
        // {
        //     BorderColor = Color.Black;
        // }

        public override void LeftClick(UIMouseEvent evt)
        {
            var mainState = ModContent.GetInstance<MainSystem>()?.mainState;
            if (mainState == null)
                return;

            // Create AllPanels list containing LeftSide and RightSidePanels
            List<DraggablePanel> AllPanels = new();
            AllPanels.AddRange(mainState.LeftSidePanels);
            AllPanels.AddRange(mainState.RightSidePanels);

            // Use AllPanels to find the panel that is our parent.
            foreach (var p in AllPanels)
            {
                if (p != null && p.GetActive())
                {
                    p.SetActive(false);
                    // Log.Info("CloseButtonPanel: Deactivated panel with name: " + p.GetType().Name);
                    break;
                }
            }
        }
    }
}