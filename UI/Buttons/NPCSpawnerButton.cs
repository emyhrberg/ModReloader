using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Panels;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class NPCSpawnerButton : BaseButton
    {
        // Set custom animation dimensions
        protected override Asset<Texture2D> Spritesheet => Assets.ButtonNPCSS;
        protected override int MaxFrames => 3;
        protected override int FrameSpeed => 8;
        protected override int FrameWidth => 38;
        protected override int FrameHeight => 48;

        public NPCSpawnerButton(Asset<Texture2D> image, string hoverText, bool animating)
            : base(image, hoverText, animating)
        {
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // Close the ItemSpawnerPanel.
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            var itemSpawnerPanel = sys?.mainState?.itemSpawnerPanel;
            if (itemSpawnerPanel != null && itemSpawnerPanel.GetActive())
            {
                itemSpawnerPanel.SetActive(false);
            }

            // Toggle the NPCSpawnerPanel.
            NPCSpawnerPanel npcSpawnerPanel = sys?.mainState?.npcSpawnerPanel;

            if (npcSpawnerPanel != null)
            {
                if (npcSpawnerPanel.GetActive())
                {
                    npcSpawnerPanel.SetActive(false);
                }
                else
                {
                    npcSpawnerPanel.SetActive(true);

                    // focus on textbox
                    npcSpawnerPanel.GetCustomTextBox()?.Focus();
                }
            }
        }
    }
}