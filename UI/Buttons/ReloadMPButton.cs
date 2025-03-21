using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class ReloadMPButton : BaseButton
    {
        // Constructor
        public ReloadMPButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, float textSize) : base(spritesheet, buttonText, hoverText, textSize)
        {
            // deactived by default since the SP button is active
            Active = true;
            ButtonText.Active = true;
        }

        // Set custom animation dimensions
        protected override float Scale => 1.25f;
        protected override int FrameCount => 5;
        protected override int FrameSpeed => 12;
        protected override int FrameWidth => 65;
        protected override int FrameHeight => 65;

        public async override void LeftClick(UIMouseEvent evt)
        {
            // If alt+click, toggle the mode and return
            bool altClick = Main.keyState.IsKeyDown(Keys.LeftAlt);
            if (altClick)
            {
                Active = false;
                ButtonText.Active = false;

                // set MP active
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                foreach (var btn in sys?.mainState?.AllButtons)
                {
                    if (btn is ReloadSPButton spBtn)
                    {
                        spBtn.Active = true;
                        spBtn.ButtonText.Active = true;
                    }
                }
                return;
            }

            ReloadUtilities.PrepareClient(ClientMode.MPMain);

            // we must be in multiplayer for this to have an effect
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                await ReloadUtilities.ExitAndKillServer();
                ReloadUtilities.BuildAndReloadMod();
            }
        }

        public override void RightClick(UIMouseEvent evt)
        {
            // If right click, toggle the mode and return
            Active = false;
            ButtonText.Active = false;

            // set MP active
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            foreach (var btn in sys?.mainState?.AllButtons)
            {
                if (btn is ReloadSPButton spBtn)
                {
                    spBtn.Active = true;
                    spBtn.ButtonText.Active = true;
                }
            }
            return;
        }
    }
}