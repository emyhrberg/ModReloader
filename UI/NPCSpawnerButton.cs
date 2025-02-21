using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.UI.Spawners;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class NPCSpawnerButton : BaseButton
    {
        public NPCSpawnerButton(Asset<Texture2D> _image, string hoverText) : base(_image, hoverText)
        {
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // force open inventory
            Main.playerInventory = true;

            // Toggle the NPCSpawnerPanel.
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            NPCSpawnerPanel npcSpawnerPanel = sys?.mainState?.npcSpawnerPanel;

            if (npcSpawnerPanel != null)
            {
                // toggle Active flag if its true to false vice versa
                npcSpawnerPanel.SetNPCPanelActive(!npcSpawnerPanel.GetNPCPanelActive());
            }
        }
    }
}