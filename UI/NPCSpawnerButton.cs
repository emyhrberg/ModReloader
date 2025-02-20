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
    public class NPCSpawnerButton(Asset<Texture2D> _image, string hoverText) : BaseButton(_image, hoverText)
    {
        public NPCSpawnerPanel npcPanel;
        public bool isNPCPanelVisible = false;

        public override void LeftClick(UIMouseEvent evt)
        {
            // force open inventory
            Main.playerInventory = true;

            ToggleNPCPanel();
        }

        public void ToggleNPCPanel()
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (Parent is not UIState state)
            {
                Log.Warn("ItemsButton has no parent UIState!");
                return;
            }

            // Toggle the isNPCPanelVisible flag.
            isNPCPanelVisible = !isNPCPanelVisible;

            if (isNPCPanelVisible)
            {
                // Create the panel if it doesn't already exist.
                if (npcPanel == null)
                {
                    npcPanel = new NPCSpawnerPanel();
                    Log.Info("Created new npcPanel.");
                }

                // Only append if not already present.
                if (!state.Children.Contains(npcPanel))
                {
                    Log.Info("Appending npcPanel to parent state.");
                    state.Append(npcPanel);
                    npcPanel.SearchTextBox.Focus();
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
