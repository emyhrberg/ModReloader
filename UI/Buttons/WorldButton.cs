using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class WorldButton : BaseButton
    {
        // Set custom animation dimensions
        protected override Asset<Texture2D> Spritesheet => Assets.ButtonWorldSS;
        protected override float SpriteScale => 0.65f;
        protected override int MaxFrames => 8;
        protected override int FrameSpeed => 7;
        protected override int FrameWidth => 48;
        protected override int FrameHeight => 68;

        public WorldButton(Asset<Texture2D> image, string hoverText, bool animating) : base(image, hoverText, animating)
        {
        }

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
