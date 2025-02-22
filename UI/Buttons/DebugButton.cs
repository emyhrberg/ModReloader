using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class DebugButton : BaseButton
    {
        // Set custom animation dimensions
        protected override Asset<Texture2D> Spritesheet => Assets.ButtonDebugWrenchSS;
        protected override float SpriteScale => 0.8f;
        protected override int MaxFrames => 16;
        protected override int FrameSpeed => 4;
        protected override int FrameWidth => 74;
        protected override int FrameHeight => 78;

        public DebugButton(Asset<Texture2D> image, string hoverText, bool animating) : base(image, hoverText, animating)
        {
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // Toggle debug panel
            var sys = ModContent.GetInstance<MainSystem>();
            var debugPanel = sys?.mainState?.debugPanel;
            var playerPanel = sys?.mainState?.playerPanel;
            var worldPanel = sys?.mainState?.worldPanel;

            // Close other panels
            if (playerPanel != null && playerPanel.GetActive())
            {
                playerPanel.SetActive(false);
            }
            if (worldPanel != null && worldPanel.GetActive())
            {
                worldPanel.SetActive(false);
            }

            // Toggle debug panel
            if (debugPanel != null)
            {
                if (debugPanel.GetActive())
                {
                    debugPanel.SetActive(false);
                }
                else
                {
                    debugPanel.SetActive(true);
                }
            }
        }


    }
}