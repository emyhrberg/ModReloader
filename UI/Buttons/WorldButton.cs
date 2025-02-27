using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class WorldButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText) : BaseButton(spritesheet, buttonText, hoverText)
    {
        // Set custom animation dimensions
        // Spinning globe pixel art
        protected override float SpriteScale => 0.08f;
        protected override int MaxFrames => 11;
        protected override int FrameSpeed => 7;
        protected override int FrameWidth => 1080;
        protected override int FrameHeight => 576;

        // Old pylon texture, do not delete
        // protected override float SpriteScale => 0.65f;
        // protected override int MaxFrames => 8;
        // protected override int FrameSpeed => 7;
        // protected override int FrameWidth => 48;
        // protected override int FrameHeight => 68;

        public override void LeftClick(UIMouseEvent evt)
        {
            // Toggle world panel
            var sys = ModContent.GetInstance<MainSystem>();
            var worldPanel = sys?.mainState?.worldPanel;
            var playerPanel = sys?.mainState?.playerPanel;
            var debugPanel = sys?.mainState?.debugPanel;

            // Close other panels
            if (playerPanel != null && playerPanel.GetActive())
            {
                playerPanel.SetActive(false);
            }
            if (debugPanel != null && debugPanel.GetActive())
            {
                debugPanel.SetActive(false);
            }

            // Toggle world panel
            if (worldPanel != null)
            {
                if (worldPanel.GetActive())
                {
                    worldPanel.SetActive(false);
                }
                else
                {
                    worldPanel.SetActive(true);
                }
            }
        }


    }
}
