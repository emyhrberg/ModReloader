using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.UI.Elements;
using ReLogic.Content;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.Buttons
{
    public class ItemButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, string hoverTextDescription) : BaseButton(spritesheet, buttonText, hoverText, hoverTextDescription)
    {
        // Set custom animation dimensions
        private float _scale = 1.2f;
        protected override float Scale => _scale;
        protected override int FrameCount => 5;
        protected override int FrameSpeed => 12;
        protected override int FrameWidth => 40;
        protected override int FrameHeight => 40;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update Scale dynamically based on the size of the button
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            MainState mainState = sys?.mainState;
            float buttonSize = mainState?.ButtonSize ?? 70f;
            _scale = 1.2f + (buttonSize - 70f) * 0.005f;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            // Toggle the ItemsPanel.
            ItemSpawner panel = sys?.mainState?.itemSpawnerPanel;
            if (panel != null)
            {
                // Close the NPC spawner if open
                var npcPanel = sys.mainState.npcSpawnerPanel;
                if (npcPanel != null && npcPanel.GetActive())
                {
                    npcPanel.SetActive(false);
                    var npcButton = sys.mainState.AllButtons.Find(x => x is NPCButton);
                    if (npcButton != null)
                        npcButton.ParentActive = false;
                }

                // Use the new helper to ensure other left-side panels are closed
                sys.mainState.TogglePanel(panel);
                ParentActive = panel.GetActive();
            }
        }
    }
}