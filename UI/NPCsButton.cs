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
    public class NPCsButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText) : BaseButton(buttonImgText, buttonImgNoText, hoverText)
    {
        public NPCPanel npcPanel;
        public bool isNPCPanelVisible = false;

        public override void LeftClick(UIMouseEvent evt)
        {
            ToggleNPCPanel();

            // open inv if its not already open
            if (!Main.playerInventory)
                Main.playerInventory = true;

            // close inv if we close the panel
            if (!isNPCPanelVisible)
                Main.playerInventory = false;
        }

        public void ToggleNPCPanel()
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
                Log.Info("Removing Itemspanel because npcPanel is being toggled.");
                state.RemoveChild(sys.mainState.npcButton.npcPanel);
                sys.mainState.npcButton.npcPanel = null;
            }

            // Toggle the isNPCPanelVisible flag.
            isNPCPanelVisible = !isNPCPanelVisible;

            if (isNPCPanelVisible)
            {
                // Create the panel if it doesn't already exist.
                if (npcPanel == null)
                {
                    npcPanel = new NPCPanel();
                    Log.Info("Created new npcPanel.");
                }

                // Only append if not already present.
                if (!state.Children.Contains(npcPanel))
                {
                    Log.Info("Appending npcPanel to parent state.");
                    state.Append(npcPanel);
                    npcPanel.searchBox.Focus();
                }
            }
            else
            {
                Log.Info("Removing npcPanel from parent state.");
                if (state.Children.Contains(npcPanel))
                    state.RemoveChild(npcPanel);
            }
            state.Recalculate();
        }
    }
}
