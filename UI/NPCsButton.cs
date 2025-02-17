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
    public class NPCsButton : BaseButton
    {
        public NPCPanel npcPanel;
        public bool isNPCPanelVisible = false;

        public NPCsButton(Asset<Texture2D> _image, string hoverText) : base(_image, hoverText)
        {
        }

        public override void LeftClick(UIMouseEvent evt)
        {
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
