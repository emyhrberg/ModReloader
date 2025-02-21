using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
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
            // Check which our parent panel is and toggle its active.
            // Its gonna be either the itemSpawnerPanel or the npcSpawnerPanel.
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            var itemSpawnerPanel = sys?.mainState?.itemSpawnerPanel;
            var npcSpawnerPanel = sys?.mainState?.npcSpawnerPanel;

            if (Parent is ItemSpawnerPanel && itemSpawnerPanel.GetActive() == true)
            {
                if (itemSpawnerPanel != null && itemSpawnerPanel.GetActive() == true)
                {
                    itemSpawnerPanel.SetActive(false);
                }
            }
            else if (Parent is NPCSpawnerPanel && npcSpawnerPanel.GetNPCPanelActive() == true)
            {
                if (npcSpawnerPanel != null && npcSpawnerPanel.GetNPCPanelActive() == true)
                {
                    npcSpawnerPanel.SetNPCPanelActive(false);
                }
            }
        }
    }
}